using System.Collections.Generic;
using UnityEngine;

public class LaserSystem : MonoBehaviour
{
    [SerializeField] private float defaultThickness = 0.1f;

    private readonly Dictionary<string, LaserBeam> activeBeams = new Dictionary<string, LaserBeam>();

    private void Start()
    {
        BuildBeams();
    }

    public void BuildBeams()
    {
        foreach (var kv in activeBeams)
        {
            if (kv.Value)
            {
                Destroy(kv.Value.gameObject);
            }
        }
        activeBeams.Clear();

        var endpoints = FindObjectsOfType<LaserEndpoint>(includeInactive: false);
        var byId = new Dictionary<string, List<LaserEndpoint>>();
        foreach (var ep in endpoints)
        {
            if (ep == null || string.IsNullOrEmpty(ep.laserId)) continue;
            if (!byId.TryGetValue(ep.laserId, out var list))
            {
                list = new List<LaserEndpoint>();
                byId[ep.laserId] = list;
            }
            list.Add(ep);
        }

        foreach (var pair in byId)
        {
            var id = pair.Key;
            var list = pair.Value;
            if (list.Count < 2) { Debug.LogWarning($"Laser id '{id}' has less than 2 endpoints."); continue; }
            if (list.Count > 2) { Debug.LogWarning($"Laser id '{id}' has more than 2 endpoints. Using the first two."); }

            var a = list[0].transform;
            var b = list[1].transform;

            var go = new GameObject($"LaserBeam_{id}");
            var beam = go.AddComponent<LaserBeam>();
            var lr = go.GetComponent<LineRenderer>();

            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.red;
            lr.endColor = Color.red;

            beam.Initialize(a, b, defaultThickness);
            activeBeams[id] = beam;
        }
    }
}
