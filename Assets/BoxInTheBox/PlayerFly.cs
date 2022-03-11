using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using FrameworkVR;

public class PlayerFly : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Set the Character Rig object.")]
    public PlayerRigController player;
    private CharacterController charController;
    private Transform HMDTransform;

    [Header("Input")]
    [Tooltip("Set the input action reference to enable fly.")]
    public InputActionReference action;
    [Header("Input")]
    [Tooltip("Set the input action reference to joystick axis.")]
    public InputActionReference joystick;

    InputAction flyButton;
    bool buttonPressed = false;
    InputAction leftJoystickInput;
    Vector2 joystickValue;

    public bool flyEnabled = false;


    void Awake()
    {
        flyButton = action.action;
        
        leftJoystickInput = joystick.action;
        charController = player.GetComponent<CharacterController>();
        HMDTransform = transform.GetChild(0).GetChild(0);
    }

    void Update()
    {
        joystickValue = leftJoystickInput.ReadValue<Vector2>();

        flyButton.performed += (ctx) =>
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
                flyEnabled = !flyEnabled;
                player.EnablePlayerWalk(!flyEnabled);
                player.EnablePlayerGravity(!flyEnabled);
            }
        };
        flyButton.canceled += (ctx) =>
        {
            if (buttonPressed)
            {
                buttonPressed = false;
            }
        };

        Vector3 moveX = HMDTransform.forward * joystickValue.y * player.playerSpeed * Time.deltaTime;
        Vector3 moveZ = HMDTransform.right * joystickValue.x * player.playerSpeed * Time.deltaTime;

        if (flyEnabled)
        {
            Vector3 moveTowards = moveX + moveZ;
            player.gravityVelocity = 0;
            charController.Move(moveTowards);
        }    
        
    }
}
