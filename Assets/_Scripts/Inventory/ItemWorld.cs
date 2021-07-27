using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [Header("Assign on Spawn")]
    public int itemID;
    public int itemLevel; // Assign from boss
    [Space]
    public Item.Rarity rarity; // Assign from boss
    public Item.itemType itemType;
    public bool randomSpawn;

    [Header("Dont Assign")]
    [SerializeField] Light haloLight;
    [SerializeField] bool pickupable;

    private void Start()
    {
        if (randomSpawn)
        {
            List<TypeItems> tempList = ItemDataBase.instance.itemLibrary;
            TypeItems randomitems = tempList[Random.Range(0, tempList.Count)];
            Item randomItem = randomitems.items[Random.Range(0, randomitems.items.Count)];
            itemID = randomItem.ID;
            itemType = randomItem.typeItem;
        }

        haloLight = GetComponent<Light>();
        Color32 tempColor = new Color32(255,255,255,255);
        switch (rarity)
        {
            case Item.Rarity.common:
                tempColor = ItemDataBase.instance.common;
                break;
            case Item.Rarity.uncommon:
                tempColor = ItemDataBase.instance.uncommon;
                break;
            case Item.Rarity.rare:
                tempColor = ItemDataBase.instance.rare;
                break;
            case Item.Rarity.epic:
                tempColor = ItemDataBase.instance.epic;
                break;
            case Item.Rarity.legendary:
                tempColor = ItemDataBase.instance.legendary;
                break;
            default:
                break;
        }
        haloLight.color = tempColor;
    }

    void Update()
    {
        if (pickupable)
        {
            if (InventoryManager.instance.RoomInInventory())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Item temp = ItemDataBase.instance.GetItemInfo(itemID, itemType);
                    if (temp != null)
                    {
                        Item newItem = new Item();
                        newItem.ID = temp.ID;
                        newItem.itemName = temp.itemName;
                        newItem.itemPotrait = temp.itemPotrait;
                        newItem.itemDescription = temp.itemDescription;
                        newItem.weight = temp.weight;
                        newItem.gold = temp.gold;
                        newItem.typeItem = temp.typeItem;
                        newItem.typeWeapon = temp.typeWeapon;

                        newItem.itemLevel = itemLevel;
                        newItem.CurrentRarity = rarity;
                        
                        InventoryManager.instance.AddItemToInventory(newItem);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            pickupable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            pickupable = false;
        }
    }
}
