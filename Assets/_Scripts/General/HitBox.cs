using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public List<Actor> targets = new List<Actor>();
    private Actor ac;

    private void Start()
    {
        ac = GetComponent<Actor>();
    }

    public void DealDamage(int damageType)
    {
        foreach (var target in targets)
        {
            target.TakeDamage(ac.damage, damageType);
        }
    }
}