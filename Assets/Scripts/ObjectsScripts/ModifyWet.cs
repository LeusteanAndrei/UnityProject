using UnityEngine;

public class ModifyWet : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private GameObject dropletEffect;
    [SerializeField] private WetMeterManager wetMeterManager;
    [SerializeField] private float duration;
    void Start()
    {
        if(wetMeterManager == null)
        {
            wetMeterManager = GameObject.Find("GameManager").GetComponent<WetMeterManager>();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (wetMeterManager != null)
            {
                if (type == "Campfire")
                wetMeterManager.campfireNearby = true;
                else if (type == "Lake"){
                duration = 0f;
                Debug.Log("Player entered lake");
                if(dropletEffect != null){
                    Instantiate(dropletEffect, other.transform.position, Quaternion.identity);
                }
                }
                else if (type == "Weather")
                wetMeterManager.underWeather = true;
            }
        }
        else if(other.GetComponent<Fish>() != null && type == "Lake")
        {
            Fish fish = other.GetComponent<Fish>();
            fish.SetMarked();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && type == "Lake")
        {
            duration += Time.deltaTime;
            if (type == "Lake" && duration >= 0.75f)
            {
                if (wetMeterManager != null)
                {
                    wetMeterManager.inLake = true;
                }
                duration = 0f;
                if(dropletEffect != null){
                    Instantiate(dropletEffect, other.transform.position, Quaternion.identity);
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (wetMeterManager != null)
            {
                if (type == "Campfire")
                wetMeterManager.campfireNearby = false;
                else if (type == "Lake")
                wetMeterManager.inLake = false;
                else if (type == "Weather")
                wetMeterManager.underWeather = false;
            }
        }
    }
}