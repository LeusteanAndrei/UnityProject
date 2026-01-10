using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    // Drag all your buttons here in the Inspector, ordered 1 to 5
    public List<LevelButton> levelButtons;

    void Start()
    {
        InitializeLevels();

    }
    public void OnBack()
    {
        // Replace "MainMenu" with the EXACT name of your menu scene file
        SceneManager.LoadScene("Main Menu");
    }

    void InitializeLevels()
    {
        // GET DATA FROM YOUR SAVE SYSTEM HERE
        // For example: int reachedLevel = SaveSystem.GetReachedLevel();
        // Let's pretend the player is on Level 3 for this example:
        //string currentLevel = GameDataManager.Instance.gameData.levelName;
        //string[] unlockedLevels = GameDataManager.Instance.gameData.completedLevels;
        //Debug.Log(GameDataManager.Instance.gameData.levelName);
        int reachedLevel = 3;
        string currentLevel = "Level 3";
        string[] unlockedLevels = new string[] { "Level 1", "Level 2" };

        for (int i = 0; i < levelButtons.Count; i++)
        {
            // Level indices usually start at 1, but lists start at 0
            // So button[0] is Level 1.
            bool isUnlocked = false;
            bool isCurrent = false;
            int buttonLevelIndex = i + 1;
            if (currentLevel == levelButtons[i].levelName)
            {
                isUnlocked = true;
                isCurrent = true;
            }
                for (int j = 0; j < unlockedLevels.Length; j++)
            {
                if (unlockedLevels[j] == levelButtons[i].levelName)
                {
                    isUnlocked = true;
                    isCurrent = false;

                }
            }
            // Configure the specific button
            levelButtons[i].Setup(isUnlocked, isCurrent);
        }
    }
}