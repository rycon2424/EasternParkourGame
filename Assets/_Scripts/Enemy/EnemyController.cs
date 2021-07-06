using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class EnemyController : Humanoid
{
    public float runDistance = 5;
    public float attackDistance = 2f;
    public float lookDistance = 5;
    public float hearDistance = 2;
    [Space]
    [Range(0, 180)] public int viewAngle = 70;
    [Range(0, 1)] public float stunTime;
    [Range(0, 2)] public float minThinkTime;
    [Range(0, 5)] public float maxThinkTime;
    [Range(0, 20)] public float chaseTime;
    [Space]
    public EnemyStates currentState;
    public enum EnemyStates { normal, chasing, inCombat, dead }
    public GuardStates guardPos;
    public enum GuardStates { TopRight, RightDown ,LeftDown, TopLeft, None}
    public Transform dropWeapon;
    public GameObject fightUI;
    public Animator fightAnim;
    [Header("Patrol settings")]
    public Transform[] patrolPoints;

    [Header("PatrolDebug")]
    [SerializeField] private Mesh debugPos;
    [SerializeField] private Color32 colorDebug;

    [Header("Private/Dont Assign")]
    [SerializeField] private Transform player;
    [SerializeField] private bool targetOfPlayer;
    [SerializeField] private bool attackCooldown;
    [SerializeField] private bool lostPlayer;
    [SerializeField] private bool walkBehaviour;
    [SerializeField] private bool rotateCooldown;
    [SerializeField] private float randomInt;
    [SerializeField] private float distancePlayer;

    private NavMeshAgent agent;
    private Animator anim;
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
        
        fightUI.SetActive(false);

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

    public void GotParried()
    {
        TakeDamage(0, 0, 0);
    }

    public override void TakeDamage(int damage, int damageType, int attackDir)
    {
        base.TakeDamage(damage, damageType, attackDir);
        if (dead == false)
        {
            switch (attackDir)
            {
                case -1: // Force Dodge
                    anim.Play("Dodgeback");
                    return;
                case 0: // Always hits
                    anim.Play("Damaged");
                    break;
                case 1:
                    if (guardPos == GuardStates.TopLeft || guardPos == GuardStates.TopRight)
                    {
                        anim.Play("Dodgeback");
                        return;
                    }
                    else
                    {
                        anim.Play("Damaged");
                    }
                    break;
                case 2:
                    if (guardPos == GuardStates.RightDown || guardPos == GuardStates.TopRight)
                    {
                        anim.Play("Dodgeback");
                        return;
                    }
                    else
                    {
                        anim.Play("Damaged");
                    }
                    break;
                case 3:
                    if (guardPos == GuardStates.RightDown || guardPos == GuardStates.LeftDown)
                    {
                        anim.Play("Dodgeback");
                        return;
                    }
                    else
                    {
                        anim.Play("Damaged");
                    }
                    break;
                case 4:
                    if (guardPos == GuardStates.TopLeft || guardPos == GuardStates.LeftDown)
                    {
                        anim.Play("Dodgeback");
                        return;
                    }
                    else
                    {
                        anim.Play("Damaged");
                    }
                    break;
                default:
                    break;
            }
        }

        RandomizeGuard();

        if (damage > 0)
        {
            bfh.SlashDamage(damageType);
        }

        health -= damage;

        AttackStunned();

        if (health < 1)
        {
            currentState = EnemyStates.dead;
            dead = true;
            health = 0;
            Death(damageType);
        }
    }
    
    public override void Death(int damageType)
    {
        TargetingSystem ts = FindObjectOfType<TargetingSystem>();
        OrbitCamera oc = FindObjectOfType<OrbitCamera>();

        anim.SetBool("Dead", true);

        anim.SetInteger("RollType", Random.Range(1, 4));
        anim.SetTrigger("Death");

        StopCoroutine("WalkDirection");

        bfh.DeathBleed(damageType);

        GetComponent<CharacterController>().enabled = false;

        dropWeapon.parent = null;
        dropWeapon.GetComponent<Rigidbody>().isKinematic = false;
        dropWeapon.GetComponent<Collider>().enabled = true;

        ShowBattleUI(false);
        pb.KilledTarget(t);
    }

    void SwitchState(EnemyStates es)
    {
        currentState = es;
        anim.SetInteger("WalkingDir", 0);
        switch (currentState)
        {
            case EnemyStates.normal:
                lostPlayer = false;
                anim.applyRootMotion = false;
                agent.enabled = true;
                agent.speed = 1;

                NoGuard();

                if (patrolPoints.Length > 0)
                {
                    StartCoroutine("Patrolling");
                }

                anim.SetBool("inCombat", false);
                
                StopCoroutine("WalkDirection");
                break;

            case EnemyStates.chasing:
                lostPlayer = false;
                anim.applyRootMotion = false;
                agent.enabled = true;

                anim.SetBool("inCombat", false);

                StopCoroutine("Patrolling");
                StopCoroutine("WalkDirection");
                break;

            case EnemyStates.inCombat:
                lostPlayer = false;
                AttackStunned();

                anim.applyRootMotion = true;
                agent.enabled = false;

                RandomizeGuard();

                anim.SetBool("inCombat", true);
                anim.SetBool("Walking", true);
                anim.SetInteger("WalkingDir", 1);
                
                walkBehaviour = false;

                StopCoroutine("Patrolling");
                StartCoroutine("WalkDirection");
                break;

            default:
                break;
        }
    }

    bool PlayerInSight()
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
                    return true;
                }
                return false;
            }
            return false;
        }
        return false;
    }
    
    #region patrolling

    IEnumerator Patrolling()
    {
        Vector3 randomWayPoint = patrolPoints[Random.Range(0, patrolPoints.Length)].position;
        agent.SetDestination(randomWayPoint);
        anim.SetBool("Walking", true);
        while (Vector3.Distance(randomWayPoint, transform.position) > 1)
        {
            yield return new WaitForSeconds(0.25f);
        }
        anim.SetBool("Walking", false);
        yield return new WaitForSeconds(Random.Range(minThinkTime, maxThinkTime));
        StartCoroutine("Patrolling");
    }

    void Patrol()
    {
        if (distancePlayer <= hearDistance)
        {
            SwitchState(EnemyStates.inCombat);
        }
        if (PlayerInSight())
        {
            SwitchState(EnemyStates.chasing);
        }
    }

    #endregion

    #region Chase

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
        if (lostPlayer == false)
        {
            if (PlayerInSight() == false)
            {
                lostPlayer = true;
                StartCoroutine("LosingPlayer");
            }
        }
        if (lostPlayer)
        {
            if (PlayerInSight())
            {
                lostPlayer = false;
                StopCoroutine("LosingPlayer");
            }
        }
    }

    IEnumerator LosingPlayer()
    {
        yield return new WaitForSeconds(chaseTime);
        anim.SetBool("Running", false);
        anim.SetBool("Walking", true);
        SwitchState(EnemyStates.normal);
    }

    #endregion

    #region Combat

    void InCombat()
    {
        if (rotateCooldown == false)
        {
            RotateTowardsPlayer();
        }
        if (distancePlayer > (runDistance + 1f))
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
        StartCoroutine(AttackCld());
    }

    IEnumerator AttackCld()
    {
        attackCooldown = true;
        rotateCooldown = true;
        yield return new WaitForSeconds(1.2f);
        rotateCooldown = false;
        RandomizeGuard();
        yield return new WaitForSeconds(Random.Range(minThinkTime, maxThinkTime));
        attackCooldown = false;
    }
    
    void AttackStunned()
    {
        if (attackCooldown == false)
        {
            attackCooldown = true;
            StopCoroutine("Stunned");
            StartCoroutine("Stunned");
        }
    }

    IEnumerator Stunned()
    {
        yield return new WaitForSeconds(stunTime);
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

        randomInt = Random.Range(1.0f, 3.0f);
        yield return new WaitForSeconds(randomInt);

        anim.SetBool("Walking", false);

        randomInt = Random.Range(0.5f, 2.0f);
        yield return new WaitForSeconds(randomInt);

        int randomWalkDir = Random.Range(1, 4);
        anim.SetBool("Walking", true);
        anim.SetInteger("WalkingDir", randomWalkDir);
        RandomizeGuard();

        StartCoroutine("WalkDirection");
    }
    
    public void RotateTowardsPlayer()
    {
        Quaternion newLookAt = Quaternion.LookRotation(player.position - transform.position);
        newLookAt.x = 0;
        newLookAt.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, newLookAt, Time.deltaTime * 5);
    }

    public void ShowBattleUI(bool show)
    {
        fightUI.SetActive(show);
        targetOfPlayer = show;
    }

    //12 23 34 41
    // Y 1 = TOPLEFT Y -1 = RightDown
    // X 1 = LeftDown X -1 = TopRight
    public void RandomizeGuard()
    {
        if (currentState != EnemyStates.inCombat)
        {
            return;
        }
        int randomTemp = Random.Range(0, 2);
        if (randomTemp == 1)
        {
            fightAnim.SetFloat("mouseX", 0);
            fightAnim.SetFloat("mouseY", Random.Range(0, 2));
            if (fightAnim.GetFloat("mouseY") == 0)
            {
                fightAnim.SetFloat("mouseY", -1);
                guardPos = GuardStates.RightDown;
            }
            else
            {
                fightAnim.SetFloat("mouseY", 1);
                guardPos = GuardStates.TopLeft;
            }
        }
        else
        {
            fightAnim.SetFloat("mouseY", 0);
            fightAnim.SetFloat("mouseX", Random.Range(0, 2));
            if (fightAnim.GetFloat("mouseX") == 0)
            {
                fightAnim.SetFloat("mouseX", -1);
                guardPos = GuardStates.TopRight;
            }
            else
            {
                fightAnim.SetFloat("mouseX", 1);
                guardPos = GuardStates.LeftDown;
            }
        }
    }

    void NoGuard()
    {
        fightAnim.SetFloat("mouseY", 0);
        fightAnim.SetFloat("mouseX", 0);
        guardPos = GuardStates.None;
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

        Gizmos.color = Color.cyan;

        Vector3 pointRight = transform.position + Vector3.up * 1.2f + (transform.forward + ((transform.right * ((float)viewAngle / 100)) * 4)).normalized * lookDistance;
        Gizmos.DrawSphere(pointRight, 0.1f);

        Vector3 pointLeft = transform.position + Vector3.up * 1.2f + (transform.forward + ((-transform.right * ((float)viewAngle / 100)) * 4)).normalized * lookDistance;
        Gizmos.DrawSphere(pointLeft, 0.1f);

        Vector3 middlePoint = transform.position + Vector3.up * 1.2f + transform.forward * lookDistance;
        Gizmos.DrawSphere(middlePoint, 0.1f);

        Gizmos.DrawLine(middlePoint, pointLeft);
        Gizmos.DrawLine(middlePoint, pointRight);

        Gizmos.color = colorDebug;

        if (patrolPoints.Length > 0)
        {
            foreach (var t in patrolPoints)
            {
                Gizmos.DrawWireMesh(debugPos, t.position - Vector3.up);
            }
        }
    }
}
