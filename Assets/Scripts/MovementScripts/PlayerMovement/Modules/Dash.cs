using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Dash : MonoBehaviour
{

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 30f; // the force with which the player dashes
    [SerializeField] private float dashDuration = 0.5f; // the duration of the dash
    [SerializeField] public float dashStaminaConsumption = 30f;


    private int dashCount; // number of dashes done 
    private Rigidbody rb; 
    [HideInInspector] public bool startDash = false; // states whether the dash has started or not
    [HideInInspector] public bool noDashRunning = true; // states whether the dash is currently running or not



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void StartDash(Vector3 dashDirection)
    {
        // dashing function which starts the dash coroutine and increases the dash count 
        // additionally checks whether it's the first dash
        // dashes towards the dashDirection parameter 

        if (dashCount < 1)
        {
            dashCount += 1;
            StartCoroutine(DashCoroutine(dashDirection));
        }
    }

    IEnumerator DashCoroutine(Vector3 dashDirection)
    {

        if (dashDirection == Vector3.zero)
            dashDirection = transform.forward; // if no direction is given from the keyboards than it dashes in the current forward direction


        // rotates the player towards the dash direction
        Quaternion toRotation = Quaternion.LookRotation(dashDirection);
        transform.rotation = toRotation;

        // sets the new velocity
        rb.linearVelocity = dashDirection.normalized * dashForce;

        // waits dashDuration seconds for dashing
        yield return new WaitForSeconds(dashDuration);

        // resets the player's velocity on the x and y plane so he stops in place
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        noDashRunning = true;

    }

    public void ResetDash()
    {
        // resets dash variables needed for starting a dash
        dashCount = 0;
        startDash = false;
    }

    public int GetDashCount()
    {
        return dashCount;
    }



}
