using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveQuest
{
    public string questName;
    [Space]
    public List<QuestObjective> objectives;
    public enum ObjectiveType { Non, kill, collect, unseen}
}

[System.Serializable]
public class QuestObjective
{
    public string objective;
    public int objCount;
    public ActiveQuest.ObjectiveType typeObj;
    public bool finishedObj;
}
