using System.Collections.Generic;
using UnityEngine;

public class BreakObject : MonoBehaviour
{
    public GameObject sliceTarget;
    public Material intersectionMaterial;    
    public Transform[] planeTransforms;
    public LayerMask sliceOnLayers = ~0;
    public float minRelativeVelocity = 0.1f;
    public float pieceMass = 1f;
    public bool addMeshColliders = true;
    public bool convexColliders = true;
    public float explosionForce = 200f;
    public float explosionRadius = 1.5f;
    public float explosionUpwards = 0.2f;
    public ForceMode forceMode = ForceMode.Impulse;
    public bool enableExplosion = true;
    public bool removeInheritedColliders = true;
    private bool _sliced;
    public bool showPlaneGizmos = true;
    public float gizmoPlaneSize = 0.5f;
    public Color gizmoPlaneColor = new Color(0.2f, 0.8f, 1f, 0.6f);
    public Color gizmoNormalColor = new Color(1f, 0.2f, 0.2f, 0.9f);
    public bool bakeChildrenIntoSlice = true;

    private void Reset()
    {
        sliceTarget = gameObject;
    }

    public void Slice()
    {
        var origin = sliceTarget != null ? sliceTarget.transform.position : transform.position;
        if (planeTransforms != null && planeTransforms.Length > 0)
        {
            SliceAndExplodeWithTransforms(origin);
        }
        else
        {
            SliceAndExplode(new[] { new Plane(transform.up, transform.position) }, origin);
        }
    }

