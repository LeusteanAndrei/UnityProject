using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class Jump : MonoBehaviour
{
    int logindex = 0;
    [SerializeField] private float jumpForce = 15f; // the force with which the character jumps upwards
    [SerializeField] private float firstJumpmaxHoldTime = 0.5f; // the maximum hold time for the enhanced jump
                                                                // ( so that the jump increases after a certain amount of time while holding)
    [SerializeField] private float secondJumpMaxHoldTime = 0.2f; // same but for the second jump
    [SerializeField] private float holdJumpMultiplier = 0.5f; // the number with which to decrease the jump when space is no longer pressed
    [SerializeField] private float fallMultiplier = 3f; // the multiplier for the gravity when falling down

   
    Rigidbody rb; 
    Movement movementComponent; // the movement component ( class ) of the player
    float currentHoldTime = 0; // the current holdTime of the jump button
    int nrJump = 0; // the number of jumps the player has done ( resets when touching the ground )
    private float jumpStartPosition = 0f; // the current height of the jump

    InputAction jumpAction; // input action for the jump
    private bool shouldJump = false; // wether the player should jump or not
    private bool holdJump = false; // wether the player is holding jump or not

    private bool wasGroundedLastFrame = true;
    private bool reduceSecondJump = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<Movement>();
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        CheckJumpInput();


    }

    private void FixedUpdate()
    {
        TryJump();
        TryFallOff();

        if (movementComponent.isGrounded)
        {
            if (!wasGroundedLastFrame)
            {
                ResetJump();
            }
            wasGroundedLastFrame = true;
        }
        else
        {
            wasGroundedLastFrame = false;
        }
    }

    /*  Functions for checking input */
    private void CheckJumpInput()
    {
        if (jumpAction.WasPressedThisFrame())
            shouldJump = true;
        if (jumpAction.IsPressed())
            holdJump = true;
        else
            holdJump = false;
    }


    /*  Functions for jumping */

    private void TryJump()
    {
        if ( shouldJump )
        {
            if ( nrJump < 2 )
            {
                RequestJump();
            }
        }

        if (rb.linearVelocity.y > 0)
        {
            // if we're moving up in the jump
            float jumpHeight = transform.position.y - jumpStartPosition;

            if (!holdJump )
            {
                // if the space bar was released and it is the first jump, reduce the velocity by a multiplier to cut the jump short
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * holdJumpMultiplier, rb.linearVelocity.z);
                
                //if (currentHoldTime < maxHoldTime)
                //{
                //    rb.AddForce(Vector3.up * holdForce * Time.fixedDeltaTime, ForceMode.Impulse);
                //}
            }
            else if (holdJump)
            {
                // if the player is still holding the jump button ( and it is the first jump )
                currentHoldTime += Time.fixedDeltaTime; // increase the hold time
            }

            if (nrJump == 1)
            {
                //Debug.Log("Jump Hold Time: " + currentHoldTime + " Log Index: " + logindex);
                if (currentHoldTime > firstJumpmaxHoldTime)
                {
                    // if the hold time exceeded the maximum hold time, we stop the jump
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * holdJumpMultiplier, rb.linearVelocity.z);
                }
            }
            else if (nrJump == 2)
            {
                //Debug.Log("Second Jump Hold Time: " + currentHoldTime + " Log Index: " + logindex);
                if (currentHoldTime > secondJumpMaxHoldTime)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * holdJumpMultiplier, rb.linearVelocity.z);
                }
            }
        }

        if (!holdJump)
            currentHoldTime = 0;

        logindex++;
        shouldJump = false;

    }
    public void RequestJump()
    {
        nrJump += 1;
        float currentJumpForce = jumpForce;
        jumpStartPosition = 0;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * currentJumpForce, ForceMode.Impulse);
        
    }
    private void TryFallOff()
    {
        // if falling down then add an extra force pulling it down so it feels crispier

        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    private void ResetJump()
    {
        logindex++;
        shouldJump = false;
        holdJump = false;
        nrJump = 0;
    }

}
