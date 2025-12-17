using UnityEngine;

public class GoalObject : MonoBehaviour
{
    [SerializeField] private Collisions collisions;
    [SerializeField] private bool isMarked;
    [SerializeField] private int order;

    private ShowAnnouncement showAnnouncement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collisions = GetComponent<Collisions>();
        showAnnouncement = GameObject.Find("GameManager").GetComponent<ShowAnnouncement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collisions != null && collisions.IsDestroyed() && !isMarked)
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

    public void Mark()
    {
        isMarked=true;
    }

    public void showText()
    {
        if (!showAnnouncement)
            Debug.Log("No announcement bar set");
        else
        {
            showAnnouncement.StartShow("This object has been destroyed.");
        }

    }
}
