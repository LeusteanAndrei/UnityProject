using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CapsuleCollider))]
public class LaserBeam : MonoBehaviour
{
    [SerializeField] private Transform endpointA;
    [SerializeField] private Transform endpointB;
    [SerializeField] private float thickness = 0.1f;
    private LineRenderer lr;
    private CapsuleCollider capsule;
    private GameManager gameManager;

    public void Initialize(Transform a, Transform b, float laserThickness = 0.1f)
    {
        endpointA = a;
        endpointB = b;
        thickness = laserThickness;
        Setup();
        UpdateGeometry();
    }

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        capsule = GetComponent<CapsuleCollider>();
        gameManager = GetComponent<GameManager>();
        Setup();
    }

    private void Setup()
    {
        if (lr == null) lr = GetComponent<LineRenderer>();
        if (capsule == null) capsule = GetComponent<CapsuleCollider>();

        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.startWidth = thickness;
        lr.endWidth = thickness;

        capsule.isTrigger = true;
        capsule.direction = 2;
        capsule.radius = thickness * 0.5f;
        capsule.center = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (endpointA == null || endpointB == null) return;
        UpdateGeometry();
    }

    private void UpdateGeometry()
    {
        Vector3 a = endpointA.position;
        Vector3 b = endpointB.position;
        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        Vector3 mid = (a + b) * 0.5f;
        Vector3 dir = (b - a);
        float dist = dir.magnitude;
        if (dist < 0.0001f)
        {
            capsule.height = thickness;
            transform.position = mid;
            return;
        }

        transform.position = mid;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        capsule.radius = thickness * 0.5f;
        capsule.height = Mathf.Max(dist, thickness);
    }

    private void OnTriggerEnter(Collider other)
    {
        gameManager.GameOver();
    }

    private void OnTriggerStay(Collider other)
    {

    }

    
}
