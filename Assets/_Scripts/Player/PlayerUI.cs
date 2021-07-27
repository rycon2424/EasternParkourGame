using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Image hpBar;

    public void UpdateHPBar(int health)
    {
        float hpFloat = (float)health / 100;
        hpBar.fillAmount = hpFloat;
    }
}
