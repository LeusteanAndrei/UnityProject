using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public string levelName;
    public float soundMeterLevel;

    public float[] playerPosition;
    public int[] brokenObjectIds;

    public GameData()
    {
        soundMeterLevel = 0;
        levelName = "Level 1";
        playerPosition = null;
        brokenObjectIds = new int[0];
    }
    public GameData(float soundMeterLevel)
    {
        this.soundMeterLevel = soundMeterLevel;
    }


    //public void addBrokenObjectId(int newId)
    //{
    //    int[] oldIds = new int[brokenObjectIds.Length + 1];
    //    for (int i = 0; i < brokenObjectIds.Length; i++)
    //    {
    //        oldIds[i] = brokenObjectIds[i];
    //    }
    //    oldIds[oldIds.Length-1] = newId;
    //    brokenObjectIds = oldIds;
    //}

    public void AddBrokenObjectId(int newId)
    {
        if (System.Array.Exists(brokenObjectIds, id => id == newId))
        {
            return;
        }

        int[] newArray = new int[brokenObjectIds.Length + 1];

        for (int i = 0; i < brokenObjectIds.Length; i++)
        {
            newArray[i] = brokenObjectIds[i];
        }

        newArray[newArray.Length - 1] = newId;

        brokenObjectIds = newArray;

    }
}
