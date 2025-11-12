using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float inAirSpeedMultiplier = 0.5f;

    [Header("Ground Detection")]
    [SerializeField] public LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] public Transform groundOrigin;

    [HideInInspector] public bool isGrounded = false;
    private bool isSprinting = false;

    private Rigidbody rb;
    private Vector3 movementDirection;
    private InputAction moveAction;
    private InputAction sprintAction;

    private Dash dashComponent;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        dashComponent = GetComponent<Dash>();
    }

    private void Update()
    {
        OnGroundReset();
        CheckInputs();
        ReadInput();
        GroundCheck();

    }


    private void FixedUpdate()
    {
        RotateTowards(movementDirection); // rotate the player towards the movement direction

        if (dashComponent.startDash)
        {
            // if we need to start a dash, we call the dash function
            dashComponent.StartDash(movementDirection);
            dashComponent.startDash = false; // set this to zero because we started a dash
        }
        else
        {
            Move();
        }
    }


    /*  Physics based functions */
    /*   - to be called in the FixedUpdate function */

    private void Move()
    {
        // movement function which moves the player
        if (movementDirection.sqrMagnitude < 0.01f)
        {
            // if the movement direction is almost zero, stopts the player's movement
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        float speed = moveSpeed; // normal moving speed
        if (!dashComponent.noDashRunning && isSprinting && isGrounded)
            // if we are sprinting ( that means no dash is running, the sprint button is pressed and we are grounded we set speed to the sprint speed
            speed = sprintSpeed;
        if (!isGrounded)
            // if we are in air, we move the character slower towards the desired direction
            speed *= inAirSpeedMultiplier;
        
        Vector3 desiredVelocity = movementDirection * speed; // the desired velocity
        Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // how much we need to change the velocity to reach the desired velocity
        rb.AddForce(velocityChange, ForceMode.Acceleration); // add the coresponding force
        // ForceMode.Acceleration is used so that the mass of the rigidbody doesn't affect the movement speed
    }

    private void RotateTowards(Vector3 vector)
    {
        // rotates the player towards the given direction vector 
        // the smoothness of the rotation is determined by the rotateSpeed variable ( the bigger the faster )

        if (vector == Vector3.zero)
            return;

        Quaternion toRotation = Quaternion.LookRotation(vector); //  computes the rotation needed to look at the target vector
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,  // current rotation
            toRotation,   // target rotation
            rotateSpeed * Time.deltaTime // how many degrees to rotate this frame 
            );
    }

    public bool GroundCheck()
    {
        // checks wether the user is on ground or not
        // it checks using a sphere cast at the ground origin position ( the feet ) 
        // with the given ground check distance ( the radius of the sphere ) and ground mask ( ground type )
        isGrounded = Physics.CheckSphere(groundOrigin.position, groundCheckDistance, groundMask);
        return isGrounded;
    }


    /*  Function for checking inputs */
    /*   - to be called in the Update function */
    private void CheckInputs()
    {
        if (isGrounded)
        {
            if (sprintAction.IsPressed())
            {
                isSprinting = true;
            }
            else
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;

            if (sprintAction.WasPressedThisFrame())
            {
                dashComponent.startDash = true;
                dashComponent.noDashRunning = false;
            }
        }
        //if (Keyboard.current.leftShiftKey.wasPressedThisFrame && !isGrounded)
        //{
        //    dashComponent.startDash = true;
        //    dashComponent.noDashRunning = false;
        //}
    }
    private void ReadInput()
    {
        // function which reads the movement input and sets the direction accordingly
        Vector2 inputRead = moveAction.ReadValue<Vector2>();
        movementDirection = new Vector3(inputRead.x, 0, inputRead.y).normalized;

    }
    private void OnGroundReset()
    {
        // checks ground and resets all the corresponding elements
        if (GroundCheck())
        {
            dashComponent.ResetDash();
        }
    }


    /* Debugging */
    void OnDrawGizmosSelected()
    {
        if (groundOrigin == null) return;

        Gizmos.color = Color.yellow; // or any color you like
        Gizmos.DrawWireSphere(groundOrigin.position, groundCheckDistance);
    }

}
