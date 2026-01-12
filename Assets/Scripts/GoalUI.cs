using UnityEngine;
using TMPro; // folosim TextMeshPro, dar poți folosi și UnityEngine.UI.Text

public class GoalUI : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private TMP_Text goalText; // Textul din canvas
    [SerializeField] private int goalAmount = 10000;

    private int currentAmount = 0;

    private void Awake()
    {
        UpdateUI();
    }

    public void AddAmount(int amount)
    {
        currentAmount += amount;
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
