using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioHandler : MonoBehaviour
{
    public AudioClip[] audios;

    private AudioSource aso;
    private void Start()
    {
        aso = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect()
    {
        if (audios.Length < 1)
        {
            return;
        }
        aso.clip = audios[Random.Range(0, audios.Length)];
        aso.Play();
    }
}
