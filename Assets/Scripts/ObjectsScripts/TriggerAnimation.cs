using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField] private GoalObject goalObject;
    [SerializeField] private bool triggered = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(goalObject.IsMarked() && !triggered)
        {
            triggered = true;
            GetComponent<Animator>().SetTrigger("Trigger");
        }
    }
}
