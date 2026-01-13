using UnityEngine;
using TMPro; // folosim TextMeshPro, dar poți folosi și UnityEngine.UI.Text

public class ValueGoalManager : MonoBehaviour
{
    [SerializeField] public TMP_Text goalText; // Textul din canvas
    [SerializeField] private GameObject teleport;
    [SerializeField] private int goalAmount1 = 10000;
    [SerializeField] private int goalAmount2 = 10000;

    [Header("Debug")]
    [SerializeField] private int currentAmount = 0;
    [SerializeField] private int goalAmount;
    [SerializeField] private bool goal1Reached;
    [SerializeField] private bool goal2Reached;
    [SerializeField] private GameObject barrier;
    [SerializeField] private ShowAnnouncement showAnnouncement;
    [SerializeField] private SoundMeterManage soundMeterManage;
    [SerializeField] private bool tenMillionShowed = false;
    [SerializeField] private bool noisyKittyShowed = false;
    [SerializeField] private bool uhOhShowed = false;


    private void Awake()
    {
        barrier = GameObject.Find("Room 1 Barrier");
        goal1Reached = goal2Reached = false;
        goalAmount = goalAmount1;
        showAnnouncement = GetComponent<ShowAnnouncement>();
        soundMeterManage = GetComponent<SoundMeterManage>();
    }

    private void Update()
    {
        if (soundMeterManage.currentSoundLevel >= 0.1*soundMeterManage.maxSoundLevel)
        {
            if (!noisyKittyShowed)
            {
                string announcement = "Who's a noisy kitty!?";
                showAnnouncement.StartShow(announcement);
                noisyKittyShowed = true;
            }
        }

        if (soundMeterManage.currentSoundLevel >= 0.75*soundMeterManage.maxSoundLevel)
        {
            if (!uhOhShowed)
            {
                string announcement = "Uh-Oh...! You're getting close to the limit! Aren't you a noisy kitty!?";
                showAnnouncement.StartShow(announcement);
                uhOhShowed = true;
            }
        }

        if (goalText != null)
        {
            goalText.text = $"{currentAmount:N0}$ / \n{goalAmount:N0}$";
        }
    }

    public void AddAmount(int amount)
    {
        currentAmount += amount;

        if (currentAmount >= goalAmount)
        {
            currentAmount = 0;

            if (!goal1Reached)
            {
                goal1Reached = true;
                goalAmount = goalAmount2;
                barrier.SetActive(false);
                string announcement = $"Congratulations! You broke objects with a value over {goalAmount1:N0}$! \nThe vent has opened! Now you can go to the next room.";
                showAnnouncement.StartShow(announcement);
            }

            else
            {
                goal2Reached = true;
                string announcement = $"Hooray! You finished the level! Now you can escape!";
                showAnnouncement.StartShow(announcement);
                teleport.SetActive(true);
            }            
        }

        else if (currentAmount >= 10_000_000)
        {
            if (!tenMillionShowed)
            {
                string announcement = "You made your first ten million! (in damages)";
                showAnnouncement.StartShow(announcement);
                tenMillionShowed = true;
            }
        }
    }
}
