using UnityEditor.Search;
using UnityEngine;

public class Jump : MonoBehaviour
{

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float upwardAcceleration = 5f;
     
    Rigidbody rb;
    private bool isJumping = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (!isJumping)
            return;
        //if (rb.linearVelocity.y < 0)
        //    FallOff();

        if (GetComponent<Movement>().IsGrounded())
            isJumping = false;

    }

    public void RequestJump()
    {
        if (!GetComponent<Movement>().IsGrounded() ) // if not on ground then no can do
            return;

        if (isJumping) // if is already jumping then no can do
            return;

        StartJump();
    }

    private void StartJump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x, 
            jumpForce + Physics.gravity.y * upwardAcceleration * Time.deltaTime, 
            rb.linearVelocity.y
            );
        isJumping = true;
    }
    private void FallOff()
    {
        rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
    }
}
