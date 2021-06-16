using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : State
{
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.anim.applyRootMotion = false;
        Vector3 slopeDir = pb.CalculateSlopeDirection();

        if (slopeDir == Vector3.zero)
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
            return;
        }

        pb.transform.rotation = Quaternion.LookRotation(slopeDir);

        pb.RayHit(pb.transform.position + pb.transform.up, Vector3.down, 1.5f, pb.everything);

        pb.transform.position = pb.lastCachedhit;

        pb.anim.SetBool("isSliding", true);
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.anim.SetBool("isSliding", false);

        pb.transform.rotation = new Quaternion(0, 1, 0, 0.1f);

        pb.anim.applyRootMotion = true;
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        float x = Input.GetAxis(pb.pc.inputHorizontal);
        pb.anim.SetFloat("x", x);

        pb.cc.Move((pb.transform.forward * pb.slideSpeed + pb.transform.right * x * 2) * Time.deltaTime);

        ExitCheck(pb);
    }

    void ExitCheck(PlayerBehaviour pb)
    {
        if (pb.CheckTag(Vector3.zero) != "Slide")
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
    }

}
