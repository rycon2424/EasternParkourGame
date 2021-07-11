using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler
{
    public bool taken;
    public bool toRemove;
    [Space]
    public Item item;
    public bool beingHovered;
    [Space]
    public Image image;
    public GameObject cross;
    
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
                if (Input.GetMouseButtonDown(1))
                {
                    if (item.equipped == false)
                    {
                        toRemove = !toRemove;
                        cross.SetActive(toRemove);
                        InventoryManager.instance.CheckForSelected();
                    }
                }
                if (item.typeItem != Item.itemType.NotEquipable)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        toRemove = false;
                        if (cross != null)
                        {
                            cross.SetActive(false);
                        }
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
                                InventoryManager.instance.CheckForSelected();
                            }
                        }
                        else
                        {
                            //Equip/Swap object
                            toRemove = false;
                            if (cross != null)
                            {
                                cross.SetActive(false);
                            }
                            InventoryManager.instance.EquipItem(this);
                            InventoryManager.instance.CheckForSelected();
                        }
                    }
                }
            }
        }
    }

    public void ResetSlot()
    {
        toRemove = false;
        if (cross != null)
        {
            cross.SetActive(false);
        }
        taken = false;
        item = null;
        beingHovered = false;
        image.sprite = InventoryManager.instance.emptyImage;
    }
}