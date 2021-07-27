using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFXHandler : MonoBehaviour
{
    [SerializeField] bool canBleed = true;
    [Header("FX")]
    [SerializeField] ParticleSystem slashHigh;
    [SerializeField] ParticleSystem slashMid;
    [SerializeField] ParticleSystem slashLow;
    [Space]
    [SerializeField] ParticleSystem deathHigh;
    [SerializeField] ParticleSystem deathMid;

    public void SlashDamage(int type)
    {
        if (canBleed == false)
        {
            return;
        }
        switch (type)
        {
            case 1:
                slashLow.Play();
                break;
            case 2:
                slashMid.Play();
                break;
            case 3:
                slashHigh.Play();
                break;
            default:
                Debug.LogError("An attack has wrong parameters on animation event");
                break;
        }
    }

    public void DeathBleed(int type)
    {
        if (canBleed == false)
        {
            return;
        }
        switch (type)
        {
            case 2:
                deathMid.Play();
                break;
            case 3:
                deathHigh.Play();
                break;
            default:
                break;
        }
    }
}
