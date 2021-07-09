using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    public int helmetID;
    public int bodyID;
    public int glovesID;
    public int trousersID;
    public int bootsID;
    public int capeID;
    public int weaponID;
    [Space]
    public Equipment[] helmet1100;
    public Equipment[] body1200;
    public Equipment[] gloves1300;
    public Equipment[] trousers1400;
    public Equipment[] boots1500;
    [Space]
    //public Equipment[] necklace1600;
    //public Equipment[] ringOne1700;
    //public Equipment[] ringTwo1800;
    public Equipment[] cape1900;
    public GameObject[] weaponsModel;

    [Header("PlayerCollection")]
    public GameObject[] playerClothes;
    public GameObject[] weaponsSheated;
    public GameObject[] weaponsHand;
    [Space]
    public PlayerBehaviour pb;

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
        List<string> equipOnModel = new List<string>();
        List<string> removeOnModel = new List<string>();
        if (equipping)
        {
            foreach (Equipment items in eq)
            {
                if (ID == items.ID)
                {
                    foreach (var g in items.equipment)
                    {
                        equipOnModel.Add(g.name);
                        g.SetActive(true);
                    }
                }
            }
            int amountToAdd = equipOnModel.Count;
            foreach (string s in equipOnModel)
            {
                foreach (GameObject g in playerClothes)
                {
                    if (g.name == s)
                    {
                        g.SetActive(true);
                        amountToAdd--;
                        break;
                    }
                }
            }
            if (amountToAdd > 0)
            {
                Debug.Log("One object could not be added (" + amountToAdd + ")");
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
                        removeOnModel.Add(g.name);
                        g.SetActive(false);
                    }
                }
            }
            int amountToRemove = removeOnModel.Count;
            foreach (string s in removeOnModel)
            {
                foreach (GameObject g in playerClothes)
                {
                    if (g.name == s)
                    {
                        g.SetActive(false);
                        amountToRemove--;
                        break;
                    }
                }
            }
            if (amountToRemove > 0)
            {
                Debug.Log("One object could not be removed (" + amountToRemove + ")");
            }
        }
    }

    public void RemoveBase(Item.itemType type)
    {
        switch (type)
        {
            case Item.itemType.body:
                EquipHelper(0, body1200, 1200, false);
                break;
            case Item.itemType.gloves:
                EquipHelper(0, gloves1300, 1300, false);
                break;
            case Item.itemType.pants:
                EquipHelper(0, trousers1400, 1400, false);
                break;
            case Item.itemType.boots:
                EquipHelper(0, boots1500, 1500, false);
                break;
            default:
                break;
        }
    }

    public void Equip(Item i, bool equip, bool removing)
    {
        Item.itemType type = i.typeItem;
        int ID = i.ID;
        if (!removing)
        {
            RemoveBase(type);
        }
        switch (type)
        {
            case Item.itemType.NotEquipable:
                return;
            case Item.itemType.helmet:
                EquipHelper(ID, helmet1100, 1100, equip);
                helmetID = 1100 + ID;
                if (removing)
                {
                    EquipHelper(0, helmet1100, 1100, true);
                }
                break;
            case Item.itemType.body:
                EquipHelper(ID, body1200, 1200, equip);
                bodyID = 1200 + ID;
                if (removing)
                {
                    EquipHelper(0, body1200, 1200, true);
                }
                break;
            case Item.itemType.gloves:
                EquipHelper(ID, gloves1300, 1300, equip);
                glovesID = 1300 + ID;
                if (removing)
                {
                    EquipHelper(0, gloves1300, 1300, true);
                }
                break;
            case Item.itemType.pants:
                EquipHelper(ID, trousers1400, 1400, equip);
                trousersID = 1400 + ID;
                if (removing)
                {
                    EquipHelper(0, trousers1400, 1400, true);
                }
                break;
            case Item.itemType.boots:
                EquipHelper(ID, boots1500, 1500, equip);
                bootsID = 1500 + ID;
                if (removing)
                {
                    EquipHelper(0, boots1500, 1500, true);
                }
                break;
            #region unused equipment
            case Item.itemType.necklace:
                return;
                //EquipHelper(ID, necklace1600, 1600, equip);
            case Item.itemType.ringone:
                return;
                //EquipHelper(ID, ringOne1700, 1700, equip);
            case Item.itemType.ringtwo:
                return;
            //EquipHelper(ID, ringTwo1800, 1800, equip);
            #endregion
            case Item.itemType.cape:
                EquipHelper(ID, cape1900, 1900, equip);
                capeID = 1900 + ID;
                break;
            case Item.itemType.weapon:
                weaponID = 2000 + ID;
                if (removing)
                {
                    WeaponHandler("");
                }
                else
                {
                    WeaponHandler(i.itemName);
                }
                break;
            default:
                break;
        }
    }

    void WeaponHandler(string itemName)
    {
        if (pb.sheatedWeapon != null)
        {
            Debug.Log("unequipping old sword");
            pb.sheatedWeapon.SetActive(false);
            pb.playerWeapon.SetActive(false);
            pb.sheatedWeapon = null;
            pb.playerWeapon = null;
        }
        foreach (GameObject sword in weaponsHand)
        {
            if (sword.name == itemName)
            {
                pb.playerWeapon = sword;
                break;
            }
        }
        foreach (GameObject sword in weaponsSheated)
        {
            if (sword.name == itemName)
            {
                pb.sheatedWeapon = sword;
                pb.sheatedWeapon.SetActive(true);
                break;
            }
        }
        foreach (GameObject sword in weaponsModel)
        {
            sword.SetActive(false);
            if (sword.name == itemName)
            {
                sword.SetActive(true);
            }
        }
    }
}

[System.Serializable]
public class Equipment
{
    public int ID;
    public GameObject[] equipment;
}
