using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNote : MonoBehaviour
{
    public GameObject highlight;
    public bool selectable;

    void OnMouseOver()
    {
        if (selectable)
        {
            highlight.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (selectable)
        {
            highlight.SetActive(false);
        }
    }
}
