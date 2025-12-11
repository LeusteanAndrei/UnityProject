using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
public class OldEnemyMovement : MonoBehaviour
{
    [SerializeField] private int pathID;
    [SerializeField] private int currentPathPoint;
    [SerializeField] private GameObject CurrentTarget;
    [SerializeField] private List<PathPoint> pathPoints;
    [SerializeField] private string CurrentAction;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float distractedTimer;
    [SerializeField] private float distractedDuration = 2.5f;
    [SerializeField] private float rotationLerpDuration = 0.2f;
    [SerializeField] private float stunnedTimer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathPoints = new List<PathPoint>(FindObjectsOfType<PathPoint>().Where(point => point.pathID == pathID).OrderBy(point => point.pointOrderNumber));
        CurrentAction = "Move";
        currentPathPoint = 0;
        CurrentTarget = pathPoints[currentPathPoint].gameObject;
        agent.SetDestination(CurrentTarget.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        while (CurrentAction == "Move")
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                currentPathPoint++;
                if (currentPathPoint >= pathPoints.Count)
                {
                    currentPathPoint = 0;
                }
                CurrentTarget = pathPoints[currentPathPoint].gameObject;
                agent.SetDestination(CurrentTarget.transform.position);
            }
            //transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.identity,Mathf.Clamp01(Time.deltaTime / rotationLerpDuration));
            break;
        }
        if (CurrentAction == "Distracted")
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
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Mathf.Clamp01(Time.deltaTime / rotationLerpDuration)
                    );
                }
            }

            if (distractedTimer >= distractedDuration)
            {
                distractedTimer = 0f;
                CurrentAction = "Move";
                CurrentTarget = pathPoints[currentPathPoint].gameObject;
                agent.SetDestination(CurrentTarget.transform.position);
            }
        }
        if (CurrentAction == "Stunned")
        {
            agent.SetDestination(transform.position);
            stunnedTimer -= Time.deltaTime;
            if (stunnedTimer <= 0f)
            {
                stunnedTimer = 0f;
                CurrentAction = "Move";
                CurrentTarget = pathPoints[currentPathPoint].gameObject;
                agent.SetDestination(CurrentTarget.transform.position);
            }
        }
    }
    public void SetAction(string action)
    {
        CurrentAction = action;
    }
    public void GetDistracted(GameObject soundOrigin)
    {
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

