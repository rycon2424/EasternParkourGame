using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int health = 100;
    public bool dead = false;
    public int damage = 20;
    public bool parrying;

    public virtual void TakeDamage(int damage, int damageType, int attackDir)
    {
        if (dead == true)
        {
            return;
        }
    }

    public virtual void Death(int damageType) { }
}
