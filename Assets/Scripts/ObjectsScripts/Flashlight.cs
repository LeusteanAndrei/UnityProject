using UnityEngine;
using UnityEngine.UI;
public class Flashlight : MonoBehaviour
{
    [SerializeField] private float loseTime;
    [SerializeField] private float currentTime;
    [SerializeField] private bool isPlayerInLight;
    [SerializeField] private Image DetectionMark;
    [SerializeField] private Image DetectionFillImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentTime = 0f;
        if (DetectionFillImage != null)
        {
            DetectionFillImage.fillAmount = 0f;
            DetectionFillImage.enabled = false;
        }
        if (DetectionMark != null)
        {
            DetectionMark.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(DetectionFillImage != null && DetectionMark != null && currentTime > 0f)
        {
            DetectionFillImage.enabled = true;
            DetectionMark.enabled = true;
        }
        else
        {
            if (DetectionFillImage != null)
            {
                DetectionFillImage.enabled = false;
            }
            if (DetectionMark != null)
            {
                DetectionMark.enabled = false;
            }
        }
        if(currentTime>loseTime)
        {
            Debug.Log("GameOver");
        }
        if(!isPlayerInLight && currentTime>0f)
        {
            currentTime = currentTime - Time.deltaTime/2f;
        }
        if (DetectionFillImage != null)
        {
            DetectionFillImage.fillAmount = currentTime / loseTime;
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