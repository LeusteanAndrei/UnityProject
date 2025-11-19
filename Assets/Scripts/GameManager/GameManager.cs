using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject mainMenu;

    public string mainGameSceneName = "Level 2";
    public string loadingSceneName = "Loading Screen";
    private LoadingScreenManager currentLoadingScreenManager;
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
    public void LoadTargetScene(string targetSceneName)
    {
        Debug.Log($"Requesting load sequence for scene: {targetSceneName}");
        StartCoroutine(LoadSceneSequence(targetSceneName));
    }
    private void Update()
    {
        //I'll put the pause Menu here when it's done
    }
    public void NewGame()
    {
        Debug.Log("New Game started...");
        LoadTargetScene(mainGameSceneName);
        //SceneManager.LoadScene(mainGameSceneName);
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
        yield return SceneManager.UnloadSceneAsync(loadingSceneName);
    }
}

