using UnityEngine;
using UnityEngine.EventSystems; // Required for Hover detection
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // <--- 1. ADD THIS!

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Button buttonComponent;
    public Image levelImage;       // The main screenshot image
    public GameObject currentBorder; // The object you created in Step 1
    public GameObject hoverBorder;   // The object you created in Step 1
    public bool isCurrent = false;
    [Header("Settings")]
    public int levelIndex;         // e.g., 1, 2, 3...
    public string levelName;
    private LoadingScreenManager currentLoadingScreenManager;
    public string loadingSceneName = "Loading Screen";
    private string lastSceneLoaded;

    // We will call this from the Manager script
    public void Setup(bool isUnlocked, bool isCurrentLevel)
    {
        // 1. Handle Locking
        if (isUnlocked)
        {
            buttonComponent.interactable = true;
            // Reset color to white (normal) just in case
            levelImage.color = Color.white;
        }
        else
        {
            // This prevents clicking
            buttonComponent.interactable = false;

            // This visually greys it out (or use the Button's 'Disabled Color' setting)
            levelImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        // 2. Handle Current Level Border
        if (isCurrentLevel)
        {
            currentBorder.SetActive(true);
            isCurrent = true;
        }
        else
        {
            currentBorder.SetActive(false);
        }

        // Ensure hover border is off at start
        hoverBorder.SetActive(false);
    }

    // 3. Handle Hover Logic (Interface Implementation)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Only show hover effect if the button is actually unlocked!
        if (buttonComponent.interactable)
        {
            if (currentBorder==false)hoverBorder.SetActive(true);
            else
            {
                currentBorder.SetActive(false);
                hoverBorder.SetActive(true);
            }
            
        }
    }
    void Start()
    {
        // 3. AUTOMATICALLY LISTEN FOR CLICKS
        // This tells Unity: "When this button is clicked, run the LoadLevel function."
        // Because we set interactable = false for locked levels, this only works for unlocked ones.
        buttonComponent.onClick.AddListener(LoadLevel);
    }
    public void LoadTargetScene(string targetSceneName)
    {
        Debug.Log($"Requesting load sequence for scene: {targetSceneName}");
        StartCoroutine(LoadSceneSequence(targetSceneName));
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
    void LoadLevel()
    {
        // 4. LOAD THE SCENE
        Debug.Log("Loading Scene: " + levelName); // Helpful for debugging!
        //SceneManager.LoadScene(levelName);
        LoadTargetScene(levelName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isCurrent == true)
        {
            hoverBorder.SetActive(false);
            currentBorder.SetActive(true);
        }
        else hoverBorder.SetActive(false);


    }
}
