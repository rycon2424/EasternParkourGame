using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int ID;
    [Space]
    public int itemLevel;
    public string itemName;
    public Sprite itemPotrait;
    [TextArea(5, 5)]
    public string itemDescription;
    public float weight;
    public float gold;
    [Space]
    public itemType typeItem;
    public enum itemType { NotEquipable, helmet, body, gloves, pants, boots, necklace, ringone, ringtwo, cape, weapon}
    [Header("Dont assign")]
    public bool equipped;
}
