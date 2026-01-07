using UnityEngine;

public class WetMeterManager : MonoBehaviour
{
    [SerializeField] private float wetMeterValue;
    [SerializeField] private float maxWetMeterValue = 100f;
    [SerializeField] public float wetMeterIncrease = 0f;
    [Header("Upward Raycast Settings")]
    [SerializeField] private float raycastDistance = 5f;
    [SerializeField] private LayerMask raycastMask = ~0; 
    [SerializeField] private Vector3 originOffset = new Vector3(0f, 0.1f, 0f);
    [SerializeField] private Transform player;
    [SerializeField]private bool covered = false;
    public bool campfireNearby = false;
    public bool inLake = false;
    public bool underWeather = false;
    public bool inPuddle = false;
    public bool harshWeather = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wetMeterValue = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 origin = player.position + originOffset;
            bool hit = Physics.Raycast(origin, Vector3.up, out RaycastHit hitInfo, raycastDistance, raycastMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(origin, Vector3.up * raycastDistance, hit ? Color.green : Color.red);
            if(hit && hitInfo.collider.CompareTag("Weather"))
            {
                covered = false;
                underWeather = true;
            }
            else if(hit)
            {
                covered = true;
                underWeather = false;
            }
            else
            {
                covered = false;
                underWeather = false;
            }
        }
        wetMeterIncrease = 0f;
        if (covered)
        {
            wetMeterIncrease -= 2f;
        }
        else
        {
            wetMeterIncrease += 0f;
        }
        if (campfireNearby)
        {
            wetMeterIncrease -= 5f;
        }
        if (inLake)
        {
            wetMeterIncrease += 100f;
        }
        if (underWeather && !covered)
        {
            wetMeterIncrease += 3f;
            if (harshWeather)
            {
                wetMeterIncrease += 3f;
            }
        }
        if (inPuddle)
        {
            wetMeterIncrease += 1f;
        }
        wetMeterValue += wetMeterIncrease * Time.deltaTime;
        wetMeterValue = Mathf.Clamp(wetMeterValue, 0f, maxWetMeterValue);
        if (wetMeterValue >= maxWetMeterValue)
        {
            //game loss
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Vector3 origin = player.position + originOffset;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + Vector3.up * raycastDistance);
    }
}
