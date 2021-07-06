using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseBehaviour : Animal
{
    public bool mounted;
    public enum playerPosition {none, back, left, right, above, frontLeft, frontRight}
    public Transform mountPosition;

    [Header("Debug")]
    [SerializeField] private playerPosition currentMountPos;
    [SerializeField] private Humanoid mountActor;
    
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

    void Update()
    {
        
    }

}
