using UnityEngine;

[System.Serializable]
public class GameData 
{
    public string levelName;
    public float soundMeterLevel;

    public GameData()
    {
        soundMeterLevel = 0;
        levelName = "Level 1";
    }
    public GameData(float soundMeterLevel)
    {
        this.soundMeterLevel = soundMeterLevel;
    }
}
