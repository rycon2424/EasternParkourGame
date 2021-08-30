using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    public AudioSource aso;
    public AudioClip[] footsteps;

    public void FootStep()
    {
        aso.clip = footsteps[Random.Range(0, footsteps.Length)];
        aso.volume = Random.Range(0.5f, 1f);
        aso.Play();
    }
}
