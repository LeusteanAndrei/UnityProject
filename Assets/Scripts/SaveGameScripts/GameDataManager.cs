using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameDataManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;


    public GameData gameData;
    public static GameDataManager Instance { get; private set; }

    public List<ISaveGame> saveObjects;
    public FileHandler fileHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Debug.Log(Application.persistentDataPath);
        this.fileHandler = new FileHandler(Application.persistentDataPath, fileName);
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
       && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Ctrl+S pressed! Saving data...");
            SaveGame();
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
        saveObjects = FindAllSaveObjects();
        SaveGame();
    }

    public void LoadGame()
    {
        this.gameData = fileHandler.Load();
        if (this.gameData == null)
        {
            Debug.Log("No game data found! - Load Game");
            NewGame();
            return;
        }


        //GameManager.Instance.LoadTargetScene(gameData.levelName);
        saveObjects = FindAllSaveObjects();
        for (int i = 0; i < saveObjects.Count; i++)
        {
            saveObjects[i].LoadData(gameData);
        }

    }

    public void SaveGame()
    {
        saveObjects = FindAllSaveObjects();
        for (int i = 0; i < saveObjects.Count; i++)
        {
            saveObjects[i].SaveData(gameData);
        }

        fileHandler.Save(this.gameData);

    }

    private List<ISaveGame> FindAllSaveObjects()
    {
        IEnumerable<ISaveGame> saveGameObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveGame>();

        return new List<ISaveGame>(saveGameObjects);  
    }

}
