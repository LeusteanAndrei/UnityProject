using UnityEngine;

public class TriggerFence : MonoBehaviour
{
    [SerializeField] private Animator fence;
    [SerializeField] private WetMeterManager wetMeterManager;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && wetMeterManager.harshWeather)
        {
            if (fence != null)
            {
                fence.SetTrigger("ActivateFence");
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
