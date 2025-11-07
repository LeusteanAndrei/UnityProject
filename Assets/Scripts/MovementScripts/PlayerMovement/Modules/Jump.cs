using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Jump : MonoBehaviour
{

    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxHoldTime = 1.0f;
    [SerializeField] private float holdForce = 1.0f;


    Rigidbody rb;
    Movement movementComponent;
    int nrJump = 0;

    float holdTime = 0;

    InputAction jumpAction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movementComponent = GetComponent<Movement>();
        jumpAction = InputSystem.actions.FindAction("Jump");

    }

    void Update()
    {
        if (movementComponent.GroundCheckJump())
        {
            nrJump = 0;
        }
        StartJump();
    }

    private void StartJump()
    {
        if (jumpAction.WasPressedThisFrame() && nrJump < 2)
        { 
            RequestJump();
            holdTime = 0f;
        }
        else if (jumpAction.IsPressed() && nrJump == 1 )
        {
            holdTime += Time.deltaTime;
            
            if (holdTime < maxHoldTime)
            {
                float t = holdTime / maxHoldTime;          
                float smoothFactor = Mathf.SmoothStep(1f, 0f, t);
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
        nrJump += 1;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public bool isJumping()
    {
        return nrJump > 0;
    }

}
