using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler
{
    public bool taken;
    [Space]
    public Item item;
    public bool beingHovered;
    [Space]
    public Image image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (beingHovered == false)
        {
            InventoryManager.instance.ShowDisplay(item, this);
        }
    }

    private void Update()
    {
        if (beingHovered)
        {
            if (item != null)
            {
                if (item.typeItem != Item.itemType.NotEquipable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (item.equipped)
                        {
                            if (InventoryManager.instance.RoomInInventory())
                            {
                                //Remove object
                                item.equipped = false;
                                InventoryManager.instance.AddItemToInventory(item);
                                InventoryManager.instance.HideDisplay();
                                EquipSystem.instance.Equip(item, false, true);
                                ResetSlot();
                            }
                        }
                        else
                        {
                            //Equip/Swap object
                            InventoryManager.instance.EquipItem(this);
                        }
                    }
                }
            }
        }
    }

    public void ResetSlot()
    {
        taken = false;
        item = null;
        beingHovered = false;
        image.sprite = InventoryManager.instance.emptyImage;
    }
}