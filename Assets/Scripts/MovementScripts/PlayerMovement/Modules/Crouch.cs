using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Crouch : MonoBehaviour
{


    // momentarily we are only shrinking the capsule collider height when crouching
    [Header("Collider")]
    [SerializeField] private CapsuleCollider playerCollider; 
    [SerializeField] private float crouchHeightMultiplier = 0.5f; // the amount the size shrinks when crouching
    
    private float standingHeight;
    [HideInInspector] public bool isCrouching;
    private InputAction crouchAction;
    Movement movementComponent;

    bool canRiseUp = true; // tells wether the player can rise up from crouch

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crouchAction = InputSystem.actions.FindAction("Crouch");
        standingHeight = playerCollider.height;
        movementComponent = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!movementComponent.isGrounded)
            // if the player is in the air he shouldn't be able to crouch and if he is crouching he should "stand up"
            ToggleCrouch(false);
        else
        {
            // he's not in the air => we check the crouch button and toggleCrouch
            if (crouchAction.IsPressed())
                ToggleCrouch(true);
            else
                ToggleCrouch(false);
        }
    }

    private void FixedUpdate()
    {
        // if we hit sth we can't rise up and if we didn't we can ( ! -> operator )
        canRiseUp = !CheckUpwardsCollider();
    }

    private void ToggleCrouch(bool crouching)
    {
        // crouching tells us what we want to do ( true if we want to crouch, false if not )
        // is Crouchin tells us what we are doing ( true if crouching, false if not

        if (crouching == isCrouching) return; // if the character is already in the correct state, do nothing

        if (crouching == false)
            if (!canRiseUp) // if we can't rise up ( collided with something above ) we don't uncrouch
                return;

        float currentHeight = playerCollider.height; // current player height
        float targetHeight = standingHeight; // the height we want to reach, by default the players standing heihgt
        if (crouching)
            // if crouching multiply that height by a number < 1 to reduce it
            targetHeight = standingHeight * crouchHeightMultiplier;

        playerCollider.height = targetHeight; // set the player height

        float changeInHeight = currentHeight - targetHeight; // the amount of height changed

        // we need to reposition the collider center downwards/upwards depdending on crouching/uncrouching
        playerCollider.center =  new Vector3(
            playerCollider.center.x, 
            playerCollider.center.y -  changeInHeight/2, // because it's the center we only move it by half the change in height
            playerCollider.center.z);

        isCrouching = crouching; // update the current state
    }


    private bool CheckUpwardsCollider()
    {
        // checks for upward collision 
        // return true if we hit something, false otherwise

        Vector3 start = playerCollider.transform.position + playerCollider.center; // convert the center to world space coordinates

        float currentHeight = playerCollider.height; 
        float castDistance = standingHeight - currentHeight; // the distance we need to check 

        RaycastHit hit;
        if (Physics.SphereCast(start, playerCollider.radius, Vector3.up, out hit, castDistance))
        {
            Debug.Log("Object hit: " + hit.collider.name);
            // if we hit, we return true
            return true;
        }
        return false;

    }

}
