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

    public void RandomRarity(int max)
    {
        int randomRange = Random.Range(0, max + 1);
        switch (randomRange)
        {
            case 0:
                CurrentRarity = Rarity.common;
                break;
            case 1:
                CurrentRarity = Rarity.uncommon;
                break;
            case 2:
                CurrentRarity = Rarity.rare;
                break;
            case 3:
                CurrentRarity = Rarity.epic;
                break;
            case 4:
                CurrentRarity = Rarity.legendary;
                break;
            default:
                break;
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
