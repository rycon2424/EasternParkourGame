using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseBehaviour : Animal
{
    [SerializeField] private bool mounted;
    public Transform mountPosition;

    [Header("Debug")]
    public horseState currentHorseState;
    [SerializeField] private playerPosition currentMountPos;
    [SerializeField] private Humanoid mountActor;
    [SerializeField] private Animator anim;

    public enum playerPosition { none, back, left, right, above, frontLeft, frontRight }
    public enum horseState { walking, back, sprinting, dead}

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (mounted)
        {
            float y = Input.GetAxis("Vertical");
            anim.SetFloat("x", Input.GetAxis("Horizontal"));
            anim.SetFloat("y", y);
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (currentHorseState == horseState.walking && anim.GetBool("Gallop") == false)
                {
                    anim.SetBool("Gallop", true);
                }
                else
                {
                    currentHorseState = horseState.sprinting;
                    anim.SetBool("Sprint", true);
                }
            }

            if (y < 0.9f)
            {
                currentHorseState = horseState.walking;
                anim.SetBool("Gallop", false);
                anim.SetBool("Sprint", false);
                if (y < 0)
                {
                    currentHorseState = horseState.back;
                }
            }
        }
    }

    public void Mount(Transform mounter)
    {
        mountActor = mounter.GetComponent<Humanoid>();
        if (mountActor == null)
        {
            Debug.LogWarning("The mounter is not an actor");
            return;
        }
        mounter.parent = mountPosition;
        Animator tempAnim = mountActor.GetComponent<Animator>();

        Vector3 goToPos = Vector3.zero;

        mountActor.currentHorse = this;

        StartCoroutine(LerpThenStartAnimation((mountPosition.position - mounter.up * 0.55f), 0.5f, tempAnim, mounter));
    }
    
    IEnumerator LerpThenStartAnimation(Vector3 pos, float lerpTime, Animator mounterAnim, Transform mounter)
    {
        mounterAnim.applyRootMotion = false;

        mounterAnim.SetInteger("MountType", mountActor.horseMountPosition);
        mounterAnim.SetTrigger("MountHorse");

        Vector3 startPos = mounter.transform.position;
        
        for (float t = 0; t < 1; t += Time.deltaTime / lerpTime)
        {
            mounter.transform.rotation = Quaternion.Lerp(mounter.transform.rotation, transform.rotation, t);
            mounter.transform.position = Vector3.Lerp(startPos, pos, t);
            yield return new WaitForEndOfFrame();
        }
        mounted = true;
    }

    public void Unmounted()
    {
        mounted = false;
    }

    public void UpdateMountPos(playerPosition newPos, Humanoid potentialMounter)
    {
        currentMountPos = newPos;
        mountActor = potentialMounter;
        mountActor.closestHorse = this;

        switch (currentMountPos)
        {
            case playerPosition.none:
                potentialMounter.horseMountPosition = -1;
                mountActor.closestHorse = null;
                mountActor = null;
                break;
            case playerPosition.back:
                potentialMounter.horseMountPosition = 0;
                break;
            case playerPosition.left:
                potentialMounter.horseMountPosition = 1;
                break;
            case playerPosition.right:
                potentialMounter.horseMountPosition = 2;
                break;
            case playerPosition.above:
                potentialMounter.horseMountPosition = 3;
                break;
            case playerPosition.frontLeft:
                potentialMounter.horseMountPosition = 4;
                break;
            case playerPosition.frontRight:
                potentialMounter.horseMountPosition = 5;
                break;
            default:
                break;
        }
    }

}
