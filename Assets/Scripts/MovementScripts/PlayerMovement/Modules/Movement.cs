using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float sprintSpeed = 15f;

    [Header("Ground Detection")]
    [SerializeField] public LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] public Transform groundOrigin;
    [SerializeField] private float groundCheckJumpDistance = 0.2f;

    [Header("Hover Settings")]
    [SerializeField] private float maxHoverHeight = 2f;      
    [SerializeField] private float hoverUpwardForce = 10f;     
    [SerializeField] private float hoverDamping = 5f;

    private Rigidbody rb;
    private bool isGrounded = false;

    private Vector3 movementDirection;
    InputAction moveAction;
    
    Jump jumpComponent;
    Dash dashComponent;

    private int sprintNr = 0;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpComponent = GetComponent<Jump>();
        dashComponent = GetComponent<Dash>();
    }

    private void Update()
    {
        if ( isGrounded )
        {
            dashComponent.ResetDash();
        }

        if ( GroundCheckJump() )
        {
            jumpComponent.ResetJumps();
        }

        Hover();
        ReadInput();
        Move();
    }

    private void RotateTowards(Vector3 vector)
    {
        if (vector == Vector3.zero)
            return;
        Quaternion toRotation = Quaternion.LookRotation(vector);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            toRotation, 
            rotateSpeed * Time.deltaTime);
    }

    private void Move()
    {
        if (movementDirection.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }
        
        RotateTowards(movementDirection);
        float speed = moveSpeed;
        if (isSprinting() && !jumpComponent.isJumping())
            speed = sprintSpeed;

        if (Keyboard.current.leftShiftKey.isPressed && jumpComponent.isJumping())
        {
            dashComponent.StartDash(movementDirection);
        }

        // direct prin transform
        //if (GroundCheck())
        //{
        //    transform.position += movementDirection * speed * Time.deltaTime;
        //}
        // cu fizica
        if ( GroundCheck() || jumpComponent.isJumping())
        {
            Vector3 desiredVelocity = movementDirection * speed;
            Vector3 velocityChange = desiredVelocity - new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(velocityChange, ForceMode.Acceleration);
        }
    }

    private bool isSprinting()
    {
        return Keyboard.current.leftShiftKey.isPressed;
    }
   
    private void ReadInput()
    {
        Vector2 inputRead = moveAction.ReadValue<Vector2>();
        movementDirection = new Vector3(inputRead.x, 0, inputRead.y).normalized;
    }

    private void Hover()
    {
        if(!GroundCheck())
            return;
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, maxHoverHeight * 2f, groundMask))
        {
            float distance = hit.distance;
            float heightError = maxHoverHeight - distance;

            float upwardSpeed = heightError * hoverUpwardForce;

            float damp = -rb.linearVelocity.y * hoverDamping;

            Vector3 force = Vector3.up * (upwardSpeed + damp);
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    public bool GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundOrigin.position, groundCheckDistance, groundMask);
        return isGrounded;
    }

    public bool GroundCheckJump()
    {
        return Physics.CheckSphere(groundOrigin.position, groundCheckJumpDistance, groundMask);
    }


    public bool IsGrounded()
    {
        return isGrounded;
    }

    public void ResetSprints()
    {
        sprintNr = 0;
    }
}
