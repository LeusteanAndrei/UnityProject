
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //[SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private Canvas gameCanvas;

    public string mainGameSceneName = "Level 2";
    public string loadingSceneName = "Loading Screen";
    public string mainMenuSceneName = "Main Menu";
    private string lastSceneLoaded;


    private LoadingScreenManager currentLoadingScreenManager;

    private bool isPaused = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed");
            TogglePause();
        }
    }
    public void LoadTargetScene(string targetSceneName)
    {
        Debug.Log($"Requesting load sequence for scene: {targetSceneName}");
        StartCoroutine(LoadSceneSequence(targetSceneName));
    }
    private void TogglePause()
    {
        isPaused = !isPaused;

        pauseMenu.SetActive(isPaused);

        // Freeze or unfreeze gameplay
        Time.timeScale = isPaused ? 0 : 1;

        if (isPaused)
        {
            // Allow clicking UI
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameCanvas.enabled  = false;
        }
        else
        {
            // Restore gameplay mode (camera scripts expect this)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            gameCanvas.enabled = true;
        }
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        gameCanvas.enabled = true;

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Restart()
    {
        //Time.timeScale = 1; // Reset in case paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NewGame()
    {
        Debug.Log("New Game started...");
        LoadTargetScene(mainGameSceneName);
    }

    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        LoadTargetScene(mainMenuSceneName);
    }

    public void OpenSettings()
    {
        // Tell the Settings menu: "I am coming from the Game"
        SettingsMenu.previousSceneName = SceneManager.GetActiveScene().name;

        // IMPORTANT: If you want to keep your game progress, 
        // you should load the settings "Additively" so the game keeps running in the background.
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public static void GameOver()
    {
        Instance.gameOverCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //gameCanvas.enabled = false;
    }

    private IEnumerator LoadSceneSequence(string targetSceneName)
    {
        // Load the loading screen additively
        yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));

        currentLoadingScreenManager = FindFirstObjectByType<LoadingScreenManager>();
        if (currentLoadingScreenManager == null)
        {
            Debug.LogError("LoadingScreenManager not found!");
            yield break;
        }

        // Begin loading the target scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
        operation.allowSceneActivation = false;
        lastSceneLoaded = targetSceneName;

        float minLoadTime = 2f;
        float timer = 0f;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            currentLoadingScreenManager.UpdateProgress(progress);

            timer += Time.deltaTime;

            if (operation.progress >= 0.9f && timer >= minLoadTime)
            {
                currentLoadingScreenManager.UpdateProgress(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // WAIT 1 FRAME for activation
        yield return null;

        // Make the target scene active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));

        // Unload loading screen
        yield return SceneManager.UnloadSceneAsync(loadingSceneName);
    }
}



