using UnityEngine;

public class StupidEnemyScrpt : MonoBehaviour
{

    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(GetComponent<EnemyMovement>()!=null)
        {

            Vector3 toTarget = player.transform.position - transform.position;
            Quaternion targetRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = targetRot;
            Debug.Log("Supid enemy looking");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
