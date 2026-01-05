using UnityEngine;
public class Puddle : MonoBehaviour
{
    private WetMeterManager wetMeterManager;
    [SerializeField] private ParticleSystem splashEffect;
    [SerializeField] private float splashCooldown = 0.5f;
    [SerializeField] private float lastSplashTime = 0f;
    [SerializeField] private LayerMask puddleLayerMask = ~0; 
    [SerializeField] private float splashHeight = 0.5f;
    void Start()
    {
        wetMeterManager = FindObjectOfType<WetMeterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wetMeterManager.inPuddle = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        // Optional: If you want to ensure the wet meter increase remains while inside the puddle
        if (other.CompareTag("Player") && Time.time - lastSplashTime >= splashCooldown)
        {
            // Spawn splash only if player is moving a bit
            Rigidbody rb = other.attachedRigidbody;
            float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
            if(speed > 0.1f)
            {
                if(splashEffect != null)
                {
                    
                    Instantiate(splashEffect.gameObject, new Vector3(other.transform.position.x,transform.position.y + splashHeight, other.transform.position.z), Quaternion.identity);
                    lastSplashTime = Time.time;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wetMeterManager.inPuddle = false;
        }
    }
}
