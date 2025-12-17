using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Checklist : MonoBehaviour
{
    [SerializeField] private List<GoalObject> goalObjects;
    [SerializeField] private TextMeshProUGUI checklistText;
    [SerializeField] private string checkListDefault = "Objects to destroy:";
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public bool finished;
    [HideInInspector] public Transform currentObjectToDestroy;
    void Start()
    {
        goalObjects = new List<GoalObject>(FindObjectsOfType<GoalObject>());
        goalObjects.Sort((a, b) => a.GetOrder().CompareTo(b.GetOrder()));
        currentObjectToDestroy = null;
    }

    // Update is called once per frame
    void Update()
    {
        //if(checklistText!= null)
        //{
        //    checklistText.text = checkListDefault;
        //    foreach (GoalObject goalObject in goalObjects)
        //    {
        //        if (!goalObject.IsMarked())
        //        {
        //            checklistText.text += "\n" + "<color=red>" + goalObject.name + "</color>";
        //        }
        //        else
        //        {
        //            checklistText.text += "\n" + "<color=green>" + goalObject.name + "</color>";
        //        }
        //    }
        //}
        finished = true;
        currentObjectToDestroy = null;
        checklistText.text = checkListDefault;
        foreach (GoalObject goalObject in goalObjects)
        {
            if (!goalObject.IsMarked() )
            {
                if (finished == true)
                    currentObjectToDestroy = goalObject.transform;
                finished = false;

                checklistText.text += "\n" + "<color=red>" + goalObject.name + "</color>";
            }
            else
            {
                checklistText.text += "\n" + "<color=green>" + goalObject.name + "</color>";
            }
        }
    }
}
