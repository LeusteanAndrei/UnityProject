using UnityEngine;
using UnityEngine.InputSystem;

public class WallClimb : MonoBehaviour
{
    [SerializeField] private LayerMask wallMask;

    [Header("Movement")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float stickForce = 5f;
    [SerializeField] private float staminaConsumption = 1f;
    [HideInInspector] public bool isClimbing = false;


    private StaminaBarScript staminaBar;

    private float sphereCastRadius = 0.5f;
    private float sphereCastDistance = 1f;


    private Rigidbody rb;
    private Vector3 wallNormal; 
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpAction = InputSystem.actions.FindAction("Jump");
        staminaBar = GetComponent<Stamina>().staminaBar;
    }

    void FixedUpdate()
    {
        CheckWall();
        HandleWallClimb();
    }
    private void CheckWall()
    {
        if ((Keyboard.current[Key.C].isPressed))
        {
            isClimbing = false;
            return;
        }

        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.SphereCast(origin, sphereCastRadius, direction, out hit, sphereCastDistance, wallMask))
        {
            isClimbing = true;
            wallNormal = hit.normal;
        }
        else
        {
            if (isClimbing)
            {
                rb.AddForce(transform.forward * climbSpeed, ForceMode.VelocityChange); // pushing forward to get over the edge of the wall
            }
            isClimbing = false;
        }
    }


    private void HandleWallClimb()
    {
        if ((Keyboard.current[Key.C].isPressed))  return;
        if (!isClimbing) return;
        if (staminaBar.getStamina() == 0) return;

        staminaBar.ConsumeStamina(staminaConsumption * Time.fixedDeltaTime);

        Vector3 horizontalVelocity = rb.linearVelocity - Vector3.Dot(rb.linearVelocity, Vector3.up) * Vector3.up; 
        Vector3 towardWall = -wallNormal * Vector3.Dot(horizontalVelocity, -wallNormal);
        rb.linearVelocity -= towardWall;

        rb.AddForce(-wallNormal * stickForce, ForceMode.Acceleration); // sticking to the wall

        // Climb up if jump pressed
        if (jumpAction != null && jumpAction.IsPressed())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, climbSpeed, rb.linearVelocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        }
    }

}
