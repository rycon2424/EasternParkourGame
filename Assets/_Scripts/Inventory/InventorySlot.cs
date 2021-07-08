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
            beingHovered = true;
            InventoryManager.instance.ShowDisplay(item, this);
        }
    }
}