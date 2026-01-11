using UnityEngine;

public class Chandelier : MonoBehaviour
{

    public float maxHealth = 10.0f;
    public float timeToFall = 3.0f;

    float damageTaken = 0.0f;
    bool startTimer = false;
    float timer = 0;
    HingeJoint hj;

    void Start()
    {
         hj = GetComponent<HingeJoint>();
    }

    void Update()
    {
        UpdateTimer();
        if(damageTaken > maxHealth)
        {
            startTimer = true;  
        }
        if (timer > timeToFall)
        {
            if (hj!= null)
            {
                Destroy(hj);
                if (GetComponent<GoalObject>()!=null)
                {
                    GetComponent<GoalObject>().Mark();
                }
            }
        }

    }

    void UpdateTimer()
    {
        if (startTimer)
        {
            timer += Time.fixedDeltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(hj!=null )
            damageTaken += collision.impulse.magnitude;
        }
    }
}
