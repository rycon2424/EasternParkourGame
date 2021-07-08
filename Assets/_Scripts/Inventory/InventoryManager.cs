using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    [Header("PlayerVisual")]
    public Slider rotateSlider;
    public Transform playerModel;

    [Header("ItemDisplay")]
    public GameObject display;
    public Text itemName;
    public Text itemDescription;
    public Text itemLevel;
    public Image itemImage;

    [Header("Inventory Slots")]
    public Sprite emptyImage;
    public InventorySlot[] inventorySpaces;
    public List<Item> inventoryItems;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    
    public void RotatePlayer()
    {
        playerModel.rotation = Quaternion.Euler(0, -rotateSlider.value, 0);
    }
    
    public void AddItemToInventory(Item newItem)
    {
        foreach (var slot in inventorySpaces)
        {
            if (slot.taken == false)
            {
                slot.taken = true;
                slot.image.sprite = newItem.itemPotrait;
                slot.item = newItem;
                break;
            }
        }
    }
    
    public void HideDisplay()
    {
        oldSlot = null;
        display.SetActive(false);
    }

    InventorySlot oldSlot;
    public void ShowDisplay(Item i, InventorySlot sendSlot)
    {
        if (oldSlot != null)
        {
            oldSlot.showingDisplay = false;
        }
        sendSlot.showingDisplay = true;

        if (i == null)
        {
            HideDisplay();
            return;
        }

        display.SetActive(true);
        display.transform.position = Input.mousePosition;
        itemName.text = i.itemName;
        itemDescription.text = i.itemDescription;
        itemLevel.text = ("Level " + i.itemLevel.ToString());
        itemImage.sprite = i.itemPotrait;

        oldSlot = sendSlot;
    }

}