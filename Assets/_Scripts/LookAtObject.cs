using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtObject : MonoBehaviour
{
    public Transform objectToLookAt;
    public Vector3 offset;

    private void Start()
    {
        if (objectToLookAt == null)
        {
            objectToLookAt = FindObjectOfType<OrbitCamera>().transform;
        }
    }

    void Update()
    {
        transform.LookAt(objectToLookAt, offset);
    }
}
