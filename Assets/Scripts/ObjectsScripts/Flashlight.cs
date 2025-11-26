using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private float loseTime;
    [SerializeField] private float currentTime;
    [SerializeField] private bool isPlayerInLight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime>loseTime)
        {
            Debug.Log("GameOver");
        }
        if(!isPlayerInLight && currentTime>0f)
        {
            currentTime = currentTime - Time.deltaTime/2f;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            isPlayerInLight = true;
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            currentTime += Time.deltaTime;
            isPlayerInLight = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInLight = false;
        }
    }
}