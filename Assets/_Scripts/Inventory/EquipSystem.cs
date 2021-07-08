using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    public Equipment[] helmet;
    public Equipment[] body;
    public Equipment[] gloves;
    public Equipment[] trousers;
    public Equipment[] boots;
    [Space]
    public Equipment[] necklace;
    public Equipment[] ring1;
    public Equipment[] ring2;
    public Equipment[] cape;
    public Equipment[] weapon;
}

[System.Serializable]
public class Equipment
{
    public int ID;
    public GameObject[] equipment;
}
