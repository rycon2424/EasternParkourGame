using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNote : MonoBehaviour
{
    public QuestBoard board;
    public GameObject highlight;
    public bool selectable;
    public bool hovering;
    public bool reading;

    private Vector3 beginPos;

    private void Start()
    {
        beginPos = transform.localPosition;
    }

    private void Update()
    {
        if (hovering && reading == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                reading = true;
                board.ReadingQuest(this);
                StartCoroutine(LerpToPos(new Vector3(0, 1.35f, 0.8f), 1f));
            }
        }
        else if (hovering && reading == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                reading = false;
                board.EnableQuests();
                StartCoroutine(LerpToPos(beginPos, 1f));
            }
        }
    }
    
    IEnumerator LerpToPos(Vector3 pos, float lerpTime)
    {
        Vector3 startPos = transform.localPosition;

        for (float t = 0; t < 1; t += Time.deltaTime / lerpTime)
        {
            transform.localPosition = Vector3.Lerp(startPos, pos, t);
            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = pos;
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
