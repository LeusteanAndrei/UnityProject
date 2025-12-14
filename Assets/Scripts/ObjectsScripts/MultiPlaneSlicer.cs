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
                // Capture pre-slice siblings to identify helper outputs created by the slicer
                Transform parent = obj.transform.parent;
                var preSet = new HashSet<int>();
                if (parent != null)
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        var c = parent.GetChild(i).gameObject;
                        preSet.Add(c.GetInstanceID());
                    }
                }
                var hasSkinned = obj.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                (GameObject, GameObject) sliced;
                if (hasSkinned)
                {
                    sliced = skinnedMeshSlicer.Slice(obj, 0, 1, Get3PointsOnPlane(plane), intersectionMaterial);
                }
                else
                {
                    sliced = meshSlicer.Slice(obj, Get3PointsOnPlane(plane), intersectionMaterial);
                }

                if (sliced.Item1 != null && sliced.Item2 != null)
                {
                    parent = obj.transform.parent;
                    if (parent != null)
                    {
                        sliced.Item1.transform.SetParent(parent, false);
                        sliced.Item2.transform.SetParent(parent, false);

                        // Remove any newly created inactive helper objects under the same parent not part of the two pieces
                        for (int i = 0; i < parent.childCount; i++)
                        {
                            var childGO = parent.GetChild(i).gameObject;
                            int id = childGO.GetInstanceID();
                            if (preSet.Contains(id)) continue; // existed before
                            if (childGO == sliced.Item1 || childGO == sliced.Item2) continue; // keep the real pieces
                            if (!childGO.activeSelf)
                            {
                                Object.Destroy(childGO);
                            }
                        }
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
                // Capture pre-slice siblings to identify helper outputs created by the slicer
                Transform parent = obj.transform.parent;
                var preSet = new HashSet<int>();
                if (parent != null)
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        var c = parent.GetChild(i).gameObject;
                        preSet.Add(c.GetInstanceID());
                    }
                }
                var hasSkinned = obj.GetComponentInChildren<SkinnedMeshRenderer>() != null;
                (GameObject, GameObject) sliced;
                if (hasSkinned)
                {
                    sliced = await skinnedMeshSlicer.SliceAsync(obj, 0, 1, Get3PointsOnPlane(plane), intersectionMaterial);
                }
                else
                {
                    sliced = await meshSlicer.SliceAsync(obj, Get3PointsOnPlane(plane), intersectionMaterial);
                }

                if (sliced.Item1 != null && sliced.Item2 != null)
                {
                    parent = obj.transform.parent;
                    if (parent != null)
                    {
                        sliced.Item1.transform.SetParent(parent, false);
                        sliced.Item2.transform.SetParent(parent, false);

                        // Remove any newly created inactive helper objects under the same parent not part of the two pieces
                        for (int i = 0; i < parent.childCount; i++)
                        {
                            var childGO = parent.GetChild(i).gameObject;
                            int id = childGO.GetInstanceID();
                            if (preSet.Contains(id)) continue;
                            if (childGO == sliced.Item1 || childGO == sliced.Item2) continue;
                            if (!childGO.activeSelf)
                            {
                                Object.Destroy(childGO);
                            }
                        }
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
}
