using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public Item currentHelm;
    public Item currentBody;
    public Item currentGloves;
    public Item currentTrousers;
    public Item currentBoots;
    public Item currentCape;
    public Item currentNecklace;
    public Item currentWeapon;
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
    public Text apLevel;
    public Text arLevel;

    public static EquipSystem instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    private void Start()
    {
        CalculateStats();
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
                currentHelm = i;
                if (removing)
                {
                    EquipHelper(0, helmet1100, 1100, true);
                    currentHelm = null;
                }
                break;
            case Item.itemType.body:
                EquipHelper(ID, body1200, 1200, equip);
                currentBody = i;
                if (removing)
                {
                    EquipHelper(0, body1200, 1200, true);
                    currentBody = null;
                }
                break;
            case Item.itemType.gloves:
                EquipHelper(ID, gloves1300, 1300, equip);
                currentGloves = i;
                if (removing)
                {
                    EquipHelper(0, gloves1300, 1300, true);
                    currentGloves = null;
                }
                break;
            case Item.itemType.pants:
                EquipHelper(ID, trousers1400, 1400, equip);
                currentTrousers = i;
                if (removing)
                {
                    EquipHelper(0, trousers1400, 1400, true);
                    currentTrousers = null;
                }
                break;
            case Item.itemType.boots:
                EquipHelper(ID, boots1500, 1500, equip);
                currentBoots = i;
                if (removing)
                {
                    EquipHelper(0, boots1500, 1500, true);
                    currentBoots = null;
                }
                break;
            #region unused equipment
            case Item.itemType.necklace:
                currentNecklace = i;
                if (removing)
                {
                    currentNecklace = null;
                }
                break;
            case Item.itemType.ringone:
                break;
            case Item.itemType.ringtwo:
                break;
            #endregion
            case Item.itemType.cape:
                EquipHelper(ID, cape1900, 1900, equip);
                currentCape = i;
                if (removing)
                {
                    currentCape = null;
                }
                break;
            case Item.itemType.weapon:
                currentWeapon = i;
                if (removing)
                {
                    WeaponHandler("", Item.weaponType.none);
                    currentWeapon = null;
                }
                else
                {
                    WeaponHandler(i.itemName, i.typeWeapon);
                }
                break;
            default:
                break;
        }
        CalculateStats();
    }

    void WeaponHandler(string itemName, Item.weaponType wt)
    {
        if (pb.sheatedWeapon != null)
        {
            pb.sheatedWeapon.SetActive(false);
            pb.playerWeapon.SetActive(false);
            pb.anim.SetInteger("ArmedType", 0);
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
                switch (wt)
                {
                    case Item.weaponType.none:
                        pb.anim.SetInteger("ArmedType", 0);
                        break;
                    case Item.weaponType.sword:
                        pb.anim.SetInteger("ArmedType", 1);
                        break;
                    case Item.weaponType.spear:
                        pb.anim.SetInteger("ArmedType", 2);
                        break;
                    case Item.weaponType.greatsword:
                        pb.anim.SetInteger("ArmedType", 3);
                        break;
                    default:
                        break;
                }
                break;
            }
        }
        foreach (GameObject sword in weaponsModel)
        {
            sword.SetActive(false);
            //Debug.Log(sword.name + " and " + itemName);
            if (sword.name == itemName)
            {
                //Debug.Log(sword.name);
                sword.SetActive(true);
            }
        }
    }

    void CalculateStats()
    {
        int armorRating = 0;
        int weaponDamage;
        if (currentHelm != null)
        {
            armorRating += currentHelm.itemLevel;
        }
        if (currentBody != null)
        {
            armorRating += currentBody.itemLevel;
        }
        if (currentGloves != null)
        {
            armorRating += currentGloves.itemLevel;
        }
        if (currentTrousers != null)
        {
            armorRating += currentTrousers.itemLevel;
        }
        if (currentBoots != null)
        {
            armorRating += currentBoots.itemLevel;
        }
        if (currentCape != null)
        {
            armorRating += currentCape.itemLevel;
        }
        if (currentNecklace != null)
        {
            armorRating += currentNecklace.itemLevel;
        }
        weaponDamage = 8;
        if (currentWeapon != null)
        {
            weaponDamage += 3 * currentWeapon.itemLevel;
        }

        pb.currentDamage = weaponDamage;
        pb.armorRating = armorRating;
        pb.damage = pb.currentDamage;

        apLevel.text = "Attack Power = " + weaponDamage.ToString();
        arLevel.text = "Armor Level = " + armorRating.ToString();
    }
}

[System.Serializable]
public class Equipment
{
    public int ID;
    public GameObject[] equipment;
}
