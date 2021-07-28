using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int ID;
    [Space]
    public string itemName;
    public Sprite itemPotrait;
    [TextArea(5, 5)]
    public string itemDescription;
    public float weight;
    public float gold;
    [Space]
    public itemType typeItem;
    public enum itemType { NotEquipable, helmet, body, gloves, pants, boots, necklace, ringone, ringtwo, cape, weapon }
    public weaponType typeWeapon;
    public enum weaponType { none, sword, daggers, greatsword }
    [Header("Dont assign")]
    public bool equipped;
    public int itemLevel;
    [SerializeField] Rarity rarity;

    public enum Rarity { common, uncommon, rare, epic, legendary}
    
    public Rarity CurrentRarity
    {
        get
        {
            return rarity;
        }
        set
        {
            rarity = value;
        }
    }

    public void CreateCopy(Item original)
    {
        ID = original.ID;
        itemName = original.itemName;
        itemPotrait = original.itemPotrait;
        itemDescription = original.itemDescription;
        weight = original.weight;
        gold = original.gold;
        typeItem = original.typeItem;
        typeWeapon = original.typeWeapon;
    }
}
