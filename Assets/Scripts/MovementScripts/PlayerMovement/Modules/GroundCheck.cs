using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


/* class for checking the ground collision */
public class GroundCheck : MonoBehaviour
{

    [SerializeField] private List<int> groundLayers = new List<int>();
    private HashSet<int> _groundLayerSet;

    int groundContacts = 0; // number of ground contacts
    Movement movementComponent;


    void Start()
    {
        movementComponent = GetComponentInParent<Movement>();
        _groundLayerSet = new HashSet<int>(groundLayers);
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

    private void OnCollisionStay(Collision other)
    {
        UpdateGround(other.gameObject);
    }

    private void UpdateGround(GameObject obj)
    {
        if (movementComponent == null) return;
        if (!IsGround(obj.layer)) return;
        movementComponent.isGrounded = groundContacts > 0;
        movementComponent.currentLayerIndex = obj.layer;
    }


    private bool IsGround(int layerNumber)
    {
        return _groundLayerSet.Contains(layerNumber);
    }

    //// checks wether the layer of the object is the groundMask layer 
    //private bool IsGroundLayer(GameObject obj)
    //{
    //    return groundMask == (groundMask | (1 << obj.layer));
    //}

}
