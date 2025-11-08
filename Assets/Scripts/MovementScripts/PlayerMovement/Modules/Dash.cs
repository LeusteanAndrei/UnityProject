using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{

    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 2f;

    private int dashCount;
    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void StartDash(Vector3 dashDirection)
    {
        if (dashCount < 1)
        {
            StartCoroutine(DashCoroutine(dashDirection));
        }
    }

    IEnumerator DashCoroutine(Vector3 dashDirection)
    {
        dashCount += 1;

        // Make sure dashDirection is normalized
        if (dashDirection == Vector3.zero)
            dashDirection = transform.forward;

        // Rotate the player towards the dash direction
        Quaternion toRotation = Quaternion.LookRotation(dashDirection);
        transform.rotation = toRotation;

        // Apply dash velocity
        rb.linearVelocity = dashDirection.normalized * dashForce;

        yield return new WaitForSeconds(dashDuration);

        // Stop horizontal movement after dash
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    public void ResetDash()
    {
        dashCount = 0;
    }

    public int GetDashCount()
    {
        return dashCount;
    }

}
