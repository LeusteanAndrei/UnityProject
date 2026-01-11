using UnityEngine;
using TMPro;

public class ShowAnnouncement : MonoBehaviour
{

    [SerializeField] public float ShowForSeconds = 2.0f;
    public GameObject announcementBoard;
    public TextMeshProUGUI announcementText;


    float showTimer = 0.0f;
    bool showing = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        announcementBoard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            showTimer += Time.deltaTime;
        }
        if (showTimer >= ShowForSeconds)
        {
            StopShow();
        }
    }

    void StopShow()
    {
        showTimer = 0.0f;
        showing = false;
        announcementBoard.SetActive(false);
    }

    public void StartShow(string text)
    {
        announcementText.text = text;
        announcementBoard.SetActive(true);
        showing = true;
        showTimer = 0;
    }
}

