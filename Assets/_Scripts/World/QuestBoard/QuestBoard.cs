using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBoard : MonoBehaviour
{
    private bool playerInRange;
    private Animator anim;
    private PlayerBehaviour pb;

    public bool viewingBoard;
    public GameObject startExitUI;
    public GameObject cam;
    public GameObject EButton;
    [Space]
    public QuestNote[] quests;

    void Start()
    {
        anim = GetComponent<Animator>();
        pb = FindObjectOfType<PlayerBehaviour>();
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
        startExitUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        foreach (var q in quests)
        {
            q.selectable = true;
        }
    }

    void LeaveBoard()
    {
        cam.SetActive(false);
        PauseSystem.instance.blockPausing = false;
        startExitUI.SetActive(false);
        pb.CameraOn(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        foreach (var q in quests)
        {
            q.selectable = false;
            q.highlight.SetActive(false);
        }
        pb.locked = false;
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
