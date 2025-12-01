using UnityEngine;
using UnityEngine.InputSystem;

public class Glide : MonoBehaviour
{
    Jump jumpComponent;
    Movement movementComponent;

    [SerializeField] float glideFallSpeed = 2f; // the multiplier for the gravity when falling down while gliding
    [SerializeField] float glideStaminaConsumption = 10f;

    Rigidbody rb;
    InputAction jumpAction;

    private bool isGrounded;
    bool holdJump; // wether the player is holding jump or not

    int nrJump; // the number of jumps the player has done ( resets when touching the ground )
    Vector3 velocity;
    bool isGliding = false;

    StaminaBarScript staminaBar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<Movement>();
        jumpAction = InputSystem.actions.FindAction("Jump");
        jumpComponent = GetComponent<Jump>();
        movementComponent = GetComponent<Movement>();
        staminaBar = GetComponent<Stamina>().staminaBar;
    }

    void FixedUpdate()
    {
        if (!isGrounded && holdJump && nrJump >= 2 && staminaBar.getStamina()>0)
        {
            isGliding = true;
            AllowGliding();
            ConsumeStamina();
        }

        else
            isGliding = false;
    }

    private void Update()
    {
        UpdateVars();
    }

    void UpdateVars()
    {
        isGrounded = movementComponent.GetIsGrounded();
        holdJump = jumpComponent.GetHoldJump();

        if (jumpAction.WasPressedThisFrame())
        {
            if (isGrounded)
            {
                nrJump = 0;
            }
            nrJump++;
            //Debug.Log("nrJump: " + nrJump);
            //Debug.Log("isGrounded: " + isGrounded);
        }
    }

    void AllowGliding()
    {
        velocity = rb.linearVelocity;

        if (velocity.y < -glideFallSpeed)
        {
            velocity.y = -glideFallSpeed;
        }

        rb.linearVelocity = velocity;

    }

    void ConsumeStamina()
    {
        staminaBar.ConsumeStamina(glideStaminaConsumption * Time.fixedDeltaTime);
    }

    public bool GetIsGliding() { return isGliding; }
}