    public void SliceAndExplodeWithTransforms(Vector3 explosionOrigin)
    {
        if (_sliced) return;
        var pieces = RunSliceWithBake(() => MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms));
        PostSlice(pieces, explosionOrigin);
    }

    public void SliceAndExplode(Plane[] planes, Vector3 explosionOrigin)
    {
        if (_sliced) return;

        List<GameObject> pieces;
        if (planeTransforms != null && planeTransforms.Length > 0)
        {
            pieces = RunSliceWithBake(() => MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms));
        }
        else
        {
            pieces = RunSliceWithBake(() => MultiPlaneSlicer.SliceWithPlanes(sliceTarget, intersectionMaterial, planes));
        }
        PostSlice(pieces, explosionOrigin);
    }

    private List<GameObject> RunSliceWithBake(System.Func<List<GameObject>> slicer)
    {
        if (!bakeChildrenIntoSlice || sliceTarget == null) return slicer();

        var combinedGO = new GameObject("SliceCombined");
        combinedGO.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        combinedGO.transform.localScale = Vector3.one;

        var mfCombined = combinedGO.AddComponent<MeshFilter>();
        var mrCombined = combinedGO.AddComponent<MeshRenderer>();

        var meshFilters = sliceTarget.GetComponentsInChildren<MeshFilter>(includeInactive: false);
        if (meshFilters == null || meshFilters.Length == 0)
        {
            Object.Destroy(combinedGO);
            return slicer();
        }

        var combines = new List<UnityEngine.CombineInstance>(meshFilters.Length);
        var materials = new List<Material>();

        var excludeRoots = new HashSet<Transform>();
        if (planeTransforms != null)
        {
            foreach (var pt in planeTransforms)
            {
                if (pt == null) continue;
                excludeRoots.Add(pt);
            }
        }

        bool IsUnderExcluded(Transform t)
        {
            var cur = t;
            while (cur != null)
            {
                if (excludeRoots.Contains(cur)) return true;
                cur = cur.parent;
            }
            return false;
        }
        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null) continue;
            if (IsUnderExcluded(mf.transform)) continue;
            var ci = new UnityEngine.CombineInstance
            {
                mesh = mf.sharedMesh,
                transform = mf.transform.localToWorldMatrix
            };
            combines.Add(ci);

            var mr = mf.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                if (mr.sharedMaterial != null) materials.Add(mr.sharedMaterial);
            }
        }

        var combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        if (combines.Count == 0)
        {
            Object.Destroy(combinedGO);
            return slicer();
        }
        combinedMesh.CombineMeshes(combines.ToArray(), mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
        combinedMesh.RecalculateBounds();
        mfCombined.sharedMesh = combinedMesh;
        if (materials.Count > 0)
        {
            mrCombined.sharedMaterial = materials[0];
        }
        else
        {
            var origMR = sliceTarget.GetComponent<MeshRenderer>();
            if (origMR != null) mrCombined.sharedMaterial = origMR.sharedMaterial;
        }

        var originalTarget = sliceTarget;
        sliceTarget = combinedGO;

        List<GameObject> pieces = null;
        try
        {
            pieces = slicer();
        }
        finally
        {
            sliceTarget = originalTarget;
            Object.Destroy(combinedGO);
        }

        return pieces;
    }

    private void PostSlice(List<GameObject> pieces, Vector3 explosionOrigin)
    {
        if (pieces == null || pieces.Count == 0)
        {
            return;
        }

        _sliced = true;

        // Disable the original slice target now that pieces were generated.
        // When baking, the slicer operates on a temporary combined object,
        // so we must explicitly disable the real original here.
        if (sliceTarget != null && sliceTarget.activeSelf)
        {
            sliceTarget.SetActive(false);
        }

        // Create a parent to organize resulting pieces
        var parent = new GameObject("SlicedPieces");
        parent.transform.SetPositionAndRotation(sliceTarget.transform.position, sliceTarget.transform.rotation);
        parent.transform.localScale = sliceTarget.transform.lossyScale;

        for (int i = 0; i < pieces.Count; i++)
        {
            var go = pieces[i];
            if (go == null) continue;
            go.transform.SetParent(parent.transform, worldPositionStays: true);
        }


        foreach (var piece in pieces)
        {

            if (!piece.activeSelf) piece.SetActive(true);

            // Inherit basic identity from original target for consistency
            piece.layer = sliceTarget.layer;
            piece.tag = sliceTarget.tag;

            var rb = piece.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = piece.AddComponent<Rigidbody>();
            }
            rb.mass = Mathf.Max(0.0001f, pieceMass);

            if (removeInheritedColliders)
            {
                var inherited = piece.GetComponents<Collider>();
                for (int i = 0; i < inherited.Length; i++)
                {
                    Object.Destroy(inherited[i]);
                }
            }

            if (addMeshColliders)
            {
                var mc = piece.GetComponent<MeshCollider>();
                if (mc == null) mc = piece.AddComponent<MeshCollider>();
                mc.convex = convexColliders;
            }

            if (rb != null && enableExplosion)
            {
                rb.AddExplosionForce(explosionForce, explosionOrigin, Mathf.Max(0.01f, explosionRadius), explosionUpwards, forceMode);
            }
        }


    }
    private void OnDrawGizmosSelected()
    {
        if (!showPlaneGizmos) return;
        if (planeTransforms == null || planeTransforms.Length == 0) return;

        foreach (var t in planeTransforms)
        {
            if (t == null) continue;
            var center = t.position;
            var right = t.right * gizmoPlaneSize;
            var forward = t.forward * gizmoPlaneSize;

            Gizmos.color = gizmoPlaneColor;
            var p0 = center - right - forward;
            var p1 = center + right - forward;
            var p2 = center + right + forward;
            var p3 = center - right + forward;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);

            Gizmos.color = gizmoNormalColor;
            var nStart = center;
            var nEnd = center + t.up * gizmoPlaneSize * 1.5f;
            Gizmos.DrawLine(nStart, nEnd);
            var headRight = Vector3.Cross(t.up, t.right).normalized;
            Gizmos.DrawLine(nEnd, nEnd - t.up * gizmoPlaneSize * 0.3f + headRight * gizmoPlaneSize * 0.2f);
            Gizmos.DrawLine(nEnd, nEnd - t.up * gizmoPlaneSize * 0.3f - headRight * gizmoPlaneSize * 0.2f);
        }
    }
}
