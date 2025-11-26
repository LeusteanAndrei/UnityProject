using UnityEngine;


/* class for checking the ground collision */
public class GroundCheck : MonoBehaviour
{

    int groundContacts = 0; // number of ground contacts
    Movement movementComponent;

    void Start()
    {
        movementComponent = GetComponentInParent<Movement>();
    }


    private void OnTriggerEnter(Collider other)
    {
        groundContacts++;
        UpdateGround(other.gameObject);

    }   

    private void OnTriggerExit(Collider other)
    {
        //if (!IsGroundLayer(other.gameObject))
        //    return;
        
        groundContacts = Mathf.Max( 0, groundContacts - 1 );
        UpdateGround(other.gameObject);
    }


    private void UpdateGround(GameObject obj)
    {
        if (movementComponent == null) return;
        movementComponent.isGrounded = groundContacts > 0;
        movementComponent.currentLayerIndex = obj.layer;
    }


    //// checks wether the layer of the object is the groundMask layer 
    //private bool IsGroundLayer(GameObject obj)
    //{
    //    return groundMask == (groundMask | (1 << obj.layer));
    //}

}
