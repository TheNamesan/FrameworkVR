using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace FrameworkVR
{
    public class UIMenu : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("Set the input action reference to trigger the UI Menu.")]
        public InputActionReference action;

        [Header("Canvas")]
        [Tooltip("Set the GameObject that corresponds to the UI Canvas.")]
        public GameObject canvas;

        [Header("Unity Events")]
        public UnityEvent onMenuOpen;
        public UnityEvent onMenuClose;

        InputAction menuButton;
        bool buttonPressed = false;

        void Awake()
        {
            canvas.SetActive(false);
            menuButton = action.action;
        }

        void Update()
        {
            menuButton.performed += (ctx) =>
            {
                if (!buttonPressed)
                {
                    buttonPressed = true;
                    OpenCloseMenu();
                }
            };
            menuButton.canceled += (ctx) =>
            {
                if (buttonPressed)
                {
                    buttonPressed = false;
                }
            };
        }

        void OpenCloseMenu()
        {
            canvas.SetActive(canvas.activeInHierarchy ? false : true);
            onMenuOpen?.Invoke();
        }

        public void CloseMenu()
        {
            canvas.SetActive(false);
            onMenuClose?.Invoke();
        }
    }
}

