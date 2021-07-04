using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour
{
    public float heightOffset;
    TargetingSystem pt;

    public UnityEvent OnTarget;
    public UnityEvent OnSwitchTarget;

    void Awake()
    {
        pt = FindObjectOfType<TargetingSystem>();
        pt.targets.Add(this);
    }
}
