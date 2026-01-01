using UnityEngine;

public class ModifyWet : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private GameObject dropletEffect;
    [SerializeField] private WetMeterManager wetMeterManager;
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
            Debug.Log("Trigger entered by " + other.name);
            if (wetMeterManager != null)
            {
                if (type == "Campfire")
                wetMeterManager.campfireNearby = true;
                else if (type == "Lake"){
                wetMeterManager.inLake = true;
                Debug.Log("Player entered lake");
                if(dropletEffect != null){
                    Instantiate(dropletEffect, other.transform.position, Quaternion.identity);
                }
                }
                else if (type == "Weather")
                wetMeterManager.underWeather = true;
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