using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : State
{
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.anim.SetBool("Target", true);
        pb.lockedOn = true;
        pb.lol.gameObject.SetActive(true);
        pb.lol.target = pb.ts.currentTarget;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.lockedOn = false;
        pb.anim.SetBool("Target", false);
        pb.oc.ChangeCamState(OrbitCamera.CamState.onPlayer);
        pb.ts.currentTarget = null;
        pb.lol.gameObject.SetActive(false);
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        pb.anim.SetFloat("x", x);
        pb.anim.SetFloat("y", y);
        pb.anim.SetFloat("y+x", (Mathf.Abs(x) + Mathf.Abs(y)));

        pb.RotateTowardsCamera();
        if (Input.mouseScrollDelta.y != 0)
        {
            pb.ts.SwitchTarget(pb.oc);
            pb.lol.target = pb.ts.currentTarget;
        }
        if (Vector3.Distance(pb.transform.position, pb.ts.currentTarget.transform.position) > pb.ts.loseTargetRange)
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
        if (Input.GetKeyDown(pb.pc.target))
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
    }
}
