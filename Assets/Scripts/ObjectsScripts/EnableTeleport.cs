using UnityEngine;
using System.Collections.Generic;
public class EnableTeleport : MonoBehaviour
{
    [SerializeField] private List<GoalObject> goalObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GoalObject goal in goalObjects)
        {
            if(!goal.IsMarked())
            {
                return;
            }
        }
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
