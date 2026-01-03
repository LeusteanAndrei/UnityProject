using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private GoalObject goalObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        goalObject = GetComponent<GoalObject>();
    }
    public void SetMarked()
    {
        if (goalObject != null && !goalObject.IsMarked())
        {
            goalObject.Mark();
        }
    }
}
