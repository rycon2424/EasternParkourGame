﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : State
{

    public override void AnimatorIKUpdate(PlayerBehaviour pb)
    {

    }

    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.anim.ResetTrigger("Jump");
        pb.anim.applyRootMotion = true;
        pb.cc.enabled = true;
        pb.airtime = 0;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {

    }

    float turnSmoothVelocity;

    public override void StateUpdate(PlayerBehaviour pb)
    {
        if (pb.Grounded())
        {
            pb.anim.ResetTrigger("fall");
            CheckSlope(pb);
        }
        if (!pb.lockedOn)
        {
            RotateToCam(pb);
        }
        GrabLedge(pb);
        Movement(pb);
        CanTarget(pb);
        if (pb.CheckTag(Vector3.zero) == "Slide")
        {
            pb.stateMachine.GoToState(pb, "Sliding");
        }
        if (pb.airtime > 0.75f)
        {
            pb.anim.SetTrigger("fall");
            pb.stateMachine.GoToState(pb, "InAir");
        }
    }

    void CheckSlope(PlayerBehaviour pb)
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            pb.stateMachine.GoToState(pb, "Sliding");
        }
    }

    void CanTarget(PlayerBehaviour pb)
    {
        if (Input.GetKeyDown(pb.pc.target))
        {
            if (pb.ts.SelectTarget(pb.oc))
            {
                pb.stateMachine.GoToState(pb, "Combat");
            }
        }
    }

    void RotateToCam(PlayerBehaviour pb)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0f, y).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + pb.oc.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(pb.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, 0.1f);
            pb.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

    }
    
    void GrabLedge(PlayerBehaviour pb)
    {
        if (pb.grounded == false)
        {
            if (Input.GetKey(pb.pc.grab))
            {
                pb.LedgeInfo();
            }
        }
    }

    void Movement(PlayerBehaviour pb)
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        pb.anim.SetFloat("x", x);
        pb.anim.SetFloat("y", y);
        pb.anim.SetFloat("y+x", (Mathf.Abs(x) + Mathf.Abs(y)));

        //Walking
        if (x != 0 || y != 0)
        {
            pb.anim.SetBool("Walking", true);
        }
        else
        {
            pb.anim.SetBool("Walking", false);
        }
        //Sprinting
        if (Input.GetKey(pb.pc.sprint))
        {
            pb.anim.SetBool("Sprinting", true);
        }
        else
        {
            pb.anim.SetBool("Sprinting", false);
        }
        //Jump
        if (Input.GetKeyDown(pb.pc.jump))
        {
            pb.anim.SetTrigger("Jump");
            pb.stateMachine.GoToState(pb, "InAir");
        }
    }
}