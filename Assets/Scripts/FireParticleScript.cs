using UnityEngine;

public class FireParticleScript : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Burning cat");
            GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().GameOver();
        }
    }
}
