using UnityEngine;
using UnityEngine.EventSystems; // Required for Hover detection
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections; // <--- 1. ADD THIS!

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Button buttonComponent;
    public Image levelImage;       
    public GameObject currentBorder; 
    public GameObject hoverBorder;  
    public bool isCurrent = false;
    [Header("Settings")]
    public int levelIndex;        
    public string levelName;
    private LoadingScreenManager currentLoadingScreenManager;
    public string loadingSceneName = "Loading Screen";
    private string lastSceneLoaded;

    public void Setup(bool isUnlocked, bool isCurrentLevel)
    {
        // 1. Handle Locking
        if (isUnlocked)
        {
            buttonComponent.interactable = true;
            levelImage.color = Color.white;
        }
        else
        {
            buttonComponent.interactable = false;

            // This visually greys it out 
            levelImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        }

        if (isCurrentLevel)
        {
            currentBorder.SetActive(true);
            isCurrent = true;
        }
        else
        {
            currentBorder.SetActive(false);
        }

        hoverBorder.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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
        buttonComponent.onClick.AddListener(LoadLevel);
    }
    //public void LoadTargetScene(string targetSceneName)
    //{
    //    Debug.Log($"Requesting load sequence for scene: {targetSceneName}");
    //    StartCoroutine(LoadSceneSequence(targetSceneName));
    //}
    //private IEnumerator LoadSceneSequence(string targetSceneName)
    //{
    //    // Load the loading screen additively
    //    yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(loadingSceneName));

    //    currentLoadingScreenManager = FindFirstObjectByType<LoadingScreenManager>();
    //    if (currentLoadingScreenManager == null)
    //    {
    //        Debug.LogError("LoadingScreenManager not found!");
    //        yield break;
    //    }

    //    // Begin loading the target scene
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(targetSceneName);
    //    operation.allowSceneActivation = false;
    //    lastSceneLoaded = targetSceneName;

    //    float minLoadTime = 2f;
    //    float timer = 0f;

    //    while (!operation.isDone)
    //    {
    //        float progress = Mathf.Clamp01(operation.progress / 0.9f);
    //        currentLoadingScreenManager.UpdateProgress(progress);

    //        timer += Time.deltaTime;

    //        if (operation.progress >= 0.9f && timer >= minLoadTime)
    //        {
    //            currentLoadingScreenManager.UpdateProgress(1f);
    //            operation.allowSceneActivation = true;
    //        }

    //        yield return null;
    //    }

    //    // WAIT 1 FRAME for activation
    //    yield return null;

    //    // Make the target scene active
    //    SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));

    //    // Unload loading screen
    //    yield return SceneManager.UnloadSceneAsync(loadingSceneName);
    //}
 
    
    void LoadLevel()
    {
        Debug.Log("Loading Scene: " + levelName);
        if (isCurrent)
        {
            Debug.Log("Loading the current scene saved in the manager");
            MainMenuManager.Instance.LoadGame(true);
        }
        else
        {
            Debug.Log("Loading another completely new scene");
            MainMenuManager.Instance.LoadGame(false, levelName);
        }
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
