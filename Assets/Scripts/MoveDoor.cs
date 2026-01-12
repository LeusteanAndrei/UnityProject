using UnityEngine;

public class MoveDoor : MonoBehaviour
{

    public Vector3 targetRot;
    public bool shouldMove = false;
    public bool moved = true;

    public float moveSpeed = 2f;
    public float rotateSpeed = 2f;

    private Checklist checklist;
    private ShowAnnouncement showAnnouncement;
    void Start()
    {
        if (checklist == null)
            checklist = GameObject.Find("GameManager").GetComponent<Checklist>();

        showAnnouncement = GameObject.Find("GameManager").GetComponent<ShowAnnouncement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGoalObjects();
        if (shouldMove && !moved)
            MoveToTarget();
    }

    void CheckGoalObjects()
    {
        if (checklist.finished)
        {
            shouldMove = true;
            showAnnouncement.StartShow("Doors have ben opened.");
        }
    }

    void MoveToTarget()
    {


        Quaternion targetQuaternion = Quaternion.Euler(targetRot);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetQuaternion,
            rotateSpeed * Time.deltaTime
        );

        if (
            transform.rotation == targetQuaternion)
        {
            moved = true;
        }
    }
}
