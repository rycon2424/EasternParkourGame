using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRiding : State
{
    bool exiting;
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        pb.anim.SetInteger("DisMountType", 0);

        exiting = false;
        pb.closestHorse.Mount(pb.transform);
        pb.cc.enabled = false;
        
        startTime = Time.time + 1;
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.anim.SetInteger("DisMountType", 0);

        startTime = Time.time;

        pb.airtime = 0;
        pb.anim.applyRootMotion = true;
        pb.cc.enabled = true;

        pb.currentHorse.ResetCircle();
        pb.currentHorse = null;
        pb.closestHorse = null;
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        if (exiting == false)
        {
            UnMountHorse(pb);
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

    float timeHeld;
    float startTime;
    void UnMountHorse(PlayerBehaviour pb)
    {
        if (pb.anim.GetCurrentAnimatorClipInfo(0)[0].clip.name == "HorseIdle")
        {
            if (Input.GetKeyDown(pb.pc.grab))
            {
                startTime = Time.time;
            }
            if (Input.GetKey(pb.pc.grab))
            {
                pb.currentHorse.ECanvas.SetActive(true);
                timeHeld = Time.time - startTime;
                if (timeHeld > 1f)
                {
                    //Debug.Log("On horse held time : " + timeHeld);
                    if (pb.RayHit(pb.transform.position, pb.transform.right, 1.25f, pb.everything) == false)
                    {
                        pb.anim.SetInteger("DisMountType", 1);
                        pb.mono.StartCoroutine(Exiting(pb));
                    }
                    if (pb.RayHit(pb.transform.position, -pb.transform.right, 1.25f, pb.everything) == false)
                    {
                        pb.anim.SetInteger("DisMountType", -1);
                        pb.mono.StartCoroutine(Exiting(pb));
                    }
                }
                else
                {
                    pb.currentHorse.FillCircle(timeHeld);
                }
            }
            if (Input.GetKeyUp(pb.pc.grab))
            {
                startTime = 0;
                pb.currentHorse.ECanvas.SetActive(false);
                pb.currentHorse.ResetCircle();
            }
        }
        else
        {
            pb.currentHorse.ECanvas.SetActive(false);
            pb.currentHorse.ResetCircle();
        }
    }
    
    IEnumerator Exiting(PlayerBehaviour pb)
    {
        pb.currentHorse.Unmounted();
        exiting = true;
        pb.anim.applyRootMotion = true;
        pb.anim.SetTrigger("UnMount");
        yield return new WaitForSeconds(0.8f);
        pb.cc.enabled = true;
        pb.transform.parent = null;
        pb.stateMachine.GoToState(pb, "Locomotion");
    }
}
