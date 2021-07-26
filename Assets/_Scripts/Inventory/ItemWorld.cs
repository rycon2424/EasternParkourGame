using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public int itemID;
    public Item.itemType itemType;
    [Space]
    public bool pickupable;
    
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
                        InventoryManager.instance.AddItemToInventory(temp);
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
