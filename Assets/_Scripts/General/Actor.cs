using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public int health = 100;
    public bool dead = false;
    public int damage = 20;

    public virtual void TakeDamage(int damage, int damageType)
    {
        if (dead == true)
        {
            return;
        }

        health -= damage;

        if (health < 1)
        {
            dead = true;
            health = 0;
            Death(damageType);
        }
    }

    public virtual void Death(int damageType) { }

    public virtual void KilledTarget() { }
}
