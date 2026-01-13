using UnityEngine;

[RequireComponent(typeof(BreakObject))]
public class BreakableValueMonitor : MonoBehaviour
{
    [Header("Value Settings")]
    public int value = 1000; // cât valorează obiectul când e spart

    private bool counted = false;

    private BreakObject breakObject;
    private Collisions colission;
    private void Awake()
    {
        breakObject = GetComponent<BreakObject>();
        colission = GetComponent<Collisions>();
        if (breakObject == null)
        {
            Debug.LogError("BreakObject lipsește de pe obiect!");
        }
    }


    private void OnEnable()
    {
        counted = false; // resetăm dacă obiectul e reactivat
       
    }

    private void Update()
    {
    }

    // private void OnDisable()
    // {
    //     // Nu facem nimic dacă obiectul nu a fost încă spart
    //     if (counted || breakObject == null) return;

    //     // sliceTarget este dezactivat de BreakObject
    //     if (breakObject.sliceTarget == null || !breakObject.sliceTarget.activeInHierarchy)
    //     {
    //         counted = true;
    //         NotifyGoal();
    //     }
    // }

    public void NotifyGoal()
    {
        counted = true;
        var goal = FindObjectOfType<ValueGoalManager>();
        if (goal != null)
        {
            goal.AddAmount(value);
            Debug.Log($"{gameObject.name} spart! +{value}$");
        }
    }
}
