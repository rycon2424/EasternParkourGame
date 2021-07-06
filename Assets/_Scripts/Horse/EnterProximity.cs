using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterProximity : MonoBehaviour
{
    public enum ActorAllowed { none, player, enemy}
    public ActorAllowed allowedMounters;
    public HorseBehaviour.playerPosition mountPosition;
    [Space]
    public HorseBehaviour horse;

    private void OnTriggerStay(Collider other)
    {
        Humanoid mounter = other.GetComponent<Humanoid>();
        if (mounter == null)
        {
            return;
        }
        switch (allowedMounters)
        {
            case ActorAllowed.none:
                break;
            case ActorAllowed.player:
                if (mounter is PlayerBehaviour)
                {
                    horse.UpdateMountPos(mountPosition, mounter);
                }
                break;
            case ActorAllowed.enemy:
                if (mounter is EnemyController)
                {
                    horse.UpdateMountPos(mountPosition, mounter);
                }
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Humanoid mounter = other.GetComponent<Humanoid>();
        if (mounter != null)
        {
            horse.UpdateMountPos(HorseBehaviour.playerPosition.none, mounter);
            return;
        }
    }
}
