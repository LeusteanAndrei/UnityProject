using UnityEngine;

public class menuControl : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenLevelMenu()
    {
        MainMenuManager.Instance.OpenLevelMenu();
    }

    public void QuitGame()
    {
        MainMenuManager.Instance.Quit();
    }
    public void Settings()
    {
        MainMenuManager.Instance.OpenSettings();
    }

    public void LoadGame(bool fromSaveFile)
    {
        MainMenuManager.Instance.LoadGame(fromSaveFile);
    }

    public void NewGame()
    {
        MainMenuManager.Instance.NewGame();
    }
}
