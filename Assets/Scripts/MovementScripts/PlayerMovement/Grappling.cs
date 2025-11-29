using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Grappling : MonoBehaviour
{
    [SerializeField] private string grappleTag = "GrapplePoint";
    [SerializeField] private float maxGrappleDistance = 25f;
    [SerializeField] private float ropeExtendSpeed = 60f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private Transform ropeOrigin;
    [SerializeField] private LayerMask lineOfSightMask = ~0;
    [SerializeField] private float grappleCooldown = 1.0f;
    [SerializeField] private float maxPullDuration = 3f;
    [SerializeField] private float minPullAcceleration = 10f;
    [SerializeField] private float maxPullAcceleration = 60f;
    [SerializeField] private Transform bestGrapplePoint;

    // Simple joint settings (no gravity/options)
    [SerializeField] private float jointSpring = 250f;
    [SerializeField] private float jointDamper = 20f;
    [SerializeField] private float jointShrinkSpeed = 20f;
    [SerializeField] private float horizontalImpulseFactor = 0.5f; // reduce horizontal component of per-tick impulses

    private Rigidbody rb;
    private LineRenderer line;

    private bool isFiring;
    private bool isPulling;
    private Vector3 targetPoint;
    private float ropeTravel;
    private float ropeTotalDistance;
    private float lastGrappleTime = -1000f;
    private SpringJoint activeJoint;
    private InputAction jumpAction;
    void Start()
    {
        EnsureInit();
        jumpAction = InputSystem.actions.FindAction("Jump");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFiring)
        {
            UpdateRopeExtend();
        }
        else if (isPulling)
        {
            UpdatePull();
        }
        if(Physics.Raycast(transform.position, targetPoint - transform.position, out RaycastHit hitInfo, Vector3.Distance(transform.position, targetPoint), lineOfSightMask, QueryTriggerInteraction.Ignore))
        {
            if(!IsSameOrChild(hitInfo.transform, ropeOrigin))
            {
                StopGrapple();
            }
        }
        if(Vector3.Distance(transform.position, targetPoint) < stopDistance || (isPulling && Time.time > lastGrappleTime + maxPullDuration) || (jumpAction.WasPressedThisFrame() && isPulling))
        {
            StopGrapple();
        }
        Transform currentBest = bestGrapplePoint;
        bestGrapplePoint = FindBestGrappleTarget();
        if (currentBest != bestGrapplePoint)
        {
            if (currentBest != null)
            {
                currentBest.gameObject.GetComponent<GrapplePoint>().isActive = false;
            }
        }
        if (bestGrapplePoint != null)
        {
            bestGrapplePoint.gameObject.GetComponent<GrapplePoint>().isActive = true;
        }
    }
    public void AttemptGrapple(InputAction.CallbackContext context)
    {
        if (!(context.started || context.performed)) return;

        if (Time.time < lastGrappleTime + grappleCooldown) return;
        if (isFiring || isPulling) return;

        EnsureInit();

        if (bestGrapplePoint == null) return;

        targetPoint = bestGrapplePoint.position;
        StartFiring();
        lastGrappleTime = Time.time;
    }



    private void EnsureInit()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (line == null)
        {
            line = GetComponent<LineRenderer>();
            if (line == null)
            {
                line = gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.startColor = Color.brown;
                line.endColor = Color.brown;
                line.startWidth = 0.05f;
                line.endWidth = 0.05f;
                line.positionCount = 0;
            }
        }
        if (ropeOrigin == null) ropeOrigin = transform;
    }
    public void StartFiring()
    {
        EnsureInit();
        isFiring = true;
        ropeTravel = 0f;
        ropeTotalDistance = Vector3.Distance(ropeOrigin.position, targetPoint);
        line.positionCount = 2;
    }
    private Transform FindBestGrappleTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxGrappleDistance);
        if (hits == null || hits.Length == 0) return null;

        float bestDist = maxGrappleDistance + 1f;
        Transform best = null;
        for (int i = 0; i < hits.Length; i++)
        {
            var c = hits[i];
            if (c == null) continue;
            if (!c.CompareTag(grappleTag)) continue;

            Vector3 origin = ropeOrigin != null ? ropeOrigin.position : transform.position;
            Vector3 toCandidate = c.transform.position - origin;
            float distFromOrigin = toCandidate.magnitude;
            if (distFromOrigin > maxGrappleDistance) continue;

            if (Physics.Raycast(origin, toCandidate.normalized, out RaycastHit hit, distFromOrigin, lineOfSightMask, QueryTriggerInteraction.Ignore))
            {
                if (!IsSameOrChild(hit.transform, c.transform))
                    continue;
            }

            float distToPlayer = Vector3.Distance(transform.position, c.transform.position);
            if (distToPlayer < bestDist)
            {
                bestDist = distToPlayer;
                best = c.transform;
            }
        }
        return best;
    }

    private bool IsSameOrChild(Transform a, Transform b)
    {
        Transform t = a;
        while (t != null)
        {
            if (t == b) return true;
            t = t.parent;
        }
        return false;
    }


    private void UpdateRopeExtend()
    {
        Vector3 origin = ropeOrigin.position;
        Vector3 dir = (targetPoint - origin);
        float total = Mathf.Max(0.0001f, ropeTotalDistance);
        ropeTravel += ropeExtendSpeed * Time.deltaTime;
        float current = Mathf.Min(ropeTravel, total);
        Vector3 tip = origin + dir.normalized * current;

        line.SetPosition(0, origin);
        line.SetPosition(1, tip);

        if (current >= total - 0.001f)
        {
            isFiring = false;
            BeginJointPull();
        }
    }

    private void UpdatePull()
    {
        if (rb == null)
        {
            StopGrapple();
            return;
        }
        Vector3 origin = ropeOrigin.position;
        Vector3 toTarget = targetPoint - transform.position;
        float dist = toTarget.magnitude;
        if (dist <= stopDistance)
        {
            StopGrapple();
            return;
        }
        if (activeJoint != null)
        {
            // Monotonic shortening: if current distance is smaller than joint length, update joint length
            float currentDist = dist;
            if (currentDist + 0.0001f < activeJoint.maxDistance)
            {
                activeJoint.maxDistance = Mathf.Max(stopDistance, currentDist);
            }

            // Assist with a small impulse toward target
            Vector3 dir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector3.zero;
            float ratio = Mathf.Clamp01(dist / Mathf.Max(0.0001f, maxGrappleDistance));
            float impulse = Mathf.Lerp(minPullAcceleration, maxPullAcceleration, ratio) * 0.02f; // small per-tick impulse
            // Reduce horizontal component to bias upward pull
            Vector3 impulseDir = new Vector3(dir.x * horizontalImpulseFactor, dir.y, dir.z * horizontalImpulseFactor).normalized;
            rb.AddForce(impulseDir * impulse, ForceMode.Impulse);
        }

        line.SetPosition(0, origin);
        line.SetPosition(1, targetPoint);
    }

    private void StopGrapple()
    {
        isFiring = false;
        isPulling = false;
        if (line != null) line.positionCount = 0;
        if (activeJoint != null)
        {
            Destroy(activeJoint);
            activeJoint = null;
        }
    }

    public bool IsPulling()
    {
        return isPulling;
    }

    private void BeginJointPull()
    {
        isPulling = true;
        if (activeJoint != null)
        {
            Destroy(activeJoint);
            activeJoint = null;
        }
        activeJoint = gameObject.AddComponent<SpringJoint>();
        activeJoint.autoConfigureConnectedAnchor = false;
        activeJoint.connectedBody = null; // world space anchor
        activeJoint.connectedAnchor = targetPoint;
        float dist = Vector3.Distance(transform.position, targetPoint);
        activeJoint.maxDistance = dist;
        activeJoint.minDistance = 0f;
        activeJoint.spring = jointSpring;
        activeJoint.damper = jointDamper;
        activeJoint.tolerance = 0.01f;
        activeJoint.enableCollision = false;
    }
}
