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
        SceneManager.LoadScene("Main Menu");
    }

    void InitializeLevels()
    {

        int reachedLevel = GameDataManager.Instance.gameData.completedLevels.Length + 1 ;
        string currentLevel = GameDataManager.Instance.gameData.levelName;
        string[] unlockedLevels = GameDataManager.Instance.gameData.completedLevels;



        for (int i = 0; i < levelButtons.Count; i++)
        {
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
            levelButtons[i].Setup(isUnlocked, isCurrent);
        }
    }
}