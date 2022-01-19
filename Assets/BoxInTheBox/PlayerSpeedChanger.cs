using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FrameworkVR;

public class PlayerSpeedChanger : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Set the Character Rig object.")]
    public PlayerRigController player;

    [Header("Input")]
    [Tooltip("Set the input action reference to alternate speed.")]
    public InputActionReference action;

    InputAction alternateButton;
    bool buttonPressed = false;

    public float normalSpeed = 0;
    public float alternativeSpeed = 0;
    void Awake()
    {
        alternateButton = action.action;
    }

    void Update()
    {
        alternateButton.performed += (ctx) =>
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
                player.playerSpeed = alternativeSpeed;
            }
        };
        alternateButton.canceled += (ctx) =>
        {
            if (buttonPressed)
            {
                player.playerSpeed = normalSpeed;
                buttonPressed = false;
            }
        };
    }
}
