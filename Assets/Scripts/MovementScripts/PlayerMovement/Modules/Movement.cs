using UnityEngine;

public class Movement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 10f;

    [Header("GroundInfo")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance = 0.5f;
    [SerializeField] private Transform groundOrigin; // the feet or bottom of the character

    private Vector3 movementDirection;
    private Rigidbody rb;
    private bool isGrounded = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 direction)
    {
        movementDirection = direction;
    }

    private void Update()
    {
        GroundCheck();
        if (isGrounded)
        {
            Vector3 velocity = new Vector3(
                moveSpeed * movementDirection.x,
                rb.linearVelocity.y,
                moveSpeed * movementDirection.z
                );
            rb.linearVelocity = velocity;
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundOrigin.position, groundCheckDistance, groundMask);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}
