using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : State
{
    public override void OnStateEnter(PlayerBehaviour pb)
    {
        if (pb.anim.GetBool("Armed"))
        {
            pb.damage = pb.weaponDamage;
        }
        else
        {
            pb.damage = pb.unArmedDamage;
        }
        pb.anim.SetBool("Target", true);
        pb.lockedOn = true;
        pb.lol.gameObject.SetActive(true);
        pb.lol.target = pb.ts.currentTarget;
        pb.combatUIVisual.SetActive(true);
        ResetAttack(pb);
    }

    public override void OnStateExit(PlayerBehaviour pb)
    {
        pb.parrying = false;
        pb.lockedOn = false;
        pb.anim.SetBool("Target", false);
        pb.oc.ChangeCamState(OrbitCamera.CamState.onPlayer);
        pb.ts.StopTargeting();
        pb.lol.gameObject.SetActive(false);
        
        ResetAttack(pb);

        pb.combatUIVisual.SetActive(false);
    }

    public override void StateUpdate(PlayerBehaviour pb)
    {
        Movement(pb);
        pb.RotateTowardsCamera();
        Dodge(pb);
        Targeting(pb);
        CombatUI(pb);
        Attacking(pb);

        if (Input.GetKeyDown(pb.pc.target))
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
    }

    void Attacking(PlayerBehaviour pb)
    {
        if (pb.anim.GetInteger("AttackType") > 0)
        {
            if (Input.GetMouseButtonDown(pb.pc.mouseClickAttack))
            {
                pb.anim.SetTrigger("Attack");
            }
        }
        if (Input.GetMouseButtonDown(pb.pc.mouseClickParry))
        {
            ResetAttack(pb);
            pb.anim.SetTrigger("Parry");
        }
        if (Input.GetKeyDown(pb.pc.crouch))
        {
            ResetAttack(pb);
            pb.anim.SetTrigger("Kick");
        }
    }

    void Targeting(PlayerBehaviour pb)
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            pb.ts.SwitchTarget(pb.oc);
            pb.lol.target = pb.ts.currentTarget;
        }
        if (Vector3.Distance(pb.transform.position, pb.ts.currentTarget.transform.position) > pb.ts.loseTargetRange)
        {
            pb.stateMachine.GoToState(pb, "Locomotion");
        }
    }

    void Dodge(PlayerBehaviour pb)
    {
        if (Input.GetKeyDown(pb.pc.jump))
        {
            pb.anim.SetTrigger("Jump");

            ResetAttack(pb);
        }
    }
    
    void CombatUI(PlayerBehaviour pb)
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (Mathf.Abs(mouseX) > 0.5f || Mathf.Abs(mouseY) > 0.5f)
        {
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                pb.combatUI.SetFloat("mouseY", 0);
                if (mouseX > 0)
                {
                    pb.combatUI.SetFloat("mouseX", 1);
                    pb.anim.SetInteger("AttackType", 2);
                }
                else
                {
                    pb.combatUI.SetFloat("mouseX", -1);
                    pb.anim.SetInteger("AttackType", 4);
                }
                return;
            }
            pb.combatUI.SetFloat("mouseX", 0);
            if (mouseY > 0)
            {
                pb.combatUI.SetFloat("mouseY", 1);
                pb.anim.SetInteger("AttackType", 1);
            }
            else
            {
                pb.combatUI.SetFloat("mouseY", -1);
                pb.anim.SetInteger("AttackType", 3);
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

    }

    void ResetAttack(PlayerBehaviour pb)
    {
        pb.anim.ResetTrigger("Attack");
        pb.anim.SetInteger("AttackType", 0);

        pb.combatUI.SetFloat("mouseX", 0);
        pb.combatUI.SetFloat("mouseY", 0);
    }
}
