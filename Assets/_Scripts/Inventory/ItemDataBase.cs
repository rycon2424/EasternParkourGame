using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : MonoBehaviour
{
    public static ItemDataBase instance;

    public bool debug;
    public List<TypeItems> itemLibrary;
    [Header("Colours")]
    public Color32 common;
    public Color32 uncommon;
    public Color32 rare;
    public Color32 epic;
    public Color32 legendary;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    public Item GetItemInfo(int id, Item.itemType type)
    {
        Item i = null;
        foreach (var item in itemLibrary)
        {
            if (type == item.type)
            {
                ErrorMessage(0, item.name, 0);
                i = item.GetItemInfo(id);
                if (i != null)
                {
                    break;
                }
                else
                {
                    ErrorMessage(1, item.name, id);
                }
            }
        }
        if (i != null)
        {
            Item newItem = new Item(); // Create copy
            newItem.CreateCopy(i);
            ErrorMessage(3, i.itemName, id);
            return newItem; // Return Copy
        }
        ErrorMessage(2, type.ToString(), id);
        return null;
    }

    void ErrorMessage(int error, string extraMessage, int id)
    {
        if (debug == false)
        {
            return;
        }
        switch (error)
        {
            case 0:
                Debug.Log("Looking through library of " + extraMessage);
                break;
            case 1:
                Debug.Log("Found no item in library " + extraMessage + " with ID " + id);
                break;
            case 2:
                Debug.Log("FINAL: Found no item in library " + extraMessage + " with ID " + id);
                break;
            case 3:
                Debug.Log("FINAL: SUCCES FOUND ITEM " + extraMessage + " with ID " + id);
                break;
            default:
                break;
        }
    }
}

[System.Serializable]
public class TypeItems
{
    public string name;
    public Item.itemType type;
    public List<Item> items;

    public Item GetItemInfo(int id)
    {
        foreach (var item in items)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }
}