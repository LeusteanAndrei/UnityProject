using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private void Update()
    {
        GetMovementInput();
        GetJumpInput();
    }

    private void GetMovementInput()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 movementDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        GetComponent<Movement>().SetDirection(movementDirection.normalized);

    }

    private void GetJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
            GetComponent<Jump>().RequestJump();
    }
}
