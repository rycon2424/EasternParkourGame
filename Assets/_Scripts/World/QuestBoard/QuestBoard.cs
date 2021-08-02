using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    private bool playerInRange;
    private Animator anim;
    private PlayerBehaviour pb;
    
    public QuestInfo[] allQuests;
    [Space]
    public GameObject[] questNotes;
    public bool viewingBoard;
    public GameObject exitUI;
    public GameObject startQuestButton;
    public GameObject cam;
    public GameObject EButton;
    [Space]
    public List<QuestNote> quests = new List<QuestNote>();

    void Start()
    {
        anim = GetComponent<Animator>();
        pb = FindObjectOfType<PlayerBehaviour>();

        for (int i = 0; i < allQuests.Length; i++)
        {
            questNotes[i].SetActive(true);
            quests.Add(allQuests[i].noteOwner);
        }
    }
    
    void Update()
    {
        if (playerInRange && viewingBoard == false)
        {
            if (PauseSystem.instance.paused == false)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PauseSystem.instance.blockPausing = true;
                    pb.CameraOn(false);
                    viewingBoard = true;
                    cam.SetActive(true);
                    pb.locked = true;
                    anim.Play("Cutscene");
                }
            }
        }
    }

    public void EndCutscene()
    {
        exitUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        foreach (var q in quests)
        {
            q.selectable = true;
        }
    }

    public void LeaveBoard()
    {
        cam.SetActive(false);
        PauseSystem.instance.blockPausing = false;
        viewingBoard = false;
        exitUI.SetActive(false);
        pb.CameraOn(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        foreach (var q in quests)
        {
            q.selectable = false;
            q.highlight.SetActive(false);
            q.ResetPos();
        }
        pb.locked = false;
    }

    public void EnableQuests()
    {
        foreach (var q in quests)
        {
            q.selectable = true;
        }
    }

    public void ReadingQuest(QuestNote qn)
    {
        foreach (var q in quests)
        {
            q.selectable = false;
            q.highlight.SetActive(false);
        }
        qn.selectable = true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            EButton.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            EButton.SetActive(false);
            playerInRange = false;
        }
    }
}

[System.Serializable]
public class QuestInfo
{
    public QuestNote noteOwner;
    public bool random;

    [Header("Quest Info")]
    public string questTitle;
    [TextArea(5, 5)] public string textDescription;
    public string questReward;

}

