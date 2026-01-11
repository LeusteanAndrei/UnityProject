using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevelScript : MonoBehaviour
{

    public float loadTime = 5f;
    public string stageToLoad = "Level 1";

    private float timer = 0;


    void Start()
    {
        
    }

    void Update()
    {
        if (timer >=loadTime)
        {
            FinishedLevel();
        }
        FillShader();
    }



    void FinishedLevel()
    {

        GameManager.Instance.NextLevelName = stageToLoad;
        GameManager.Instance.ShowFinalScreen();
        Debug.Log("Finished " + SceneManager.GetActiveScene().name);

        if (GameDataManager.Instance.loadedFromSaveFile == true)
        {
            GameDataManager.Instance.gameData.resetValues();
            GameDataManager.Instance.gameData.levelName = stageToLoad;
            GameDataManager.Instance.gameData.AddCompletedLevel(SceneManager.GetActiveScene().name);

            GameDataManager.Instance.fileHandler.Save(GameDataManager.Instance.gameData);
        }
    }

    void FillShader()
    {
        float timeNormal = timer / loadTime;
        float fillValue = timeNormal * 2 - 1 - 0.1f;
        GetComponent<Renderer>().material.SetFloat("_fillValue", fillValue);

    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            timer = 0;
        }
    }


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timer += Time.deltaTime;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            timer = 0;
        }
    }
}
