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
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        if (exiting == false)
        {
            if (Input.GetKeyDown(pb.pc.grab))
            {
                if (pb.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HorseIdle")
                {
                    exiting = true;
                    pb.anim.applyRootMotion = true;
                    pb.anim.SetTrigger("LetGo");
                    pb.mono.StartCoroutine(Exiting(pb));
                }
            }
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
