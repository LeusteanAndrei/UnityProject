using UnityEngine;
using UnityEngine.UI;

public class SaveMeterScript : MonoBehaviour, ISaveGame
{

    private SoundMeterManage soundMeterManage;
    void Start()
    {
        soundMeterManage  = GetComponent<SoundMeterManage>();
    }

    void Update()
    {
    }


    public void LoadData(GameData data)
    {
        soundMeterManage.SetCurrentSoundLevel(data.soundMeterLevel);
        Debug.Log("Loaded slider value with : " + data.soundMeterLevel);
    }
    public void SaveData(GameData data)
    {
        data.soundMeterLevel = soundMeterManage.GetCurrentSoundLevel();
    }
}
