using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public float gold;
    public bool playerInRange;
    public float goldMultiplyer;
    [Range(1, 20)] public int maxRandomLevel;
    [Space]
    public GameObject buying;
    public List<ItemSelling> itemsSelling;
    [Space]
    public List<Item> sellingItems = new List<Item>();

    private void Start()
    {
        buying.SetActive(false);
        foreach (var sellingItem in itemsSelling)
        {
            if (sellingItem.randomID)
            {
                sellingItem.RandomizeID();
            }
            sellingItems.Add(ItemDataBase.instance.GetItemInfo(sellingItem.ID, sellingItem.type));
        }
    }

    private void Update()
    {
        if (playerInRange)
        {
            if (PauseSystem.instance.paused == false)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PauseSystem.instance.OpenInventory();
                    InventoryManager.instance.shop.SetActive(true);
                    InventoryManager.instance.LoadShop(sellingItems, this);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            playerInRange = true;
            buying.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            playerInRange = false;
            buying.SetActive(false);
        }
    }
}

[System.Serializable]
public class ItemSelling
{
    public int ID;
    public Item.itemType type;
    public Item.Rarity rarity;
    [Space]
    public bool randomID;

    public void RandomizeID()
    {
        foreach (var itemSet in ItemDataBase.instance.itemLibrary)
        {
            if (itemSet.type == type)
            {
                if (itemSet.items.Count > 1)
                {
                    ID = itemSet.items[Random.Range(0, itemSet.items.Count)].ID;
                    return;
                }
                else
                {
                    ID = itemSet.items[0].ID;
                    return;
                }
            }
        }
    }
}

