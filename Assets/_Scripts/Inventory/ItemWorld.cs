using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public bool pickupable;

    public Item itemInfo;
    
    void Update()
    {
        if (pickupable)
        {
            if (InventoryManager.instance.RoomInInventory())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    InventoryManager.instance.AddItemToInventory(itemInfo);
                    Destroy(gameObject);
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
