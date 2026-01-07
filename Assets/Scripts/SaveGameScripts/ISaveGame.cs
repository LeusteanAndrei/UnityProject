using UnityEngine;

public interface ISaveGame
{

    void LoadData(GameData data);
    void SaveData(GameData data);
}
