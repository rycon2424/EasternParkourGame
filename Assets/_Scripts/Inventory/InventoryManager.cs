using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("InventoryStats")]
    public float maxWeight;
    public float weight;
    public float gold;
    public Text weightT;
    public Text weightMaxT;
    public Text goldT;

    [Header("Buffs/Debuffs")]
    public GameObject weightIcon;

    public static InventoryManager instance;
    [Header("PlayerVisual")]
    public Slider rotateSlider;
    public Transform playerModel;

    [Header("ItemDisplay")]
    public GameObject display;
    public Text itemName;
    public Text itemDescription;
    public Text itemLevel;
    public Text goldWorth;
    public Text itemWeight;
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
    public Button destroyButton;
    public GameObject inCombatBlock;
    [Header("Shop")]
    public GameObject shop;
    public GameObject shopWindow;
    public MerchantItem[] shopItems;

    private PlayerBehaviour pb;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        pb = FindObjectOfType<PlayerBehaviour>();
    }

    private void Start()
    {
        UpdateWeightText();
        weightMaxT.text = maxWeight.ToString();
    }

    public void LoadShop(List<Item> items, Merchant mc)
    {
        shopWindow.SetActive(true);
        foreach (var t in shopItems)
        {
            t.gameObject.SetActive(false);
        }
        if (items.Count < 1)
        {
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
            shopItems[i].gameObject.SetActive(true);
            shopItems[i].SetupMerchant(mc);
            shopItems[i].SetupItem(items[i]);
        }
    }

    void UpdateWeightText()
    {
        weightT.text = weight.ToString() + "/";
        if (weight > maxWeight)
        {
            weightT.color = new Color32(255, 0, 0, 255);
            weightMaxT.color = new Color32(255, 0, 0, 255);
            ShowHideIcon("Weight", true);
            pb.TooHeavy(true);
        }
        else
        {
            weightT.color = new Color32(255, 255, 0, 255);
            weightMaxT.color = new Color32(255, 255, 0, 255);
            ShowHideIcon("Weight", false);
            pb.TooHeavy(false);
        }
    }

    public void AddSubstractGold(float gain)
    {
        gold += gain;
        goldT.text = gold.ToString();
    }

    public void ExitSellingMode()
    {
        shop.SetActive(false);
        PauseSystem.instance.Resume();
    }

    public void SellItems()
    {
        foreach (var slots in inventorySpaces)
        {
            if (slots.toRemove)
            {
                AddSubstractGold(slots.item.gold);
                weight -= slots.item.weight;
                UpdateWeightText();
                slots.ResetSlot();
            }
        }
    }
    
    public void RotatePlayer()
    {
        playerModel.rotation = Quaternion.Euler(0, -rotateSlider.value, 0);
    }

    public void CheckForSelected()
    {
        foreach (var slots in inventorySpaces)
        {
            if (slots.toRemove)
            {
                destroyButton.interactable = true;
                return;
            }
        }
        destroyButton.interactable = false;
    }

    public void DestroySelected()
    {
        foreach (var slots in inventorySpaces)
        {
            if (slots.toRemove)
            {
                weight -= slots.item.weight;
                UpdateWeightText();
                slots.ResetSlot();
            }
        }
        destroyButton.interactable = false;
    }

    public void CheckForCombat()
    {
        if (pb.stateMachine.IsInState("Combat"))
        {
            inCombatBlock.SetActive(true);
        }
        else
        {
            inCombatBlock.SetActive(false);
        }
    }
    
    public void EquipItem(InventorySlot it)
    {
        Item.itemType typeEquip = it.item.typeItem;
        switch (typeEquip)
        {
            case Item.itemType.NotEquipable:
                return;
            case Item.itemType.helmet:
                EquipGear(helmet, it);
                TransferItemInfo(it.item, helmet.item);
                break;
            case Item.itemType.body:
                EquipGear(body, it);
                TransferItemInfo(it.item, body.item);
                break;
            case Item.itemType.gloves:
                EquipGear(gloves, it);
                TransferItemInfo(it.item, gloves.item);
                break;
            case Item.itemType.pants:
                EquipGear(trousers, it);
                TransferItemInfo(it.item, trousers.item);
                break;
            case Item.itemType.boots:
                EquipGear(boots, it);
                TransferItemInfo(it.item, boots.item);
                break;
            case Item.itemType.necklace:
                EquipGear(necklace, it);
                TransferItemInfo(it.item, necklace.item);
                break;
            case Item.itemType.ringone:
                EquipGear(ring1, it);
                TransferItemInfo(it.item, ring1.item);
                break;
            case Item.itemType.ringtwo:
                EquipGear(ring1, it);
                TransferItemInfo(it.item, ring2.item);
                break;
            case Item.itemType.cape:
                EquipGear(cape, it);
                TransferItemInfo(it.item, cape.item);
                break;
            case Item.itemType.weapon:
                EquipGear(weapon, it);
                TransferItemInfo(it.item, weapon.item);
                break;
            default:
                break;
        }
        it.item.equipped = true;
        it.ResetSlot();
        HideDisplay();
    }
    
    void EquipGear(InventorySlot equipping, InventorySlot giving)
    {
        //Check if replacing an item or not
        if (equipping.taken)
        {
            //Debug.Log("swapping " + equipping.item.itemName + " with " + giving.item.itemName);
            EquipSystem.instance.Equip(equipping.item, false, false);
            SwapItem(equipping);
        }
        equipping.item = giving.item;
        equipping.image.sprite = giving.item.itemPotrait;
        equipping.taken = true;
        weight -= equipping.item.weight;
        UpdateWeightText();
        EquipSystem.instance.Equip(equipping.item, true, false);
    }

    void SwapItem(InventorySlot ivst)
    {
        ivst.item.equipped = false;
        AddItemToInventory(ivst.item);
    }

    public bool RoomInInventory()
    {
        foreach (var slot in inventorySpaces)
        {
            if (slot.taken == false)
            {
                return true;
            }
        }
        return false;
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
                weight += newItem.weight;
                UpdateWeightText();
                break;
            }
        }
    }
    
    public void HideDisplay()
    {
        if (oldSlot != null)
        {
            oldSlot.beingHovered = false;
        }
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

        switch (i.CurrentRarity)
        {
            case Item.Rarity.common:
                itemName.color = ItemDataBase.instance.common;
                break;
            case Item.Rarity.uncommon:
                itemName.color = ItemDataBase.instance.uncommon;
                break;
            case Item.Rarity.rare:
                itemName.color = ItemDataBase.instance.rare;
                break;
            case Item.Rarity.epic:
                itemName.color = ItemDataBase.instance.epic;
                break;
            case Item.Rarity.legendary:
                itemName.color = ItemDataBase.instance.legendary;
                break;
            default:
                break;
        }
        
        itemName.text = i.itemName;
        itemDescription.text = i.itemDescription;
        itemLevel.text = ("Level " + i.itemLevel.ToString());
        itemWeight.text = i.weight.ToString();
        itemImage.sprite = i.itemPotrait;
        goldWorth.text = i.gold.ToString();
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

    public void ShowHideIcon(string icon, bool show)
    {
        switch (icon)
        {
            case "Weight":
                weightIcon.SetActive(show);
                break;
            default:
                break;
        }
    }
}