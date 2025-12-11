using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class NewVersionMovement : MonoBehaviour
{
    [SerializeField] private int pathID;
    [SerializeField] private int currentPathPoint;
    [SerializeField] private GameObject CurrentTarget;
    [SerializeField] private List<PathPoint> pathPoints;
    [SerializeField] private string CurrentAction;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    // --- NEW VARIABLES FOR WAITING ---
    [SerializeField] private float waitDuration = 3.0f; // Time to wait in seconds
    [SerializeField] private float waitTimer;
    // ---------------------------------

    [SerializeField] private float distractedTimer;
    [SerializeField] private float distractedDuration = 2.5f;
    [SerializeField] private float rotationLerpDuration = 0.2f;
    [SerializeField] private float stunnedTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        pathPoints = new List<PathPoint>(FindObjectsOfType<PathPoint>().Where(point => point.pathID == pathID).OrderBy(point => point.pointOrderNumber));

        CurrentAction = "Move";
        currentPathPoint = 0;

        // Safety check to ensure we have points
        if (pathPoints.Count > 0)
        {
            CurrentTarget = pathPoints[currentPathPoint].gameObject;
            agent.SetDestination(CurrentTarget.transform.position);
        }
    }

    void Update()
    {
        UpdateAnimation();

        // 1. MOVEMENT LOGIC
        if (CurrentAction == "Move")
        {
            // Check if we reached the destination
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                // We arrived! Now check if we should wait.
                // We wait if we are at the First point (0) OR the Last point (Count - 1)
                if (currentPathPoint == 0 || currentPathPoint == pathPoints.Count - 1)
                {
                    StartWaiting();
                }
                else
                {
                    // If we are at a middle point, just keep walking
                    GoToNextPoint();
                }
            }
        }
        // 2. WAITING LOGIC (New)
        else if (CurrentAction == "Waiting")
        {
            waitTimer += Time.deltaTime;

            // Optional: If you want her to look around (rotate), put that logic here

            if (waitTimer >= waitDuration)
            {
                CurrentAction = "Move";
                GoToNextPoint();
            }
        }
        // 3. DISTRACTED LOGIC
        else if (CurrentAction == "Distracted")
        {
            agent.SetDestination(transform.position);
            distractedTimer += Time.deltaTime;

            if (CurrentTarget != null)
            {
                Vector3 toTarget = CurrentTarget.transform.position - transform.position;
                toTarget.y = 0f;
                if (toTarget.sqrMagnitude > 0.0001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(Time.deltaTime / rotationLerpDuration));
                }
            }

            if (distractedTimer >= distractedDuration)
            {
                distractedTimer = 0f;
                CurrentAction = "Move";
                // Resume movement to the current target
                CurrentTarget = pathPoints[currentPathPoint].gameObject;
                agent.SetDestination(CurrentTarget.transform.position);
            }
        }
        // 4. STUNNED LOGIC
        else if (CurrentAction == "Stunned")
        {
            agent.SetDestination(transform.position);
            stunnedTimer -= Time.deltaTime;
            if (stunnedTimer <= 0f)
            {
                CurrentAction = "Move";
                CurrentTarget = pathPoints[currentPathPoint].gameObject;
                agent.SetDestination(CurrentTarget.transform.position);
            }
        }
    }

    void StartWaiting()
    {
        CurrentAction = "Waiting";
        waitTimer = 0f;
        // Stop the agent so the animation switches to Idle
        agent.SetDestination(transform.position);
    }

    void GoToNextPoint()
    {
        currentPathPoint++;
        if (currentPathPoint >= pathPoints.Count)
        {
            currentPathPoint = 0;
        }
        CurrentTarget = pathPoints[currentPathPoint].gameObject;
        agent.SetDestination(CurrentTarget.transform.position);
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            float currentSpeed = agent.velocity.magnitude;
            animator.SetFloat("Speed", currentSpeed, 0.1f, Time.deltaTime);
        }
    }

    public void SetAction(string action)
    {
        CurrentAction = action;
    }

    public void GetDistracted(GameObject soundOrigin)
    {
        // Allow distraction even if she is waiting
        if (CurrentAction != "Distracted" && CurrentAction != "Stunned")
        {
            CurrentAction = "Distracted";
            distractedTimer = 0f;
            CurrentTarget = soundOrigin;
            agent.SetDestination(transform.position);
        }
    }

    public void GetStunned(float duration)
    {
        if (CurrentAction != "Stunned")
        {
            Debug.Log("Enemy Stunned for " + duration + " seconds.");
            CurrentAction = "Stunned";
            stunnedTimer = duration;
            agent.SetDestination(transform.position);
        }
    }
}