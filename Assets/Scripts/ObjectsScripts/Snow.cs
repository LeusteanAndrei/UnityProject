using UnityEngine;

public class Snow : MonoBehaviour
{
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private float printCooldown = 0.5f;
    [SerializeField] private float lastPrintTime = 0f;
    [SerializeField] private float rotationOffsetDegrees = 0f; // yaw offset to tweak orientation
    [SerializeField] private float printOffsetX=0f;
    private bool lastleftFoot = false;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        printCooldown = Mathf.Clamp(0.5f - rb.linearVelocity.magnitude * 0.05f, 0.3f, 1f);
    }
    void OnCollisionStay(Collision collision)
    {
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
                    Vector3 facing = other.transform.forward;
                    Vector3 dir = new Vector3(facing.x, 0f, facing.z);
                    if (dir.sqrMagnitude < 0.0001f)
                    {
                        Vector3 v = rb != null ? rb.linearVelocity : Vector3.zero;
                        dir = new Vector3(v.x, 0f, v.z);
                    }
                    // Explicit yaw from facing/velocity on XZ, then set X=90°, Z=0°
                    float yawDeg = dir.sqrMagnitude > 0.0001f
                        ? Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + rotationOffsetDegrees
                        : rotationOffsetDegrees;
                    Quaternion finalRot = Quaternion.Euler(90f, yawDeg, 0f);
                    if (lastleftFoot)
                    {
                        contactPos += other.transform.right * printOffsetX;
                    }
                    else
                    {
                        contactPos -= other.transform.right * printOffsetX;
                    }
                    lastleftFoot = !lastleftFoot;
                    GameObject snowPrint = Instantiate(snowEffect.gameObject, contactPos, finalRot);
                    lastPrintTime = Time.time;
                }
            }
        }
    }
}
