using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAir : State
{
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.cc.enabled = true;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.grabTrail.SetActive(false);
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        pb.Grounded();
        if (pb.IsInAnimation("Fall") || pb.IsInAnimation("Jump"))
        {
            if (Input.GetKey(pb.pc.grab))
            {
                pb.grabTrail.SetActive(true);
                pb.LedgeInfo();
            }
            if (!pb.RayHit(pb.transform.position + pb.transform.up, pb.transform.forward, 1, pb.everything))
            {
                pb.cc.Move(pb.transform.forward * Time.deltaTime * 3);
            }
        }
        else
        {
            pb.grabTrail.SetActive(false);
        }
        if (pb.CheckTag(Vector3.zero) == "Slide")
        {
            pb.stateMachine.GoToState(pb, "Sliding");
        }
        if (pb.grounded && pb.ccGrounded)
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
        else if (Input.GetKeyDown(pb.pc.jump))
        {
            pb.anim.SetTrigger("Jump");
        }
    }
}
