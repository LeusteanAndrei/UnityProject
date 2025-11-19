using UnityEngine;
using UnityEngine.InputSystem;

public class OldCameraMovement : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationalSpeed = 10f;
    [SerializeField] private float bottomClamp = -40f;
    [SerializeField] private float topClamp = 70f;

    private float cinemachineTargetRotX;
    private float cinemachineTargetRotY;

    private Vector2 mouseDelta;

    private void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        float mouseX = GetMouseInput().x;
        float mouseY = GetMouseInput().y;

        cinemachineTargetRotX = UpdateRotation(cinemachineTargetRotX, mouseY, bottomClamp, topClamp, true);
        cinemachineTargetRotY = UpdateRotation(cinemachineTargetRotY, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetRotX, cinemachineTargetRotY);
    }

    private void ApplyRotation(float rotX, float rotY)
    {
        followTarget.rotation = Quaternion.Euler(rotX, rotY, followTarget.eulerAngles.z);
    }

    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
    {
        if(isXAxis)
            currentRotation -= input;
        else
            currentRotation += input;

        return Mathf.Clamp(currentRotation, min, max);
    }

    private Vector2 GetMouseInput()
    {
        return rotationalSpeed * Time.deltaTime * mouseDelta;
    }

    public void MoveMouse(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }
}
