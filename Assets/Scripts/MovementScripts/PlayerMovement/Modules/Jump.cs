using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Jump : MonoBehaviour
{
    int logindex = 0;
    [SerializeField] private float jumpForce = 5f; // the force with which the character jumps upwards
    [SerializeField] private float maxHoldTime = 1.0f; // the maximum hold time for the enhanced jump
                                                       // ( so that the jump increases after a certain amount of time while holding)
    [SerializeField] private float holdForce = 1.0f; // the force with which the jump constantly increases while holding


    Rigidbody rb; 
    Movement movementComponent; // the movement component ( class ) of the player
    float currentHoldTime = 0; // the current holdTime of the jump button
    int nrJump = 0; // the number of jumps the player has done ( resets when touching the ground )

    InputAction jumpAction; // input action for the jump
    private bool shouldJump = false; // wether the player should jump or not
    private bool holdJump = false; // wether the player is holding jump or not

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
        tryJump();
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

    public void tryJump()
    {
        if (nrJump == 2 && !movementComponent.isGrounded)
            return;
        
        if ( shouldJump )
        {
            if ( nrJump == 0 || nrJump == 1 )
            {
                RequestJump();
            }
        }
        
        //if ( holdJump )
        //{
        //    if ( nrJump == 0 )
        //    {
        //        currentHoldTime += Time.fixedDeltaTime;
        //        if (currentHoldTime < maxHoldTime)
        //        {
        //            rb.AddForce(Vector3.up * holdForce * Time.fixedDeltaTime, ForceMode.Impulse);
        //        }
        //    }
        //}

        Debug.Log(nrJump);
        shouldJump = false;

    }


    public void RequestJump()
    {
        nrJump += 1; 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        shouldJump = false;
        holdJump = false;
        nrJump = 0;
    }

}
