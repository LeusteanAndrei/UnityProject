using UnityEngine;

public class MoveBusScript : MonoBehaviour
{

    [SerializeField] private Checklist checklist;
    [SerializeField] private Vector3 movePoz;
    [SerializeField] private float moveSpeed = 5f;

    private ShowAnnouncement showAnnouncement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (checklist == null) 
            checklist = GameObject.Find("GameManager").GetComponent<Checklist>();

        showAnnouncement = GameObject.Find("GameManager").GetComponent<ShowAnnouncement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (checklist.finished == true )
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                movePoz,
                moveSpeed * Time.deltaTime
            );

            if (showAnnouncement == null)
            {
                Debug.Log("The announcement board has not been set");
            }
            else
            {
                showAnnouncement.StartShow("Congratulations, you have destroyed all objects. The bus has started moving.");
            }
        }
        
    }
}
