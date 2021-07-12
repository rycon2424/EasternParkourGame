using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantItem : MonoBehaviour
{
    public Item itemStore;
    [Space]
    public Text itemName;
    public Text itemLevel;
    public Text itemPrice;
    public Text itemWeight;
    public Image potrait;

    [Header("Dont assign")]
    public float prize;

    public void SetupItem(Item i)
    {
        itemStore = i;

        potrait.sprite = i.itemPotrait;
        itemName.text = i.itemName;
        itemLevel.text = i.itemLevel.ToString();
        prize = i.gold * mc.goldMultiplyer;
        itemPrice.text = prize.ToString();
        itemWeight.text = i.weight.ToString();
    }

    private Merchant mc;
    public void SetupMerchant(Merchant m)
    {
        mc = m;
    }

    public void BuyItem()
    {
        if (InventoryManager.instance.gold < prize)
        {
            return;
        }
        else
        {
            InventoryManager.instance.AddSubstractGold(-prize);
        }
        InventoryManager.instance.AddItemToInventory(itemStore);
        mc.sellingItems.Remove(itemStore);
        gameObject.SetActive(false);
    }
}
