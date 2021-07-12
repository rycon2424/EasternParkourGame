using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    public float gold;
    public bool playerInRange;
    [Space]
    public GameObject buying;
    public List<Item> sellingItems = new List<Item>();

    private void Update()
    {
        if (playerInRange)
        {
            if (PauseSystem.instance.paused == false)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PauseSystem.instance.OpenInventory();
                    InventoryManager.instance.sellMode.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour pb = other.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            playerInRange = false;
        }
    }
}
