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
            LoadScene(stageToLoad);
        }
    }

    void LoadScene(string stageToLoad)
    {
        SceneManager.LoadScene(stageToLoad);
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
