using UnityEngine;

public class WeepingAngel : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float rotateSpeed = 2f;
    [SerializeField] private Transform originalOrientation;
    private bool OnCameraView = false;
    private Renderer Renderer;
    [SerializeField] private Collisions collisions;
    void Start()
    {
        Renderer = GetComponent<Renderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        originalOrientation = transform;
    }
    void Update()
    {
        if (Renderer.isVisible)
        {
            OnCameraView = true;
        }
        else
        {
            OnCameraView = false;
        }

        if (OnCameraView && !collisions.IsDestroyed())
        {
            Vector3 dir = originalOrientation.position - transform.position;
            dir.y = 0f; // constrain to horizontal plane
            if (dir.sqrMagnitude < 0.0001f)
            {
                // fallback to forward to avoid zero-vector look rotation
                dir = new Vector3(originalOrientation.forward.x, 0f, originalOrientation.forward.z);
            }
            float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float currentYaw = transform.rotation.eulerAngles.y;
            float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }
        else if (!OnCameraView && !collisions.IsDestroyed())
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f; // constrain to horizontal plane
            if (dir.sqrMagnitude < 0.0001f)
            {
                dir = new Vector3(transform.forward.x, 0f, transform.forward.z);
            }
            float targetYaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            float currentYaw = transform.rotation.eulerAngles.y;
            float newYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);
        }
    }
}
