using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRiding : State
{
    bool exiting;
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        exiting = false;
        pb.closestHorse.Mount(pb.transform);
        pb.cc.enabled = false;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.airtime = 0;
        pb.anim.applyRootMotion = true;
        pb.cc.enabled = true;
        
        pb.currentHorse = null;
        pb.closestHorse = null;
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        if (exiting == false)
        {
            if (Input.GetKeyDown(pb.pc.grab))
            {
                if (pb.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HorseIdle")
                {
                    pb.currentHorse.Unmounted();
                    exiting = true;
                    pb.anim.applyRootMotion = true;
                    pb.anim.SetTrigger("UnMount");
                    pb.mono.StartCoroutine(Exiting(pb));
                }
            }
        }
        switch (pb.currentHorse.currentHorseState)
        {
            case HorseBehaviour.horseState.walking:
                pb.anim.SetBool("Walking", false);
                pb.anim.SetBool("Sprinting", false);
                break;
            case HorseBehaviour.horseState.back:
                pb.anim.SetBool("Walking", true);
                pb.anim.SetBool("Sprinting", false);
                break;
            case HorseBehaviour.horseState.sprinting:
                pb.anim.SetBool("Walking", false);
                pb.anim.SetBool("Sprinting", true);
                break;
            case HorseBehaviour.horseState.dead:
                pb.anim.SetBool("Walking", false);
                pb.anim.SetBool("Sprinting", false);
                break;
            default:
                break;
        }
    }

    IEnumerator Exiting(PlayerBehaviour pb)
    {
        yield return new WaitForSeconds(0.8f);
        pb.cc.enabled = true;
        pb.transform.parent = null;
        pb.stateMachine.GoToState(pb, "Locomotion");
    }
}
