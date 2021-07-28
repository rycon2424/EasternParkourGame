using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : State
{
    float defaultCCheight;

    public override void AnimatorIKUpdate(PlayerBehaviour pb)
    {

    }

    public override void OnStateEnter(PlayerBehaviour pb)
    {
        defaultCCheight = pb.cc.height;
        startTime = Time.time;

        pb.anim.ResetTrigger("Jump");
        pb.anim.applyRootMotion = true;
        pb.cc.enabled = true;
        pb.airtime = 0;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        startTime = Time.time;
        pb.cc.center = pb.defaultCenterCc;
        pb.cc.height = defaultCCheight;
        pb.crouched = false;
        pb.anim.SetBool("Crouch", false);
    }

    float turnSmoothVelocity;
    public override void StateUpdate(PlayerBehaviour pb)
    {
        if (pb.Grounded())
        {
            pb.failedClimb = false;
            pb.anim.ResetTrigger("fall");
            if (Input.GetKeyDown(pb.pc.crouch))
            {
                Crouch(!pb.crouched, pb);
            }
            HorseSystem(pb);
        }
        else
        {
            Crouch(false, pb);
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
        pb.crouched = pb.anim.GetBool("Crouch");
    }

    float startTime;
    float timeHeld;
    void HorseSystem(PlayerBehaviour pb)
    {
        if (pb.closestHorse != null)
        {
            if (Input.GetKeyDown(pb.pc.grab))
            {
                startTime = Time.time;
            }
            if (Input.GetKey(pb.pc.grab))
            {
                timeHeld = Time.time - startTime;
                if (timeHeld > 1f)
                {
                    //Debug.Log("On Foot held time : " + timeHeld);
                    pb.stateMachine.GoToState(pb, "HorseRiding");
                }
                else
                {
                    pb.closestHorse.FillCircle(timeHeld);
                }
            }
            if (Input.GetKeyUp(pb.pc.grab))
            {
                startTime = 0;
                pb.closestHorse.ResetCircle();
            }
        }
        else
        {
            startTime = Time.time;
            timeHeld = 0;
        }
    }

    void Crouch(bool crouching, PlayerBehaviour pb)
    {
        if (crouching)
        {
            pb.cc.center = new Vector3(0, 0.7f, 0);
            pb.cc.height = 1.2f;
        }
        else
        {
            if (pb.RayHit(pb.transform.position + pb.transform.up, pb.transform.up, 1, pb.everything))
            {
                return;
            }
            pb.cc.center = pb.defaultCenterCc;
            pb.cc.height = defaultCCheight;
        }
        pb.anim.SetBool("Crouch", crouching);
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
        if (pb.grounded == false && pb.failedClimb == false)
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
        if (pb.anim.GetBool("Walking"))
        {
            if (Input.GetKey(pb.pc.sprint))
            {
                pb.anim.SetBool("Sprinting", true);
            }
        }
        else
        {
            pb.anim.SetBool("Sprinting", false);
        }
        if (Input.GetKeyUp(pb.pc.sprint))
        {
            pb.anim.SetBool("Sprinting", false);
        }
        //Jump
        if (pb.overWeight == false)
        {
            if (Input.GetKeyDown(pb.pc.jump))
            {
                pb.anim.SetTrigger("Jump");
                pb.stateMachine.GoToState(pb, "InAir");
            }
        }
    }
}
