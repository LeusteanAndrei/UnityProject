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
        for (int i = 0; i < data.brokenObjectIds.Length; i++)
        {
            if (data.brokenObjectIds[i] == objectId)
            {
                collisions.fromLoad = true;
                collisions.destroyed = true;
                if(GetComponent<BreakableValueMonitor>()!=null)
                {
                    GetComponent<BreakableValueMonitor>().NotifyGoal();
                }
                if(!GetComponent<Chandelier>())
                    gameObject.SetActive(false);
                if (goalObject != null)
                {
                    goalObject.Mark();
                }
            }

        }
    }
    public void SaveData(GameData data)
    {
        if (collisions.IsDestroyed())
        {
            data.AddBrokenObjectId(objectId);
        }
    }
}
