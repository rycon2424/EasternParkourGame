using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddToHitBox : MonoBehaviour
{
    [Header("ASSIGN")]
    public HitBox hb;

    private void OnTriggerEnter(Collider other)
    {
        Actor ac = other.GetComponent<Actor>();
        if (ac != null)
        {
            hb.targets.Add(ac);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Actor ac = other.GetComponent<Actor>();
        if (ac != null)
        {
            hb.targets.Remove(ac);
        }
    }
}