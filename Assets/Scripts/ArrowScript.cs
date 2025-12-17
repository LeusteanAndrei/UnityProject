using TMPro;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    [SerializeField] public Transform objectToLookTowards;
    [SerializeField] public Transform player;
    [SerializeField] public Rigidbody playerRigidBody;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;
    [SerializeField] private float waitSeconds = 1.0f;
    [SerializeField] private float showSeconds = 1.0f;



    private float timer = 0.0f;
    private float showTimer = 0.0f;
    bool showing = false;
    public Transform Spinner;
    public Transform Scaler;

    public Checklist checklist;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (checklist == null)
            checklist = GameObject.Find("GameManager").GetComponent<Checklist>();
    }

    // Update is called once per frame
    
    
    void Update()
    {
        UpdateTimers();



        if (checklist)
        {
            objectToLookTowards = checklist.currentObjectToDestroy;
        }

        if (timer >= waitSeconds)
        {
            showing = true;
            Spinner.gameObject.SetActive(true);

        }

        if (objectToLookTowards == null)
            Spinner.gameObject.SetActive(false);

        if (showTimer >= showSeconds)
        {
            showing = false;
            showTimer = 0.0f;
            timer = 0.0f;
            Spinner.gameObject.SetActive(false);
        }

        transform.position = player.position + offset;
    }



    private void UpdateTimers()
    {
        if (playerRigidBody.linearVelocity.magnitude <=0.001)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0.0f;
        }


        if (showing == true)
        {
            showTimer += Time.deltaTime;
        }
        else
        {
            showTimer = 0.0f;
        }
    }


    private void LateUpdate()
    {
        if (objectToLookTowards != null)
            transform.LookAt(objectToLookTowards);
        if (Spinner)
            Spinner.transform.Rotate(0.0f, 0.0f, speed * Time.deltaTime);
    }
}
