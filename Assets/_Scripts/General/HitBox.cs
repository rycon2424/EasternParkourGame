using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour
{
    public List<Actor> targets = new List<Actor>();
    public UnityEvent GotParried;
    [Space]
    public bool ignoreEnemyController;
    public bool ignorePlayerController;
    private Actor ac;

    private void Start()
    {
        ac = GetComponent<Actor>();
    }

    public void DealDamage(AnimationEvent @event)
    {
        foreach (var target in targets)
        {
            if (target != ac)
            {
                if (ignoreEnemyController)
                {
                    if (target is EnemyController)
                    {
                        return;
                    }
                }
                if (ignorePlayerController)
                {
                    if (target is PlayerBehaviour)
                    {
                        return;
                    }
                }
                if (target.parrying == false)
                {
                    target.TakeDamage(ac.damage, @event.intParameter, (int)@event.floatParameter);
                    break;
                }
                else
                {
                    GotParried.Invoke();
                    break;
                }
            }
        }
    }
}