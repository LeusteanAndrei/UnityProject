using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Grappling : MonoBehaviour
{
    [SerializeField] private string grappleTag = "GrapplePoint";
    [SerializeField] private float maxGrappleDistance = 25f;
    [SerializeField] private float ropeExtendSpeed = 60f;
    [SerializeField] private float pullAcceleration = 40f;
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private Transform ropeOrigin;
    [SerializeField] private LayerMask lineOfSightMask = ~0;
    [SerializeField] private float grappleCooldown = 1.0f;
    [SerializeField] private float pullDuration = 0.3f;
    [SerializeField] private float minPullAcceleration = 10f;
    [SerializeField] private float maxPullAcceleration = 60f;
    [SerializeField] private Transform bestGrapplePoint;

    private Rigidbody rb;
    private LineRenderer line;

    private bool isFiring;
    private bool isPulling;
    private Vector3 targetPoint;
    private float ropeTravel;
    private float ropeTotalDistance;
    private float lastGrappleTime = -1000f;
    private Coroutine pullRoutine;

    // Update is called once per frame
    void Update()
    {
        if (isFiring)
        {
            UpdateRopeExtend();
        }
        else if (isPulling)
        {
            UpdatePull();
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

        EnsureInit();

        if (bestGrapplePoint == null) return;

        targetPoint = bestGrapplePoint.position;

        if (pullRoutine != null) { StopCoroutine(pullRoutine); pullRoutine = null; }
        pullRoutine = StartCoroutine(GrapplePullRoutine(targetPoint));
        lastGrappleTime = Time.time;
    }

    private void Start()
    {
        EnsureInit();
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
            isPulling = true;
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

        Vector3 dir = toTarget / Mathf.Max(0.0001f, dist);
        rb.AddForce(dir * pullAcceleration, ForceMode.Acceleration);

        line.SetPosition(0, origin);
        line.SetPosition(1, targetPoint);
    }

    private void StopGrapple()
    {
        isFiring = false;
        isPulling = false;
        if (line != null) line.positionCount = 0;
    }
    private IEnumerator GrapplePullRoutine(Vector3 target)
    {
        EnsureInit();
        isFiring = false;
        isPulling = true;

        if (line != null)
        {
            line.positionCount = 2;
        }

        float elapsed = 0f;
        while (elapsed < pullDuration)
        {
            if (rb == null) break;

            Vector3 origin = ropeOrigin.position;
            Vector3 toTarget = target - transform.position;
            float dist = toTarget.magnitude;
            if (dist <= stopDistance) break;

            float t = Mathf.Clamp01(dist / Mathf.Max(0.0001f, maxGrappleDistance));
            float acc = Mathf.Lerp(minPullAcceleration, maxPullAcceleration, t);
            Vector3 dir = dist > 0.0001f ? toTarget / dist : Vector3.zero;
            rb.AddForce(dir * acc, ForceMode.Acceleration);

            if (line != null)
            {
                line.SetPosition(0, origin);
                line.SetPosition(1, target);
            }

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isPulling = false;
        if (line != null) line.positionCount = 0;
        pullRoutine = null;
    }
}
