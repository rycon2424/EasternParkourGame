using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : Humanoid
{
    State currentState;

    [Header("PlayerStats")]
    public int currentDamage;
    public float armorRating;
    public float slideSpeed;

    [Header("RaycastInfo")]
    public string tagGround;
    public LayerMask everything;
    public LayerMask climbing;
    public Vector3 lastCachedhit;
    public float distanceFromWall;
    public float grabHeight;

    [Header("Combat")]
    public bool hasWeapon;
    public GameObject playerWeapon;
    public GameObject sheatedWeapon;
    public Animator combatUI;
    public GameObject combatUIVisual;
    private HitBox hb;

    [Header("Info")]
    public bool grounded;
    public bool ccGrounded;
    public bool crouched;
    public bool failedClimb;
    public bool overWeight;

    [Header("IK")]
    public bool injured;
    [Range(0, 1)] public float leftWeight;
    public Transform leftPos;

    [Header("VFX")]
    public ParticleSystem jumpSmoke;
    public ParticleSystem bleeding;
    private BloodFXHandler bfh;

    #region public hidden
    [HideInInspector] public CharacterController cc;
    [HideInInspector] public OrbitCamera oc;
    [HideInInspector] public PlayerUI pui;
    [HideInInspector] public LockOnLookat lol;
    [HideInInspector] public TargetingSystem ts;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool lockedOn;
    [HideInInspector] public PlayerControls pc;
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Vector3 defaultCenterCc;

    [HideInInspector] public MonoBehaviour mono;
    #endregion

    void Start()
    {
        stateMachine = new StateMachine();

        mono = this;

        cc = GetComponent<CharacterController>();
        ts = GetComponent<TargetingSystem>();
        anim = GetComponent<Animator>();
        pc = GetComponent<PlayerControls>();
        bfh = GetComponent<BloodFXHandler>();
        hb = GetComponent<HitBox>();

        defaultCenterCc = cc.center;

        oc = GetComponentInChildren<OrbitCamera>();
        lol = GetComponentInChildren<LockOnLookat>();
        pui = GetComponentInChildren<PlayerUI>();
        pui.UpdateHPBar(health);

        lol.gameObject.SetActive(false);
        
        SetupStateMachine();
    }

    void SetupStateMachine()
    {
        Locomotion lm = new Locomotion();
        InAir ia = new InAir();
        Climbing cl = new Climbing();
        Combat co = new Combat();
        Sliding sl = new Sliding();
        HorseRiding hr = new HorseRiding();

        stateMachine.allStates.Add(lm);
        stateMachine.allStates.Add(ia);
        stateMachine.allStates.Add(cl);
        stateMachine.allStates.Add(co);
        stateMachine.allStates.Add(sl);
        stateMachine.allStates.Add(hr);

        stateMachine.GoToState(this, "Locomotion");
    }

    void Update()
    {
        Pausing();
        ccGrounded = cc.isGrounded;
        currentState.StateUpdate(this);
        anim.SetBool("Land", Grounded());
    }

    void Pausing()
    {
        if (Input.GetKeyDown(pc.pause))
        {
            if (PauseSystem.instance.paused == false)
            {
                PauseSystem.instance.Pause();
            }
            else
            {
                InventoryManager.instance.ExitSellingMode();
                PauseSystem.instance.Resume();
            }
        }
        if (Input.GetKeyDown(pc.inventory))
        {
            if (PauseSystem.instance.paused == false)
            {
                PauseSystem.instance.OpenInventory();
            }
            else
            {
                InventoryManager.instance.ExitSellingMode();
                PauseSystem.instance.Resume();
            }
        }
    }

    public override void TakeDamage(int damage, int damageType, int attackDir)
    {
        base.TakeDamage(damage, damageType, attackDir);
        if (dead)
        {
            return;
        }
        if (stateMachine.IsInState("Locomotion"))
        {
            if (ts.SelectTarget(oc))
            {
                EquipWeapon(1);
                stateMachine.GoToState(this, "Combat");
            }
        }
        if (stateMachine.IsInState("Combat"))
        {
            int armedType = anim.GetInteger("ArmedType");
            switch (armedType)
            {
                case 0:
                    anim.Play("Hit UnArmed");
                    break;
                case 1:
                    anim.Play("Hit Sword");
                    break;
                case 2:
                    anim.Play("Hit Dagger");
                    break;
                default:
                    break;
            }
        }
        if (injured == false && health < 50)
        {
            injured = true;
        }
        bfh.SlashDamage(damageType);

        float damageToCalculate = damage;

        damageToCalculate = damageToCalculate * (armorRating / 60);
        int damageReduction = Mathf.RoundToInt(damageToCalculate);

        health -= (damage - damageReduction);

        pui.UpdateHPBar(health);

        if (health < 1)
        {
            dead = true;
            health = 0;
            //Death(damageType);
        }
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
    }

    public void RotateTowardsCamera()
    {
        Quaternion newLookAt = Quaternion.LookRotation(ts.currentTarget.transform.position - transform.position);
        newLookAt.x = 0;
        newLookAt.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, newLookAt, Time.deltaTime * 5);
    }

    [Header("Ground Raycast")]
    public float startHeight = 0.5f;
    public float range = 1;
    public float airtime;

    public string CheckTag(Vector3 offset)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + (transform.up * startHeight) + offset, Vector3.down);
        Debug.DrawRay(transform.position + (transform.up * startHeight) + offset, Vector3.down * 1.1f, Color.green);
        if (Physics.Raycast(ray, out hit, 1.1f, everything))
        {
            tagGround = hit.collider.tag;
            return hit.collider.tag;
        }
        else
        {
            return "";
        }
    }

    public bool Grounded()
    {
        if (cc.isGrounded == true)
        {
            return true;
        }
        else if (RayHit(transform.position + (transform.right * 0.2f) + (transform.up * startHeight), (Vector3.down), range, everything)
            || RayHit(transform.position + (-transform.right * 0.2f) + (transform.up * startHeight), (Vector3.down), range, everything)
            || RayHit(transform.position + (transform.forward * 0.2f) + (transform.up * startHeight), (Vector3.down), range, everything)
            || RayHit(transform.position + (-transform.forward * 0.2f) + (transform.up * startHeight), (Vector3.down), range, everything)
            || RayHit(transform.position + (transform.up * startHeight), (Vector3.down), range, everything))
        {
            grounded = true;
            if (airtime != 0)
            {
                //Debug.Log("Total airtime was " + airtime.ToString("F2"));
            }
            airtime = 0;
            return true;
        }
        airtime += Time.deltaTime;
        grounded = false;
        return false;
    }

    public bool RayHit(Vector3 start, Vector3 dir, float length, LayerMask lm)
    {
        RaycastHit hit;
        Ray ray = new Ray(start, dir);
        Debug.DrawRay(start, dir * length, Color.magenta, 0.1f);
        if (Physics.Raycast(ray, out hit, length, lm))
        {
            lastCachedhit = hit.point;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void LerpToPosition(Vector3 pos, float lerpTime)
    {
        Debug.DrawLine(transform.position, pos, Color.red, 5);
        StartCoroutine(LerpToPos(pos, lerpTime));
    }

    IEnumerator LerpToPos(Vector3 pos, float lerpTime)
    {
        anim.applyRootMotion = false;
        Vector3 startPos = transform.position;

        for (float t = 0; t < 1; t += Time.deltaTime / lerpTime)
        {
            transform.position = Vector3.Lerp(startPos, pos, t);
            yield return new WaitForEndOfFrame();
        }

        //transform.position = pos;
        anim.applyRootMotion = true;
    }

    public bool PlayerToWall(PlayerBehaviour pb, Vector3 dir, bool lerp, float checkYOffset, float closeDistance)
    {
        RaycastHit hit;
        float range = 2;
        Vector3 playerHeight = new Vector3(pb.transform.position.x, pb.transform.position.y + checkYOffset, pb.transform.position.z);
        Debug.DrawRay(playerHeight, dir * range, Color.green);
        if (Physics.Raycast(playerHeight, dir, out hit, range))
        {
            Vector3 temp = pb.transform.position - hit.point;
            temp.y = 0;
            Vector3 positionToSend = pb.transform.position - temp;
            positionToSend -= (pb.transform.forward * (distanceFromWall + closeDistance));
            if (lerp)
            {
                pb.LerpToPosition(positionToSend, 0.25f);
            }
            else
            {
                transform.position = positionToSend;
            }
            return true;
        }
        return false;
    }

    public bool PlayerFaceWall(PlayerBehaviour pb, Vector3 startOffset, Vector3 dir, float range)
    {
        RaycastHit hit;
        Vector3 playerHeight = pb.transform.position;
        playerHeight += startOffset;
        Debug.DrawRay(playerHeight, dir * range, Color.cyan, 5);
        if (Physics.Raycast(playerHeight, dir, out hit, range))
        {
            Quaternion hitRotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            pb.transform.rotation = hitRotation;
            return true;
        }
        return false;
    }

    public bool LedgeInfo()
    {
        if (!RayHit(transform.position + (transform.up * 1.7f), transform.forward, 0.45f, climbing))
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + (transform.forward * 0.45f) + (transform.up * 1.7f), Vector3.down);
            Debug.DrawRay(transform.position + (transform.forward * 0.45f) + (transform.up * 1.7f), Vector3.down * 0.35f, Color.red, 0.25f);
            if (Physics.Raycast(ray, out hit, 0.35f, climbing))
            {
                lastCachedhit = hit.point;
                string tagObject = hit.collider.tag;
                switch (tagObject)
                {
                    case "Ledge":
                        stateMachine.GoToState(this, "Climbing");
                        break;
                    case "Example":
                        break;
                    default:
                        break;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator LerpCharacterControllerCenter(float to, float lerpTime)
    {
        float timeElapsed = 0;
        float beginFloat = cc.center.y;
        while (timeElapsed < lerpTime)
        {
            beginFloat = Mathf.Lerp(beginFloat, to, timeElapsed / 0.25f);
            Vector3 temp = new Vector3(0, beginFloat, 0);
            cc.center = temp;
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Vector3 finalY = new Vector3(0, to, 0);
        cc.center = finalY;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (injured)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, leftPos.position);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftPos.rotation);
            if (bleeding.isPlaying == false)
            {
                bleeding.Play();
            }
        }
        else
        {
            if (bleeding.isPlaying)
            {
                bleeding.Stop();
            }
        }
    }

    public void SetInjured()
    {
        if (health <= 100)
        {
            injured = true;
            bleeding.Play();
        }
        else
        {
            injured = false;
            bleeding.Stop();
        }
    }

    public Vector3 CalculateSlopeDirection()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 1, Color.red, 1f);
        if (Physics.Raycast(ray, out hit, 1f))
        {
            Vector3 slopeRight = Vector3.Cross(Vector3.up, hit.normal);
            Vector3 slopeDirection = Vector3.Cross(slopeRight, hit.normal).normalized;
            return slopeDirection;
        }
        return Vector3.zero;
    }

    public void KilledTarget(Target targetThatDied)
    {
        ts.targets.Remove(targetThatDied);
        ts.StopTargeting();
        hb.targets.Remove(targetThatDied.GetComponent<Actor>());
        
        ts.SwitchTarget(oc);
        lol.target = ts.currentTarget;

        if (ts.currentTarget == null)
        {
            stateMachine.GoToState(this, "Locomotion");
        }
    }

    public void DelayFunction(string functionName, float delay)
    {
        Invoke(functionName, delay);
    }
    void DelayedRoot()
    {
        anim.applyRootMotion = true;
    }

    public void _GoToState(string stateName)
    {
        stateMachine.GoToState(this, stateName);
    }

    public void _DoubleJump(float targetFloat)
    {
        StartCoroutine(LerpCharacterControllerCenter(targetFloat, 0.2f));
    }

    public void AddRotation(Vector3 rot)
    {
        transform.Rotate(rot);
    }

    public bool IsInAnimation(string animName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            return true;
        }
        return false;
    }

    public void EquipWeapon(int equip)
    {
        if (playerWeapon == null || sheatedWeapon == null)
        {
            return;
        }
        if (equip == 1)
        {
            playerWeapon.SetActive(true);
            sheatedWeapon.SetActive(false);
        }
        else
        {
            playerWeapon.SetActive(false);
            sheatedWeapon.SetActive(true);
        }
    }

    public void ForceHideWeapon()
    {
        if (sheatedWeapon != null)
        {
            if (playerWeapon.activeSelf == true)
            {
                playerWeapon.SetActive(false);
                sheatedWeapon.SetActive(true);
            }
        }
    }

    public void CancelTrigger(string triggerName)
    {
        anim.ResetTrigger(triggerName);
    }

    public void CheckForSmoke()
    {
        if (Grounded() == false)
        {
            jumpSmoke.Play();
        }
    }

    public void Parrying()
    {
        parrying = !parrying;
    }

    public void TooHeavy(bool restrictmovement)
    {
        overWeight = restrictmovement;
    }

}
