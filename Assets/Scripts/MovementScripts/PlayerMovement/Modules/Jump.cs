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
    int nrJump = 0; // the number of jumps the player has done ( resets when touching the ground )
    float holdTime = 0; // the current holdTime of the jump button

    InputAction jumpAction; // input action for the jump
   
    private bool shouldJump = false;
    private bool holdJump = false;
    private void Start()
    {
        // retrieving the necessary values needed for the jump
        rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<Movement>();
        jumpAction = InputSystem.actions.FindAction("Jump");

    }

    private void Update()
    {
        if (movementComponent.isGrounded)
        {
            Debug.Log("resett");
            ResetJumps();
        }

        if(jumpAction.WasPressedThisFrame())
            shouldJump = true;
        
        if(jumpAction.IsPressed())
            holdJump = true;

    }
    private void FixedUpdate()
    {
        StartJump();    

        shouldJump = false;
        holdJump = false;
    }

    private void StartJump()
    {

        /* the jump function behavese as described:
            if the button was pressed this frame and the number of jumps is lower than 1 then request a normal jump and reset the hold time
               ( this way we can double jump )
            else if the jumpNumber is 0 and space is pressed ( first jump ) than increase the force based on the hold time
            else ( nothing else to do, nrJumps becomes higher then 2 so we finished with the current action ) 
        */
  
        if (shouldJump && nrJump < 2 )
        {
            Debug.Log(logindex + "nr jump: " + nrJump);
            logindex++;
            Debug.Log(logindex+"Jumped");
            logindex++;
            nrJump++;
            RequestJump(); // jump function which adds the upwards force
            holdTime = 0f;
        }
        else if (holdJump && nrJump == 0 )
        {
            holdTime += Time.deltaTime;
            
            if (holdTime < maxHoldTime)
            {
                float t = holdTime / maxHoldTime; // the force proportial with the holding time         
                float smoothFactor = Mathf.SmoothStep(0f, 1f, t); // smoothens it so the enhanced jump increase doesn't feel sudden
                rb.AddForce(Vector3.up * holdForce * smoothFactor, ForceMode.Acceleration);
            }
        }
        else if (jumpAction.WasReleasedThisFrame())
        {
            holdTime = 0f;
        }
    }

    public void RequestJump()
    {
        Debug.Log(logindex + "Requested Jump");
        logindex++;
        Debug.Log(logindex + "NrJump before jump: " + nrJump);
        logindex++;
        Debug.Log(logindex + "NrJump after jump: " + nrJump);
        logindex++;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public bool isJumping()
    {
        return nrJump != 0;
    }

    public void ResetJumps()
    {
        this.nrJump = 0;
    }

}
