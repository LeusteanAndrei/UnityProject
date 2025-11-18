using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject mainMenu;

    public string mainGameSceneName = "Level 2";
    private void Start()
    {
        if(Instance == null)
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
        //I'll put the pause Menu here when it's done
    }
    public void NewGame()
    {
        SceneManager.LoadScene(mainGameSceneName);
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
}
