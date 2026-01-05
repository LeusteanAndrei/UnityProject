using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Hanzzz.MeshSlicerFree;

public static class MultiPlaneSlicer
{
    public static List<GameObject> SliceWithPlanes(GameObject target, Material intersectionMaterial, params Plane[] planes)
    {
        var results = new List<GameObject>();
        if (target == null || planes == null || planes.Length == 0)
        {
            return results;
        }

        var work = new List<GameObject> { target };
        var meshSlicer = new MeshSlicer();
        var skinnedMeshSlicer = new SkinnedMeshSlicer();

        foreach (var plane in planes)
        {
            var next = new List<GameObject>();
            foreach (var obj in work)
            {
                var hasSkinned = obj.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                (GameObject, GameObject) sliced;
                if (hasSkinned)
                {
                    var (smrIndex, rootIndex) = ResolveSkinnedIndices(obj);
                    if (smrIndex < 0)
                    {
                        // Fallback to non-sliced if no valid SMR found
                        sliced = (null, null);
                    }
                    else
                    {
                        try
                        {
                            sliced = skinnedMeshSlicer.Slice(obj, smrIndex, rootIndex, Get3PointsOnPlane(plane), intersectionMaterial);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"MultiPlaneSlicer: Skinned slicing failed for '{obj.name}' (smrIndex={smrIndex}, rootIndex={rootIndex}): {ex.Message}");
                            sliced = (null, null);
                        }
                    }
                }
                else
                {
                    sliced = meshSlicer.Slice(obj, Get3PointsOnPlane(plane), intersectionMaterial);
                }

                if (sliced.Item1 != null && sliced.Item2 != null)
                {
                    var parent = obj.transform.parent;
                    if (parent != null)
                    {
                        sliced.Item1.transform.SetParent(parent, false);
                        sliced.Item2.transform.SetParent(parent, false);
                    }
                    obj.SetActive(false);
                    next.Add(sliced.Item1);
                    next.Add(sliced.Item2);
                }
                else
                {
                    next.Add(obj);
                }
            }
            work = next;
        }

        results.AddRange(work);
        return results;
    }

    public static async Task<List<GameObject>> SliceWithPlanesAsync(GameObject target, Material intersectionMaterial, params Plane[] planes)
    {
        var results = new List<GameObject>();
        if (target == null || planes == null || planes.Length == 0)
        {
            return results;
        }

        var work = new List<GameObject> { target };
        var meshSlicer = new MeshSlicer();
        var skinnedMeshSlicer = new SkinnedMeshSlicer();

        foreach (var plane in planes)
        {
            var next = new List<GameObject>();
            foreach (var obj in work)
            {
                var hasSkinned = obj.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                (GameObject, GameObject) sliced;
                if (hasSkinned)
                {
                    var (smrIndex, rootIndex) = ResolveSkinnedIndices(obj);
                    if (smrIndex < 0)
                    {
                        sliced = (null, null);
                    }
                    else
                    {
                        try
                        {
                            sliced = await skinnedMeshSlicer.SliceAsync(obj, smrIndex, rootIndex, Get3PointsOnPlane(plane), intersectionMaterial);
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError($"MultiPlaneSlicer: Skinned slicing failed for '{obj.name}' (smrIndex={smrIndex}, rootIndex={rootIndex}): {ex.Message}");
                            sliced = (null, null);
                        }
                    }
                }
                else
                {
                    sliced = await meshSlicer.SliceAsync(obj, Get3PointsOnPlane(plane), intersectionMaterial);
                }

                if (sliced.Item1 != null && sliced.Item2 != null)
                {
                    var parent = obj.transform.parent;
                    if (parent != null)
                    {
                        sliced.Item1.transform.SetParent(parent, false);
                        sliced.Item2.transform.SetParent(parent, false);
                    }
                    obj.SetActive(false);
                    next.Add(sliced.Item1);
                    next.Add(sliced.Item2);
                }
                else
                {
                    next.Add(obj);
                }
            }
            work = next;
        }

        results.AddRange(work);
        return results;
    }

    public static List<GameObject> SliceWithPlaneTransforms(GameObject target, Material intersectionMaterial, params Transform[] planeTransforms)
    {
        if (planeTransforms == null || planeTransforms.Length == 0)
        {
            return new List<GameObject>();
        }
        var planes = new Plane[planeTransforms.Length];
        for (int i = 0; i < planeTransforms.Length; i++)
        {
            var t = planeTransforms[i];
            planes[i] = new Plane(t.up, t.position);
        }
        return SliceWithPlanes(target, intersectionMaterial, planes);
    }

    public static Task<List<GameObject>> SliceWithPlaneTransformsAsync(GameObject target, Material intersectionMaterial, params Transform[] planeTransforms)
    {
        if (planeTransforms == null || planeTransforms.Length == 0)
        {
            return Task.FromResult(new List<GameObject>());
        }
        var planes = new Plane[planeTransforms.Length];
        for (int i = 0; i < planeTransforms.Length; i++)
        {
            var t = planeTransforms[i];
            planes[i] = new Plane(t.up, t.position);
        }
        return SliceWithPlanesAsync(target, intersectionMaterial, planes);
    }

    private static (Vector3, Vector3, Vector3) Get3PointsOnPlane(Plane p)
    {
        Vector3 xAxis;
        if (0f != p.normal.x)
        {
            xAxis = new Vector3(-p.normal.y / p.normal.x, 1f, 0f);
        }
        else if (0f != p.normal.y)
        {
            xAxis = new Vector3(0f, -p.normal.z / p.normal.y, 1f);
        }
        else
        {
            xAxis = new Vector3(1f, 0f, -p.normal.x / p.normal.z);
        }
        Vector3 yAxis = Vector3.Cross(p.normal, xAxis);
        return (-p.distance * p.normal, -p.distance * p.normal + xAxis, -p.distance * p.normal + yAxis);
    }

    private static (int smrIndex, int rootIndex) ResolveSkinnedIndices(GameObject obj)
    {
        var smrs = obj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (smrs == null || smrs.Length == 0)
        {
            return (-1, -1);
        }

        // Choose the first SMR with a valid sharedMesh and bones
        int chosenSmr = -1;
        for (int i = 0; i < smrs.Length; i++)
        {
            var smr = smrs[i];
            if (smr != null && smr.sharedMesh != null && smr.bones != null && smr.bones.Length > 0)
            {
                chosenSmr = i;
                break;
            }
        }
        if (chosenSmr < 0)
        {
            // fallback to first available
            chosenSmr = 0;
        }

        var targetSmr = smrs[chosenSmr];
        int rootIndex = 0;
        if (targetSmr != null)
        {
            var bones = targetSmr.bones;
            var root = targetSmr.rootBone;
            if (bones != null && bones.Length > 0 && root != null)
            {
                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i] == root)
                    {
                        rootIndex = i;
                        break;
                    }
                }
            }
        }

        return (chosenSmr, rootIndex);
    }
}
