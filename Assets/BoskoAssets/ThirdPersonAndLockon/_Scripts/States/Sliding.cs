using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : State
{
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.anim.applyRootMotion = false;
        pb.transform.rotation = Quaternion.LookRotation(pb.CalculateSlopeDirection());

        pb.anim.SetTrigger("Slide");
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.anim.SetTrigger("LetGo");

        pb.transform.rotation = new Quaternion(0, 1, 0, 0.1f);

        pb.anim.applyRootMotion = true;
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        float x = Input.GetAxis(pb.pc.inputHorizontal);
        pb.anim.SetFloat("x", x);

        pb.cc.Move((pb.transform.forward * pb.slideSpeed + pb.transform.right * x * 2) * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
    }

    void ExitCheck(PlayerBehaviour pb)
    {
        //Check if no ground
        //Check if ground is not tagged "Slide"
    }

}
