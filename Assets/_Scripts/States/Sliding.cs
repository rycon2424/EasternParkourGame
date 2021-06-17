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

        //pb.anim.Play("Sliding");
        pb.anim.SetTrigger("Slide");
        pb.anim.SetBool("isSliding", true);
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.anim.SetBool("isSliding", false);

        Quaternion lookingPos = pb.transform.rotation;
        pb.transform.rotation = new Quaternion(0, lookingPos.y, 0, lookingPos.w);

        pb.anim.applyRootMotion = true;

        pb.anim.ResetTrigger("Slide");
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
