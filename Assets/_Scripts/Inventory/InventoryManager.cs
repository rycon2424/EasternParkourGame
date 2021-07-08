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
    public InventorySlot helmet;
    public InventorySlot body;
    public InventorySlot gloves;
    public InventorySlot trousers;
    public InventorySlot boots;
    [Space]
    public InventorySlot necklace;
    public InventorySlot ring1;
    public InventorySlot ring2;
    public InventorySlot cape;
    public InventorySlot weapon;
    [Space]
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
    
    public void EquipItem(InventorySlot it)
    {
        Item.itemType typeEquip = it.item.typeItem;
        switch (typeEquip)
        {
            case Item.itemType.NotEquipable:
                return;
            case Item.itemType.helmet:
                TransferItemInfo(it.item, helmet.item);
                helmet.item = it.item;
                helmet.image.sprite = helmet.item.itemPotrait;
                break;
            case Item.itemType.body:
                TransferItemInfo(it.item, body.item);
                body.item = it.item;
                body.image.sprite = body.item.itemPotrait;
                break;
            case Item.itemType.gloves:
                TransferItemInfo(it.item, gloves.item);
                gloves.item = it.item;
                gloves.image.sprite = gloves.item.itemPotrait;
                break;
            case Item.itemType.pants:
                TransferItemInfo(it.item, trousers.item);
                trousers.item = it.item;
                trousers.image.sprite = trousers.item.itemPotrait;
                break;
            case Item.itemType.boots:
                TransferItemInfo(it.item, boots.item);
                boots.item = it.item;
                boots.image.sprite = boots.item.itemPotrait;
                break;
            case Item.itemType.necklace:
                TransferItemInfo(it.item, necklace.item);
                necklace.item = it.item;
                necklace.image.sprite = necklace.item.itemPotrait;
                break;
            case Item.itemType.ringone:
                TransferItemInfo(it.item, ring1.item);
                ring1.item = it.item;
                ring1.image.sprite = ring1.item.itemPotrait;
                break;
            case Item.itemType.ringtwo:
                TransferItemInfo(it.item, ring2.item);
                ring2.item = it.item;
                ring2.image.sprite = ring2.item.itemPotrait;
                break;
            case Item.itemType.cape:
                TransferItemInfo(it.item, cape.item);
                cape.item = it.item;
                cape.image.sprite = cape.item.itemPotrait;
                break;
            case Item.itemType.weapon:
                TransferItemInfo(it.item, weapon.item);
                weapon.item = it.item;
                weapon.image.sprite = weapon.item.itemPotrait;
                break;
            default:
                break;
        }
        it.item.equipped = true;
        it.ResetSlot();
        HideDisplay();
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
        display.SetActive(false);
    }

    InventorySlot oldSlot;
    public void ShowDisplay(Item i, InventorySlot sendSlot)
    {
        if (oldSlot != null)
        {
            oldSlot.beingHovered = false;
        }

        sendSlot.beingHovered = true;
        oldSlot = sendSlot;

        if (i == null || i.ID == 0)
        {
            HideDisplay();
            return;
        }

        display.SetActive(true);
        Vector3 offset = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;
        if (mousePos.y <= 520)
        {
            offset += new Vector3(0, 480, 0);
        }
        if (mousePos.x >= 1600)
        {
            offset += new Vector3(-250, 0, 0);
        }

        display.transform.position = mousePos + offset;

        itemName.text = i.itemName;
        itemDescription.text = i.itemDescription;
        itemLevel.text = ("Level " + i.itemLevel.ToString());
        itemImage.sprite = i.itemPotrait;
    }

    void TransferItemInfo(Item givingInfo, Item receivingInfo)
    {
        if (receivingInfo == null)
        {
            return;
        }
        receivingInfo.itemLevel = givingInfo.itemLevel;
        receivingInfo.itemName = givingInfo.itemName;
        receivingInfo.itemDescription = givingInfo.itemDescription;
        receivingInfo.ID = givingInfo.ID;
        receivingInfo.typeItem = givingInfo.typeItem;
        receivingInfo.itemLevel = givingInfo.itemLevel;
    }

}