using UnityEngine;
using TMPro; // folosim TextMeshPro, dar poți folosi și UnityEngine.UI.Text

public class ValueGoalManager : MonoBehaviour
{
    [SerializeField] private int goalAmount1 = 10000;
    [SerializeField] private int goalAmount2 = 10000;
    [Header("Debug")]
    [SerializeField] public TMP_Text goalText; // Textul din canvas
    [SerializeField] private int currentAmount = 0;

    [SerializeField] private int goalAmount;
    [SerializeField] private bool goal1Reached;
    [SerializeField] private bool goal2Reached;

    [SerializeField] private GameObject barrier;


    private void Awake()
    {
        barrier = GameObject.Find("Room 1 Barrier");
        goal1Reached = goal2Reached = false;
        goalAmount = goalAmount1;
        UpdateUI();
    }

    public void AddAmount(int amount)
    {
        currentAmount += amount;

        if (currentAmount >= goalAmount)
        {
            if (!goal1Reached)
            {
                goal1Reached = true;
                currentAmount = 0;
                goalAmount = goalAmount2;
                barrier.SetActive(false);
            }

            else
            {
                goal2Reached = true;
            }            
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (goalText != null)
        {
            goalText.text = $"{currentAmount}$ / {goalAmount}$";
        }
    }
}
