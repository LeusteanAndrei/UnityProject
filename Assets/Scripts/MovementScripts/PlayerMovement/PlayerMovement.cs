using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 0; 

    }
}
