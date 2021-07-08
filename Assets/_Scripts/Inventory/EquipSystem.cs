using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    public Equipment[] helmet1100;
    public Equipment[] body1200;
    public Equipment[] gloves1300;
    public Equipment[] trousers1400;
    public Equipment[] boots1500;
    [Space]
    public Equipment[] necklace1600;
    public Equipment[] ringOne1700;
    public Equipment[] ringTwo1800;
    public Equipment[] cape1900;
    public Equipment[] weapon2000;

    public static EquipSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    void EquipHelper(int ID, Equipment[] eq, int extraID, bool equipping)
    {
        ID += extraID;
        if (equipping)
        {
            foreach (Equipment items in eq)
            {
                if (ID == items.ID)
                {
                    foreach (var g in items.equipment)
                    {
                        g.SetActive(true);
                    }
                    return;
                }
            }
        }
        else
        {
            foreach (Equipment items in eq)
            {
                if (ID == items.ID)
                {
                    foreach (var g in items.equipment)
                    {
                        g.SetActive(false);
                    }
                    return;
                }
            }
        }
        Debug.Log("error no type with such ID");
    }

    public void Equip(Item.itemType type, int ID, bool equip)
    {
        switch (type)
        {
            case Item.itemType.NotEquipable:
                return;
            case Item.itemType.helmet:
                EquipHelper(ID, helmet1100, 1100, equip);
                break;
            case Item.itemType.body:
                EquipHelper(ID, body1200, 1200, equip);
                break;
            case Item.itemType.gloves:
                EquipHelper(ID, gloves1300, 1300, equip);
                break;
            case Item.itemType.pants:
                EquipHelper(ID, trousers1400, 1400, equip);
                break;
            case Item.itemType.boots:
                EquipHelper(ID, boots1500, 1500, equip);
                break;
            case Item.itemType.necklace:
                EquipHelper(ID, necklace1600, 1600, equip);
                break;
            case Item.itemType.ringone:
                EquipHelper(ID, ringOne1700, 1700, equip);
                break;
            case Item.itemType.ringtwo:
                EquipHelper(ID, ringTwo1800, 1800, equip);
                break;
            case Item.itemType.cape:
                EquipHelper(ID, cape1900, 1900, equip);
                break;
            case Item.itemType.weapon:
                EquipHelper(ID, weapon2000, 2000, equip);
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class Equipment
{
    public int ID;
    public GameObject[] equipment;
}
