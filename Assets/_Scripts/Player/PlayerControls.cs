using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    [Header("Customizable Controls")]
    [TextArea]
    [Tooltip("Doesn't do anything. Just comments shown in inspector")]
    public string Notes = "Target swapping is currently ---MouseScroll--- and not customizable yet";

    public int mouseClickAttack = 0;
    public int mouseClickParry = 1;
    [Space]
    public string inputHorizontal = "Horizontal";
    public string inputVertical = "Vertical";
    [Space]
    public KeyCode grab = KeyCode.E;
    public KeyCode target = KeyCode.F;
    public KeyCode jump = KeyCode.Space;
    public KeyCode crouch = KeyCode.X;
    public KeyCode sprint = KeyCode.LeftShift;
    public KeyCode inventory = KeyCode.Tab;
    public KeyCode pause = KeyCode.Escape;
}