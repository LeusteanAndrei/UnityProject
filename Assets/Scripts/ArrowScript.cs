using TMPro;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    public Transform objectToLookTowards;
    [HideInInspector] public Transform player;
    public Vector3 offset;
    public float speed;

    public Transform Spinner;
    public Transform Scaler;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToLookTowards==null)
            Spinner.gameObject.SetActive(false);
        else Spinner.gameObject.SetActive(true);
        transform.position = player.position + offset;
    }
    private void LateUpdate()
    {
        if (objectToLookTowards != null)
            transform.LookAt(objectToLookTowards);
        if (Spinner)
            Spinner.transform.Rotate(0.0f, 0.0f, speed * Time.deltaTime);
    }
}
