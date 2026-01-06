using UnityEngine;

public class SavePlayerScript : MonoBehaviour, ISaveGame
{

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadData(GameData data)
    {
        if (data.playerPosition.Length == 3 )
        {
            transform.position = new Vector3(data.playerPosition[0], data.playerPosition[1], data.playerPosition[2]);
            Debug.Log("Loaded player position");

        }
    }
    public void SaveData(GameData data)
    {
        data.playerPosition = new float[3];
        data.playerPosition[0] = transform.position.x;
        data.playerPosition[1] = transform.position.y;
        data.playerPosition[2] = transform.position.z;
    }
}
