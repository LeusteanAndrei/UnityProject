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
        var pieces = MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms);
        PostSlice(pieces, explosionOrigin);
    }

    public void SliceAndExplode(Plane[] planes, Vector3 explosionOrigin)
    {
        if (_sliced) return;

        List<GameObject> pieces;
        if (planeTransforms != null && planeTransforms.Length > 0)
        {
            pieces = MultiPlaneSlicer.SliceWithPlaneTransforms(sliceTarget, intersectionMaterial, planeTransforms);
        }
        else
        {
            pieces = MultiPlaneSlicer.SliceWithPlanes(sliceTarget, intersectionMaterial, planes);
        }
        PostSlice(pieces, explosionOrigin);
    }

    private void PostSlice(List<GameObject> pieces, Vector3 explosionOrigin)
    {
        if (pieces == null || pieces.Count == 0)
        {
            return;
        }

        _sliced = true;

        if (destroyOriginalAfterSlice)
        {
            sliceTarget.SetActive(false);
        }
        foreach (var piece in pieces)
        {
            Rigidbody rb = null;

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
