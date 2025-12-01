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
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wetMeterValue = 0f;
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 origin = player.position + originOffset;
            bool hit = Physics.Raycast(origin, Vector3.up, out RaycastHit hitInfo, raycastDistance, raycastMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(origin, Vector3.up * raycastDistance, hit ? Color.green : Color.red);
            if(hit)
            {
                wetMeterIncrease = 0f;
            }
            else
            {
                wetMeterIncrease = 1f; // Example increase rate when not obstructed
            }
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
