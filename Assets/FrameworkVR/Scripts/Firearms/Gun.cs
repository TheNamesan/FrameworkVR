using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace FrameworkVR 
{
    public class Gun : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        [Tooltip("Input Action Asset. Set from ActionMap folder.")]
        private InputActionAsset m_actionAsset;
        public InputActionAsset actionAsset { get => m_actionAsset; set => m_actionAsset = value; }
        [SerializeField]
        [Tooltip("Optional. Button to unload the magazine.")]
        private InputActionReference unloadInput;
        

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Determines the amount of force on the controller's trigger to count as shooting.")]
        public float holdForce = 0.8f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Determines the amount of force on the controller's trigger to count as not shooting.")]
        public float releaseForce = 0.5f;

        [SerializeField]
        [Tooltip("Current controller's trigger force.")]
        public float triggerForce = 0;

        [Header("Gun Properties")]
        [SerializeField]
        [Tooltip("Determines the firearm's rate of fire in seconds while the trigger is held. If 0, the weapon will be semi-automatic.")]
        public float rateOfFire = 0;

        [SerializeField]
        [Tooltip("Determines the firearm's type. Magazines with the same type name can load this weapon.")]
        public string firearmType = "";

        [Header("References")]
        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to this firearm's grabbable object.")]
        public Grabbable grabbableObjectOrigin;

        [SerializeField]
        [Tooltip("Currently loaded magazine.")]
        protected Magazine m_mag = null;
        public Magazine mag
        {
            get { return m_mag; }
            set
            {
                m_mag = value;
                OnReload();
            }
        }

        protected float cooldownTime = 0;

        [HideInInspector]
        public bool triggerHeld = false;
        protected bool heldAfterShot = false;

        [Header("Optional")]
        [SerializeField]
        [Tooltip("Set the Particle System when firing. Optional.")]
        public ParticleSystem muzzleFlash;

        [Header("Unity Events")]
        public UnityEvent onShoot;
        public UnityEvent onReload;

        protected HandController handController;
        protected InputAction triggerButton;

        protected virtual void Update()
        {
            if (grabbableObjectOrigin != null)
            {
                if (grabbableObjectOrigin.isHeld)
                {
                    if (handController == null)
                    {
                        GetHandController();
                    }
                    Unload();
                    GetTriggerForce();
                    CheckTriggerHeld();
                }
                else
                {
                    handController = null;
                    triggerHeld = false;
                    triggerButton = null;
                    triggerForce = 0;
                }
            }
            RateOfFireTimer();
        }

        void OnShoot()
        {
            if (onShoot != null)
            {
                onShoot.Invoke();
            }
        }

        void OnReload()
        {
            if (onReload != null)
            {
                onReload.Invoke();
            }
        }

        void Unload()
        {
            if (unloadInput == null) return;
            unloadInput.action.performed += (ctx) =>
            {
                if (mag != null)
                {
                    mag.transform.parent = null;
                    mag.grabbable.previousParent = null;
                    mag.grabbable.Release();
                    mag.SetIsLoaded(false, null);
                }
            };
        }

        protected virtual void GetHandController()
        {
            if (grabbableObjectOrigin == null) Debug.Log("Grabbable") ;
            if (grabbableObjectOrigin.holder == null) Debug.Log("Holder");
            handController = grabbableObjectOrigin.holder.GetComponent<HandController>();
            if (handController == null) Debug.Log("Hand Controller");
            triggerButton = m_actionAsset.FindActionMap(handController.handSide + "Hand", true).FindAction("Trigger", true);
        }

        void GetTriggerForce()
        {
            triggerForce = triggerButton.ReadValue<float>();
            if (triggerForce > holdForce)
            {
                triggerHeld = true;
            }
            else if (triggerForce <= releaseForce)
            {
                heldAfterShot = false;
                triggerHeld = false;
            }
        }

        protected virtual void CheckTriggerHeld()
        {
            if (mag != null)
            {
                if (mag.ammo > 0 && triggerHeld && cooldownTime <= 0)
                {
                    if (rateOfFire <= 0) //Non-automatic
                    {
                        if (!heldAfterShot) Shoot();
                    }
                    else Shoot();
                }
            }
        }

        void RateOfFireTimer()
        {
            if (cooldownTime != 0)
            {
                cooldownTime -= Time.deltaTime;
                if (cooldownTime < 0) cooldownTime = 0;
            }
        }

        public virtual void Shoot()
        {
            mag.ammo--;
            cooldownTime = rateOfFire;
            heldAfterShot = true;
            if(muzzleFlash != null) muzzleFlash.Play();
            OnShoot();
        }
    }

}
