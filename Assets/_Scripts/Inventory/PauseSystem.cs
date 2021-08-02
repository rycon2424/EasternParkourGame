using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseSystem : MonoBehaviour
{
    public bool blockPausing;
    public static PauseSystem instance;
    public bool paused;
    public GameObject menu;
    public GameObject inventory;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public void OpenInventory()
    {
        Pause(false);
        inventory.SetActive(true);
        InventoryManager.instance.CheckForCombat();
    }

    public void Pause(bool showMenu)
    {
        paused = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0;
        menu.SetActive(showMenu);
        InventoryManager.instance.HideDisplay();
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        paused = false;
        Time.timeScale = 1;

        menu.SetActive(false);
        inventory.SetActive(false);
    }
}