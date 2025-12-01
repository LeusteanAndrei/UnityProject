using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float inAirSpeedMultiplier = 0.5f;
    [SerializeField] private float crouchSpeedMultiplier = 0.3f;
    [SerializeField] private float staminaSprintDrainCost = 20f;
    [Range(0f, 1f)]
    [SerializeField] private float turnSmoothness = 0.3f; // variable which controls how smooth the character turns

    [Header("Ground movement")]
    [SerializeField] private LayerStruct slipperyLayers ; // all the slipperyLayers

    //[Header("Shake settings")]
    //[SerializeField] private float sprintIntensity;
    //[SerializeField] private float sprintShakeDuration = 1f;
    //[SerializeField] private float frequency = 1f;

    [Header("Camera settings")]
    [SerializeField] private Transform cameraTransform;

    [Header("Stamina bar")]
    [SerializeField] public StaminaBarScript staminaBar;


    [HideInInspector] public bool isGrounded = false;
    private bool isSprinting = false;

    private Rigidbody rb;
    private Vector3 movementDirection;
    private InputAction moveAction;
    private InputAction sprintAction;

    private Dash dashComponent;
    private Crouch crouchComponent;


    [HideInInspector] public int currentLayerIndex;
    private bool onSlipperyLayer = false;
    private bool isSliding = false;
    private Vector3 slideDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {

        rb.linearDamping = 0f;
        rb.angularDamping = 0f;

        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");

        dashComponent = GetComponent<Dash>();
        crouchComponent = GetComponent<Crouch>();

    }

    private void Update()
    {
        onSlipperyLayer = LayerIsInMask(currentLayerIndex, slipperyLayers.layers);
        OnGroundReset();
        CheckInputs();
        ReadInput();

        HandleStaminaBar();

    }


    private void FixedUpdate()
    {
        movementDirection = GetDirectionRelativeToCamera(movementDirection); // get the movement direction relative to the camera

        if (isSliding)
        {
            Spin(slipperyLayers.spinSpeed);
        }

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



    private void HandleStaminaBar()
    {
        if (isSprinting)
        {
            staminaBar.ConsumeStamina(staminaSprintDrainCost * Time.fixedDeltaTime); // if the player is sprinting, consume stamina
        }
    }


    /*  Physics based functions */
    /*   - to be called in the FixedUpdate function */



    private void Move()
    {


        // movement function which moves the player
        if (movementDirection.sqrMagnitude < 0.01f)
        {
            if (!onSlipperyLayer)
            {
                // if the movement direction is almost zero, stops the player's movement
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                isSliding = false;
            }
            else { 
                Slide();
            }
            return;
        }

        isSliding = false;
        float speed = moveSpeed; // normal moving speed
        if (dashComponent.noDashRunning && isSprinting && isGrounded && !crouchComponent.isCrouching && staminaBar.getStamina()>0 )
            // if we are sprinting ( that means no dash is running, the sprint button is pressed and we are grounded we set speed to the sprint speed )
            speed = sprintSpeed;
        if (!isGrounded)
            // if we are in air, we move the character slower towards the desired direction
            speed *= inAirSpeedMultiplier;
        if ( crouchComponent.isCrouching )
            // if we're crouching we move him slower compared to his initial speed 
            speed *= crouchSpeedMultiplier;

        float smoothness = turnSmoothness;

        if (onSlipperyLayer)
            smoothness = slipperyLayers.turnSmoothness;

        Vector3 desiredVelocity = movementDirection * speed; // the desired velocity
        Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // how much we need to change the velocity to reach the desired velocity


        Vector3 currentVelocity = new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z); // the current velocity of the object
        Vector3 projection = Vector3.Project(currentVelocity, movementDirection); // the projection of the current velocity on the new movement direction


        projection = Vector3.Lerp(currentVelocity, projection, smoothness); // lerping it for greater control using the turn smooth variable
        rb.linearVelocity = new Vector3(projection.x, rb.linearVelocity.y, projection.z); // the new velocity, using the projection to keep the momentum

        rb.AddForce(velocityChange, ForceMode.Acceleration); // add the coresponding force

    }

    private Vector3 GetDirectionRelativeToCamera(Vector3 currentDir)
    {
        // function which returns the normalized direction vector of the movement ( currentDir ) relative to the facing of the camera

        Vector3 cameraForward = cameraTransform.forward; // gets the forward direction of the camera
        cameraForward.y = 0; // we only want the direction on the xz plane

        Vector3 cameraRight = cameraTransform.right; // gets the right direction of the camera
        cameraRight.y = 0;  // we only want the direction on the xz plane

        Vector3 newDir = cameraForward * currentDir.z + cameraRight * currentDir.x;
        // z component = normal forward direction
        // x component = normal right direction
        // new direction = forward of camera * normal forward + right of camera * normal right

        return newDir.normalized; // return it normalized
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

    private void Slide()
    {
        if (!isSliding)
        {
            isSliding = true;
            slideDirection = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.y);
            rb.AddForce(slideDirection * slipperyLayers.slideSpeed, ForceMode.Acceleration);
            rb.AddForce(-slideDirection * slipperyLayers.friction, ForceMode.Acceleration);
        }
    }

    private void Spin(float speed)
    {
        if (rb.linearVelocity.magnitude > 0.01f)
            transform.Rotate(0f, speed * Time.deltaTime * rb.linearVelocity.magnitude, 0);
    }

    /* Helper to check if layer in mask */
    private bool LayerIsInMask(int layerIndex, LayerMask mask)
    {
        if ( (mask.value & (1<<layerIndex)) != 0 )
        {

            return true;
        }
        return false;
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
                if (staminaBar.getStamina() > dashComponent.dashStaminaConsumption) // if there's enough stamina for dashing
                {
                    dashComponent.startDash = true;
                    dashComponent.noDashRunning = false;
                    staminaBar.ConsumeStamina(dashComponent.dashStaminaConsumption); // consume stamina
                }
            }
        }

    }

    private void ReadInput()
    {
        // function which reads the movement input and sets the direction accordingly
        Vector2 inputRead = moveAction.ReadValue<Vector2>();
        Vector3 newMovementDir = new Vector3(inputRead.x, 0, inputRead.y).normalized;
        movementDirection = newMovementDir;
    }

    private void OnGroundReset()
    {
        // checks ground and resets all the corresponding elements
        if (isGrounded)
        {
            dashComponent.ResetDash();
        }
    }


}
