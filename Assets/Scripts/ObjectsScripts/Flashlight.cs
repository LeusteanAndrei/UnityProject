using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
public class Flashlight : MonoBehaviour
{
    [SerializeField] private float loseTime;
    [SerializeField] private float currentTime;
    [SerializeField] private bool isPlayerInLight;
    [SerializeField] private Image DetectionMark;
    [SerializeField] private Image DetectionFillImage;
    [SerializeField] private Transform LightOrigin;

    [SerializeField] private LayerMask raycastMask;

    private GameObject player;
    private EnemyMovement enemyScript;
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
        player = GameObject.FindGameObjectWithTag("Player");
        enemyScript = transform.parent.GetComponent<EnemyMovement>();    
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
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().GameOver();
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
        if (other.gameObject.CompareTag("Player") && CanSeePLayer(player))
        {
            isPlayerInLight = true;
            if (enemyScript != null)
                enemyScript.GetDistracted(player);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && CanSeePLayer(player))
        {
            currentTime += Time.deltaTime;
            isPlayerInLight = true;
            if (enemyScript != null)
                enemyScript.GetDistracted(player);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInLight = false;
        }
    }


    private bool CanSeePLayer(GameObject player)
    {
        Vector3 target = player.transform.position;
        Vector3 direction = target - LightOrigin.position;
        float distance = direction.magnitude;

        Ray ray = new Ray(LightOrigin.position, direction.normalized);
        if (Physics.Raycast(ray, out RaycastHit hit, distance, raycastMask))
        {


            Color rayColor = hit.collider.CompareTag("Player") ? Color.green : Color.red;
            Debug.DrawRay(LightOrigin.position, direction.normalized * hit.distance, rayColor);
            return hit.collider.CompareTag("Player");
        }

        Debug.DrawRay(LightOrigin.position, direction.normalized * distance, Color.yellow);
        return true;
    }
}