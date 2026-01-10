using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    //public static MainMenuManager Instance;
    public string mainGameSceneName = "Level 2";
    public string loadingSceneName = "Loading Screen";
    public string mainMenuSceneName = "Main Menu";


    private LoadingScreenManager currentLoadingScreenManager;
    private string pendingSceneName;
    private bool isLoadGame = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    [SerializeField]
    private GameObject mainMenu;
    public void NewGame()
    {
        Debug.Log("New Game started...");
        LoadTargetScene(mainGameSceneName);
        //GameDataManager.Instance.NewGame();
        //GameDataManager.Instance.gameData.levelName = mainGameSceneName;
        //SceneManager.LoadScene(mainGameSceneName);
    }

    public void LoadGame()
    {
        
        GameData gameData = GameDataManager.Instance.fileHandler.Load();
        Debug.Log(gameData.levelName);
        if (gameData != null)
        {
            Debug.Log("Loading Game");
            isLoadGame = true;
            LoadTargetScene(gameData.levelName);
            //GameDataManager.Instance.LoadGame();
        }
        else
        {
            Debug.Log("No save found");
        }
    }

    public void LoadTargetScene(string targetSceneName)
    {
        Debug.Log($"Requesting load sequence for scene: {targetSceneName}");
        pendingSceneName = targetSceneName;
        StartCoroutine(LoadSceneSequence(targetSceneName));
    }
    public void Quit()
    {
        // Quits the application when run as a build
        Application.Quit();

        // Special command to stop the game when running inside the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        // Tell the Settings menu: "I am coming from the Main Menu"
        SettingsMenu.previousSceneName = "Main Menu";

        SceneManager.LoadScene("Settings");
    }
    private IEnumerator LoadSceneSequence(string targetSceneName)
    {
        // Load loading screen additively
        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));
        currentLoadingScreenManager = FindFirstObjectByType<LoadingScreenManager>();
        if (currentLoadingScreenManager == null)
        {
            Debug.Log("LoadingScreenManager not found!");
            yield break;
        }

        // Start async load of target scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false;

        float minLoadTime = 2f;
        float timer = 0f;

        while (!operation.isDone)
        {

            // Update fake progress (Unity load stops at 0.9)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            currentLoadingScreenManager.UpdateProgress(progress);

            timer += Time.deltaTime;

            // Conditions to activate the scene:
            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                // Ensure progress bar is full
                currentLoadingScreenManager.UpdateProgress(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Unload loading scene
        //yield return SceneManager.UnloadSceneAsync(loadingSceneName);
        Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
        if (loadingScene.IsValid() && loadingScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(loadingScene);
            Debug.Log("Loading scene unloaded successfully");
        }
        else
        {
            Debug.LogWarning("Loading scene is not valid or not loaded, cannot unload");
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded called: " + scene.name);
        Debug.Log("Pending scene: " + pendingSceneName);

        if (scene.name != pendingSceneName)
        {
            Debug.Log("Scene does not match pendingSceneName, skipping");
            return;
        }

        Debug.Log("Scene matches pendingSceneName, starting coroutine");
        StartCoroutine(ApplyGameDataAfterStart());
    }


    private IEnumerator ApplyGameDataAfterStart()
    {
        // Wait one frame so Start() runs on all objects
        yield return null;

        if (isLoadGame)
        {
            Debug.Log("load Game");
            GameDataManager.Instance.LoadGame();
            Debug.Log(GameDataManager.Instance.gameData.soundMeterLevel);
        }
        else
        {
            GameDataManager.Instance.NewGame();
            GameDataManager.Instance.gameData.levelName = pendingSceneName;
        }

        pendingSceneName = null;
    }


}
