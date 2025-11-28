using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public bool isActive = false;
    public GameObject indicator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive)
        {
            indicator.SetActive(true);
        }
        else
        {
            indicator.SetActive(false);
        }
    }
}
