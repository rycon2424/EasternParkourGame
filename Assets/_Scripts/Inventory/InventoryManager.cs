using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Slider rotateSlider;
    public Transform playerModel;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void RotatePlayer()
    {
        playerModel.rotation = Quaternion.Euler(0, -rotateSlider.value, 0);
    }
}
