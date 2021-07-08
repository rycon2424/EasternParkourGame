using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemLevel;
    public string itemName;
    public Sprite itemPotrait;
    public string itemDescription;
    [Space]
    public int ID;
    [Space]
    public itemType typeItem;
    public enum itemType { NotEquipable, helmet, body, gloves, pants, boots, necklace, ringone, ringtwo, cape, weapon}
}
