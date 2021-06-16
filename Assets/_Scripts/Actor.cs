using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int health = 100;
    public bool dead = false;

    public virtual void TakeDamage(int damage, int damageType)
    {
        if (dead == true)
        {
            return;
        }

        health -= damage;

        if (health < 1)
        {
            Death();
        }
    }

    public virtual void Death() { }
}
