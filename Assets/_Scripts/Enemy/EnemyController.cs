using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : Actor
{
    public float runDistance = 5;
    public float attackDistance = 2f;
    public float lookDistance = 5;
    public float hearDistance = 2;
    [Space]
    [Range(0, 180)] public int viewAngle = 70;
    [Range(0, 2)] public float minThinkTime;
    [Range(0, 5)] public float maxThinkTime;
    [Space]
    public EnemyStates currentState;
    public enum EnemyStates { normal, chasing, inCombat }
    public Transform dropWeapon;

    [Header("Private/Dont Assign")]
    public Transform player;
    public Animator anim;
    public bool attackCooldown;
    public bool walkBehaviour;
    public bool rotateCooldown;
    public float randomInt;
    public float distancePlayer;
    public NavMeshAgent agent;

    private PlayerBehaviour pb;
    private BloodFXHandler bfh;
    private Target t;

    void Start()
    {
        pb = FindObjectOfType<PlayerBehaviour>();
        player = pb.transform;
        anim = GetComponent<Animator>();
        bfh = GetComponent<BloodFXHandler>();
        agent = GetComponent<NavMeshAgent>();
        t = GetComponent<Target>();

        SwitchState(EnemyStates.normal);
    }
    
    void Update()
    {
        if (dead)
        {
            return;
        }
        distancePlayer = Vector3.Distance(transform.position, player.position);
        switch (currentState)
        {
            case EnemyStates.normal:
                Patrol();
                break;
            case EnemyStates.chasing:
                InChase();
                break;
            case EnemyStates.inCombat:
                InCombat();
                break;
            default:
                break;
        }
    }

    public override void TakeDamage(int damage, int damageType)
    {
        base.TakeDamage(damage, damageType);
        bfh.SlashDamage(damageType);
        anim.Play("Damaged");
    }

    public override void Death(int damageType)
    {
        TargetingSystem ts = FindObjectOfType<TargetingSystem>();
        OrbitCamera oc = FindObjectOfType<OrbitCamera>();

        anim.SetBool("Dead", true);

        anim.SetInteger("RollType", Random.Range(1, 4));
        anim.SetTrigger("Death");

        StopCoroutine("WalkDirection");
        StopCoroutine("AttackCld");

        bfh.DeathBleed(damageType);

        GetComponent<CharacterController>().enabled = false;

        dropWeapon.parent = null;
        dropWeapon.GetComponent<Rigidbody>().isKinematic = false;
        dropWeapon.GetComponent<Collider>().enabled = true;

        pb.KilledTarget(t);
    }

    void SwitchState(EnemyStates es)
    {
        currentState = es;
        anim.SetInteger("WalkingDir", 0);
        switch (currentState)
        {
            case EnemyStates.normal:
                anim.applyRootMotion = false;
                agent.enabled = true;

                anim.SetBool("inCombat", false);
                
                StopCoroutine("WalkDirection");
                break;

            case EnemyStates.chasing:
                anim.applyRootMotion = false;
                agent.enabled = true;

                anim.SetBool("inCombat", false);

                StopCoroutine("WalkDirection");
                break;

            case EnemyStates.inCombat:
                anim.applyRootMotion = true;
                agent.enabled = false;

                anim.SetBool("inCombat", true);
                anim.SetBool("Walking", true);
                anim.SetInteger("WalkingDir", 1);
                
                walkBehaviour = false;
                StartCoroutine("WalkDirection");
                break;

            default:
                break;
        }
    }

    void Patrol()
    {
        float angel = Vector3.Angle(transform.forward, player.position - transform.position);
        if (angel <= viewAngle)
        {
            RaycastHit hit;
            Vector3 dir = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position + Vector3.up * 1.2f, dir);
            Debug.DrawRay(transform.position + Vector3.up * 1.2f, dir * lookDistance, Color.black);
            if (Physics.Raycast(ray, out hit, lookDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("SeePlayer");
                    SwitchState(EnemyStates.chasing);
                }
            }
        }
    }

    void InChase()
    {
        agent.SetDestination(player.position);
        if (distancePlayer < runDistance)
        {
            SwitchState(EnemyStates.inCombat);
        }
        else if (distancePlayer > runDistance + 2)
        {
            agent.speed = 3;
            anim.SetBool("Walking", false);
            anim.SetBool("Running", true);
        }
        else if (distancePlayer > runDistance - 1)
        {
            agent.speed = 1;
            anim.SetBool("Running", false);
            anim.SetBool("Walking", true);
        }
    }

    #region Combat

    void InCombat()
    {
        if (rotateCooldown == false)
        {
            RotateTowardsPlayer();
        }
        if (distancePlayer > (runDistance + 0.25f))
        {
            if (attackCooldown == false)
            {
                SwitchState(EnemyStates.chasing);
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
        StartCoroutine("AttackCld");
    }

    IEnumerator AttackCld()
    {
        attackCooldown = true;
        rotateCooldown = true;
        yield return new WaitForSeconds(1.2f);
        rotateCooldown = false;
        yield return new WaitForSeconds(Random.Range(minThinkTime, maxThinkTime));
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

        randomInt = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(randomInt);

        anim.SetBool("Walking", false);

        randomInt = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(randomInt);

        int randomWalkDir = Random.Range(1, 4);
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

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, hearDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 1.2f, transform.forward * lookDistance);
        
        Gizmos.DrawRay(transform.position + Vector3.up * 1.2f, (transform.forward + ((transform.right * ((float)viewAngle / 100)) * 4)).normalized * lookDistance);
        Gizmos.DrawRay(transform.position + Vector3.up * 1.2f, (transform.forward + ((-transform.right * ((float)viewAngle / 100)) * 4)).normalized * lookDistance);
    }
}
