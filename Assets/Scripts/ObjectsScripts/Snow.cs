using UnityEngine;

public class Snow : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private float printCooldown = 0.5f;
    [SerializeField] private float lastPrintTime = 0f;
    [SerializeField] private float rotationOffsetDegrees = 0f; // yaw offset to tweak orientation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
        Collider other = collision.collider;
        if (collision.gameObject.CompareTag("Player") && Time.time - lastPrintTime >= printCooldown)
        {
            // Spawn splash only if player is moving a bit
            Rigidbody rb = other.attachedRigidbody;
            float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
            if(speed > 0.1f)
            {
                if(snowEffect != null)
                {
                    Vector3 contactPos = collision.GetContact(0).point + Vector3.up * 0.1f;
                    // Use player's facing from Movement component for orientation
                    Movement move = other.GetComponentInParent<Movement>();
                    Vector3 facing = (move != null ? move.transform.forward : other.transform.forward);
                    Vector3 dir = new Vector3(facing.x, 0f, facing.z);
                    if (dir.sqrMagnitude < 0.0001f)
                    {
                        // Fallback to velocity direction if facing is degenerate
                        Vector3 v = rb != null ? rb.linearVelocity : Vector3.zero;
                        dir = new Vector3(v.x, 0f, v.z);
                    }
                    Quaternion baseRot = dir.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(dir.normalized, Vector3.up) : Quaternion.identity;
                    Quaternion rot = Quaternion.AngleAxis(rotationOffsetDegrees, Vector3.up) * baseRot;

                    GameObject snowPrint = Instantiate(snowEffect.gameObject, contactPos, rot);
                    lastPrintTime = Time.time;
                }
            }
        }
    }
}
