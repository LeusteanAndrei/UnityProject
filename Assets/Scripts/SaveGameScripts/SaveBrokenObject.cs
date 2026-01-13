using System;
using Unity.VisualScripting;
using UnityEngine;

public class SaveBrokenObject : MonoBehaviour, ISaveGame
{
    [SerializeField] int objectId;
    Collisions collisions;
    GoalObject goalObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisions = GetComponent<Collisions>();
        goalObject = GetComponent<GoalObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(GameData data)
    {
        Array.Sort(data.brokenObjectIds);

        for (int i = 0; i < data.brokenObjectIds.Length; i++)
        {
            if (data.brokenObjectIds[i] == objectId)
            {
                if (collisions != null)
                {
                    collisions.fromLoad = true;
                    collisions.destroyed = true;
                }
                if(GetComponent<BreakableValueMonitor>()!=null)
                {
                    GetComponent<BreakableValueMonitor>().NotifyGoal();
                }
                if (!GetComponent<Chandelier>())
                {
                    gameObject.SetActive(false);  
                }
                else
                {
                    HingeJoint hj = GetComponent<HingeJoint>();
                    if (hj != null)
                        Destroy(hj);
                }
                if (goalObject != null)
                {
                    goalObject.Mark();
                }
            }

        }
    }
    public void SaveData(GameData data)
    {
        if(GetComponent<Chandelier>()!=null)
        {
            if(GetComponent<GoalObject>()!=null)
            {
                if(GetComponent<GoalObject>().IsMarked() == true  )
                {
                    data.AddBrokenObjectId(objectId);
                }
            }
        }
        if (collisions)
        {
            if (collisions.IsDestroyed())
            {
                data.AddBrokenObjectId(objectId);
            }
        }
    }
}
