using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestNote : MonoBehaviour
{
    [SerializeField] Text questT;
    [SerializeField] Text questD;
    [SerializeField] Text questR;
    [Space]
    [SerializeField] QuestBoard board;
    [SerializeField] bool hovering;
    [SerializeField] bool reading;

    public GameObject highlight;
    public bool selectable;

    private Vector3 beginPos;

    private void Start()
    {
        beginPos = transform.localPosition;
    }

    public void CreateQuest(string questTitle, string textDescription, string questReward)
    {
        questT.text = questTitle;
        questD.text = textDescription;
        questR.text = questReward + " $";
    }

    private void Update()
    {
        if (hovering && reading == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                reading = true;
                board.ReadingQuest(this);
                StartCoroutine(LerpToPos(new Vector3(0, 1.35f, 0.8f), 1f, true));
            }
        }
        else if (hovering && reading == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                reading = false;
                board.EnableQuests();
                StartCoroutine(LerpToPos(beginPos, 1f, false));
            }
        }
    }

    public void ResetPos()
    {
        StartCoroutine(LerpToPos(beginPos, 1f, false));
    }
    
    IEnumerator LerpToPos(Vector3 pos, float lerpTime, bool showStart)
    {
        if (showStart == false)
        {
            board.startQuestButton.SetActive(false);
        }
        Vector3 startPos = transform.localPosition;

        for (float t = 0; t < 1; t += Time.deltaTime / lerpTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, pos, t);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = pos;
        if (showStart)
        {
            board.startQuestButton.SetActive(true);
        }
    }

    void OnMouseOver()
    {
        if (selectable)
        {
            hovering = true;
            highlight.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (selectable)
        {
            hovering = false;
            highlight.SetActive(false);
        }
    }
}
