using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float runDistance = 5;
    public float attackDistance = 2f;

    public enum EnemyStates {normal, chasing, inCombat }
    public EnemyStates currentState;

    [Header("Private/Dont Assign")]
    public Transform player;
    public Animator anim;
    public bool attackCooldown;
    public bool walkBehaviour;
    public bool rotateCooldown;
    public float randomInt;
    public float distancePlayer;

    void Start()
    {
        player = FindObjectOfType<PlayerBehaviour>().transform;
        anim = GetComponent<Animator>();
        SwitchState(EnemyStates.inCombat);
    }
    
    void Update()
    {
        distancePlayer = Vector3.Distance(transform.position, player.position);
        switch (currentState)
        {
            case EnemyStates.normal:
                break;
            case EnemyStates.chasing:
                break;
            case EnemyStates.inCombat:
                InCombat();
                break;
            default:
                break;
        }
    }

    void SwitchState(EnemyStates es)
    {
        currentState = es;
        switch (currentState)
        {
            case EnemyStates.normal:
                break;
            case EnemyStates.chasing:
                break;
            case EnemyStates.inCombat:
                StartCoroutine("WalkDirection");
                break;
            default:
                break;
        }
    }

    void InCombat()
    {
        if (rotateCooldown == false)
        {
            RotateTowardsPlayer();
        }
        if (distancePlayer > runDistance)
        {
            anim.SetBool("Walking", true);
            anim.SetInteger("WalkingDir", 1);
            if (walkBehaviour == true)
            {
                StopCoroutine("WalkDirection");
                walkBehaviour = false;
            }
        }
        else if (distancePlayer > attackDistance)
        {
            StartWalking();
        }
        else
        {
            if (attackCooldown == false)
            {
                Attack();
            }
            StopWalking();
        }
    }
    
    void Attack()
    {
        anim.SetInteger("AttackType", Random.Range(1, 6));
        anim.SetTrigger("Attack");
        StartCoroutine(AttackCld());
    }

    IEnumerator AttackCld()
    {
        attackCooldown = true;
        rotateCooldown = true;
        yield return new WaitForSeconds(1.4f);
        rotateCooldown = false;
        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        attackCooldown = false;
    }

    void StopWalking()
    {
        if (walkBehaviour == true)
        {
            StopCoroutine("WalkDirection");
            anim.SetBool("Walking", false);
            walkBehaviour = false;
        }
    }

    void StartWalking()
    {
        if (walkBehaviour == false)
        {
            StartCoroutine("WalkDirection");
        }
    }
    
    IEnumerator WalkDirection()
    {
        walkBehaviour = true;

        randomInt = Random.Range(1.0f, 5.0f);
        yield return new WaitForSeconds(randomInt);

        anim.SetBool("Walking", false);

        randomInt = Random.Range(1.0f, 5.0f);
        yield return new WaitForSeconds(randomInt);

        int randomWalkDir = Random.Range(1, 5);
        anim.SetBool("Walking", true);
        anim.SetInteger("WalkingDir", randomWalkDir);
        
        StartCoroutine("WalkDirection");
    }
    
    public void RotateTowardsPlayer()
    {
        Quaternion newLookAt = Quaternion.LookRotation(player.position - transform.position);
        newLookAt.x = 0;
        newLookAt.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, newLookAt, Time.deltaTime * 5);
    }
}
