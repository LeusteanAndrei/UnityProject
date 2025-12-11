using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

public class EnemyAnimator : MonoBehaviour
{
    // Drag the NavMeshAgent component from this object here
    public NavMeshAgent agent;

    // Drag the Child Object's Animator (the Policewoman) here
    public Animator anim;

    void Update()
    {
        // Get the speed the agent is actually moving at
        float currentSpeed = agent.velocity.magnitude;

        // Pass that speed to the Animator
        // The animations will switch automatically based on your transitions!
        anim.SetFloat("Speed", currentSpeed);
    }
}