using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [Header("Assigned on spawn")]
    public int itemLevel;
    public Item.Rarity rarity; // Assign from boss
    [Header("Assigned randomly or not")]
    public int itemID;
    public Item.itemType itemType;
    public bool randomSpawn;

    [Header("Dont Assign")]
    [SerializeField] Light haloLight;
    [SerializeField] bool pickupable;
    [SerializeField] Rigidbody rb;

    private void Start()
    {
        if (randomSpawn)
        {
            List<TypeItems> tempList = ItemDataBase.instance.itemLibrary;
            TypeItems randomitems = tempList[Random.Range(0, tempList.Count)];
            Item randomItem = randomitems.items[Random.Range(0, randomitems.items.Count)];
            itemID = randomItem.ID;
            itemType = randomItem.typeItem;
        }
        rb = GetComponent<Rigidbody>();
        haloLight = GetComponent<Light>();
        Color32 tempColor = new Color32(255,255,255,255);
        switch (rarity)
        {
            case Item.Rarity.common:
                tempColor = ItemDataBase.instance.common;
                break;
            case Item.Rarity.uncommon:
                tempColor = ItemDataBase.instance.uncommon;
                break;
            case Item.Rarity.rare:
                tempColor = ItemDataBase.instance.rare;
                break;
            case Item.Rarity.epic:
                tempColor = ItemDataBase.instance.epic;
                break;
            case Item.Rarity.legendary:
                tempColor = ItemDataBase.instance.legendary;
                break;
            default:
                break;
        }
        haloLight.color = tempColor;
        ShootToRandomDirection();
    }

    void ShootToRandomDirection()
    {
        rb.AddForce(new Vector3(Random.Range(-1f , 1f), Random.Range(-0.5f, 1f) * Random.Range(1f , 3f), Random.Range(-1f, 1f)), ForceMode.Impulse);
    }

    void Update()
    {
        if (pickupable)
        {
            if (InventoryManager.instance.RoomInInventory())
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Item itemCopy = ItemDataBase.instance.GetItemInfo(itemID, itemType);
                    if (itemCopy != null)
                    {
                        itemCopy.itemLevel = itemLevel;
                        itemCopy.CurrentRarity = rarity;
                        
                        InventoryManager.instance.AddItemToInventory(itemCopy);
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
