using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [SerializeField] private Collisions collisions;
    [SerializeField] private bool isMarked;
    [SerializeField] private int order;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisions = GetComponent<Collisions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collisions != null && collisions.IsDestroyed())
        {
            isMarked = true;
        }
    }
    public bool IsMarked()
    {
        return isMarked;
    }
    public int GetOrder()
    {
        return order;
    }
}
