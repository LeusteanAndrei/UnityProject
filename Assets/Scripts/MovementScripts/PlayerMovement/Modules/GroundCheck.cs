using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    [SerializeField] public LayerMask groundMask;

    Movement movementComponent;

    void Start()
    {
        movementComponent = GetComponent<Movement>();
    }

    void Update()
    {}

    private void OnTriggerEnter(Collider other)
    {
        if (IsGroundLayer(other.gameObject))
        {
            movementComponent.isGrounded = true;
            Debug.Log("Ground");
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        return groundMask == (groundMask | (1 << obj.layer));
    }

}
