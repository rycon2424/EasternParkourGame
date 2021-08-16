using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public ActiveQuest currentActiveQuest;
    [Space]
    [SerializeField] Text questNT;
    [SerializeField] Text questO1T;
    [SerializeField] Text questO2T;
    [SerializeField] Text questO3T;
    
    public static QuestManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public void ResetQuestTab()
    {
        questNT.text = "No Quest";
        questO1T.text = "Currently following no quest";
        questO1T.text = "";
        questO1T.text = "";
    }

    public void UpdateQuestObjective()
    {
        
    }
    
    public void UpdateQuestText()
    {
        questNT.text = currentActiveQuest.questName;
        questO1T.text = currentActiveQuest.objectives[0].objective;
        questO1T.text = currentActiveQuest.objectives[1].objective;
        questO1T.text = currentActiveQuest.objectives[2].objective;
    }

}