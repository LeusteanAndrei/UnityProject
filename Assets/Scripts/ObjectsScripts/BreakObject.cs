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
    [Tooltip("Remove colliders cloned from the original before adding new ones.")]
    public bool removeInheritedColliders = true;

    public bool destroyOriginalAfterSlice = true;
    private bool _sliced;
    public bool showPlaneGizmos = true;
    public float gizmoPlaneSize = 0.5f;
    public Color gizmoPlaneColor = new Color(0.2f, 0.8f, 1f, 0.6f);
    public Color gizmoNormalColor = new Color(1f, 0.2f, 0.2f, 0.9f);

    [Tooltip("If true, child meshes are baked into a single combined mesh and included in the slice.")]
    public bool bakeChildrenIntoSlice = true;
    [Tooltip("Remove any other GameObjects created during slice that are not part of the returned pieces.")]
    public bool destroyNonPieceOutputs = true;

    private List<GameObject> _sliceNewObjects;

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
        var pieces = RunSliceWithBakeAndCapture(() => MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms));
        PostSlice(pieces, explosionOrigin);
    }

    public void SliceAndExplode(Plane[] planes, Vector3 explosionOrigin)
    {
        if (_sliced) return;

        List<GameObject> pieces;
        if (planeTransforms != null && planeTransforms.Length > 0)
        {
            pieces = RunSliceWithBakeAndCapture(() => MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms));
        }
        else
        {
            pieces = RunSliceWithBakeAndCapture(() => MultiPlaneSlicer.SliceWithPlanes(sliceTarget, intersectionMaterial, planes));
        }
        PostSlice(pieces, explosionOrigin);
    }

    private List<GameObject> RunSliceWithBake(System.Func<List<GameObject>> slicer)
    {
        if (!bakeChildrenIntoSlice || sliceTarget == null) return slicer();

        // Build a temporary combined mesh GameObject from sliceTarget hierarchy
        var combinedGO = new GameObject("__SliceCombined");
        // Use identity transform because we will feed localToWorld matrices per source mesh.
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

        // Build a set of transforms to exclude: any child under planeTransforms
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
            // Skip meshes that belong to plane helper objects
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
                // Collect first material per renderer (Multi-mat meshes will be simplified)
                if (mr.sharedMaterial != null) materials.Add(mr.sharedMaterial);
            }
        }

        var combinedMesh = new Mesh();
        combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // allow large meshes
        if (combines.Count == 0)
        {
            Object.Destroy(combinedGO);
            return slicer();
        }
        combinedMesh.CombineMeshes(combines.ToArray(), mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
        combinedMesh.RecalculateBounds();
        mfCombined.sharedMesh = combinedMesh;
        // Use first material found or fallback to original renderer if present
        if (materials.Count > 0)
        {
            mrCombined.sharedMaterial = materials[0];
        }
        else
        {
            var origMR = sliceTarget.GetComponent<MeshRenderer>();
            if (origMR != null) mrCombined.sharedMaterial = origMR.sharedMaterial;
        }

        // Redirect sliceTarget to the combined mesh for slicing
        var originalTarget = sliceTarget;
        sliceTarget = combinedGO;

        List<GameObject> pieces = null;
        try
        {
            pieces = slicer();
        }
        finally
        {
            // Restore sliceTarget and cleanup temp combined object
            sliceTarget = originalTarget;
            Object.Destroy(combinedGO);
        }

        return pieces;
    }

    private List<GameObject> RunSliceWithBakeAndCapture(System.Func<List<GameObject>> slicer)
    {
        // Snapshot pre-slice objects
        var pre = Resources.FindObjectsOfTypeAll<GameObject>();
        var preIds = new HashSet<int>();
        for (int i = 0; i < pre.Length; i++) preIds.Add(pre[i].GetInstanceID());

        var pieces = RunSliceWithBake(slicer);

        // Snapshot post-slice and capture newly created objects
        var post = Resources.FindObjectsOfTypeAll<GameObject>();
        var created = new List<GameObject>();
        for (int i = 0; i < post.Length; i++)
        {
            var go = post[i];
            if (!preIds.Contains(go.GetInstanceID())) created.Add(go);
        }
        _sliceNewObjects = created;
        return pieces;
    }

    private void PostSlice(List<GameObject> pieces, Vector3 explosionOrigin)
    {
        if (pieces == null || pieces.Count == 0)
        {
            return;
        }

        _sliced = true;

        // Create a parent to organize resulting pieces
        var parent = new GameObject("SlicedPieces");
        parent.transform.SetPositionAndRotation(sliceTarget.transform.position, sliceTarget.transform.rotation);
        parent.transform.localScale = sliceTarget.transform.lossyScale;

        if (destroyOriginalAfterSlice)
        {
            sliceTarget.SetActive(false);
        }
        // Immediately organize: put all returned pieces under the parent, even if disabled
        for (int i = 0; i < pieces.Count; i++)
        {
            var go = pieces[i];
            if (go == null) continue;
            go.transform.SetParent(parent.transform, worldPositionStays: true);
        }


        foreach (var piece in pieces)
        {
            // Reparent first so even inactive objects get organized under the parent
            piece.transform.SetParent(parent.transform, worldPositionStays: true);

            // Ensure piece is active after reparenting
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

            // Remove any colliders carried over from Instantiate (e.g., BoxCollider)
            if (removeInheritedColliders)
            {
                var inherited = piece.GetComponents<Collider>();
                for (int i = 0; i < inherited.Length; i++)
                {
                    // Don't remove MeshCollider if we plan to reuse it, we'll recreate below anyway
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

        // Destroy non-piece outputs created during slice (helpers, caps) if requested
        if (destroyNonPieceOutputs && _sliceNewObjects != null && _sliceNewObjects.Count > 0)
        {
            for (int i = 0; i < _sliceNewObjects.Count; i++)
            {
                var go = _sliceNewObjects[i];
                if (go == null) continue;
                bool isPiece = false;
                for (int j = 0; j < pieces.Count; j++)
                {
                    if (pieces[j] == go)
                    {
                        isPiece = true;
                        break;
                    }
                }
                if (isPiece) continue;
                Object.Destroy(go);
            }
            _sliceNewObjects = null;
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
