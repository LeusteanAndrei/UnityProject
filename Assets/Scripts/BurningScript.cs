using Unity.VisualScripting;
using UnityEngine;

public class BurningScript : MonoBehaviour
{
    bool startBurn = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        GameObject go = collision.gameObject;
        if ( go.GetComponent<BurningScript>()!=null)
        {
            startBurn = true;
        }
        if(go.GetComponent<Chandelier>()!=null)
        {
            startBurn = true;
        }
    }
}
