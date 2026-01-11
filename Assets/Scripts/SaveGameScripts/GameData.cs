using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;


[System.Serializable]
public class EnemyData
{
    public int enemyId;
    public float[] enemyPoz;
    public int currentPathPoint = 0;
}


[System.Serializable]
public class GameData 
{
    public string[] completedLevels = { "Level 1" };
    public bool fullscreen = true;
    public float volume = 1.0f;
    public int resolutionWidth, resolutionHeight;


    public string levelName;
    public float soundMeterLevel;

    public float[] playerPosition;
    public int[] brokenObjectIds;

    public EnemyData[] enemyData = new EnemyData[0];

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


    public void AddEnemyData(EnemyData newEnemyData)
    {
        if (enemyData == null)
        {
            enemyData = new EnemyData[] { newEnemyData };
            return;
        }

        for (int i = 0; i < enemyData.Length; i++)
        {
            if (enemyData[i].enemyId == newEnemyData.enemyId)
            {
                enemyData[i] = newEnemyData;
                return;
            }
        }
        Debug.Log("EnemyData: " +  newEnemyData.enemyId );

        EnemyData[] newArray = new EnemyData[enemyData.Length + 1];

        for (int i = 0; i < enemyData.Length; i++)
        {
            newArray[i] = enemyData[i];
        }

        newArray[newArray.Length - 1] = newEnemyData;
        enemyData = newArray;
    }

}
