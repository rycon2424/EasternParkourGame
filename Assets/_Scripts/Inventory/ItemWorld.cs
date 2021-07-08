using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public bool pickupable;
    [Header("Item Info")]
    public int itemLevel;
    public string itemName;
    public Sprite itemPotrait;
    [TextArea(5, 5)]
    public string itemDescription;
    
    void Update()
    {
        if (pickupable)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Item tempItem = new Item();
                tempItem.itemLevel = itemLevel;
                tempItem.itemName = itemName;
                tempItem.itemPotrait = itemPotrait;
                tempItem.itemDescription = itemDescription;

                InventoryManager.instance.AddItemToInventory(tempItem);
                Destroy(gameObject);
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
