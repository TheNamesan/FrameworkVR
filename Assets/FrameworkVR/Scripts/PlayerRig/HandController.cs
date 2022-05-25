using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.Events;

namespace FrameworkVR 
{
    public class HandController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        [Tooltip("Input Action Asset. Set from ActionMap folder.")]
        private InputActionAsset m_ActionAsset;
        public InputActionAsset actionAsset { get => m_ActionAsset; set => m_ActionAsset = value; }

        public enum HandSide { Left, Right };
        [SerializeField]
        [Tooltip("Which hand this component is bound to.")]
        public HandSide handSide = HandSide.Left;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Determines the amount of force on the controller's grip/trigger to count as trying to hold an object.")]
        public float holdForce = 0.8f;

        [SerializeField]
        [Range(0f, 1f)]
        [Tooltip("Determines the amount of force on the controller's grip/trigger to count as releasing a held object.")]
        public float releaseForce = 0.5f;

        [SerializeField]
        [Tooltip("The multiplier of the amount of force applied after throwing an object.")]
        public float throwForce = 1500;

        [SerializeField]
        [Tooltip("Current controller's grip force.")]
        public float gripForce = 0;

        [SerializeField]
        [Tooltip("Current controller's trigger force.")]
        public float triggerForce = 0;

        [Header("Grabbing System")]
        [SerializeField]
        [Tooltip("Currently held Game Object.")]
        public Grabbable heldItem;

        [Header("Collision")]
        [Tooltip("Set the GameObject with the Player Rig Controller component. This is used to avoid the grabbable's collision with the Player Rig.")]
        public GameObject characterRig;
        [Tooltip("The hand follower's physical collider. This is used to avoid the grabbable's collision with the collider.")]
        public Collider handCol;

        [Header("Weight System")]
        [SerializeField]
        [Tooltip("If true, the player's hands will move slower while holding heavier objects (Determined by the grabbable's Rigidbody mass). Will override grabbable settings.")]
        public bool enableWeightSystem = true;

        [SerializeField]
        [Tooltip("Currently held Game Object.")]
        public float heldItemMass = 0;

        [Header("Haptics")]
        [SerializeField]
        [Tooltip("The amplitude of the controller's rumble when grabbing an object.")]
        public float grabRumbleAmplitude = 0.5f;

        [SerializeField]
        [Tooltip("The duration of the amplitude in seconds when grabbing an object.")]
        public float grabRumbleDuration = 0.1f;

        [SerializeField]
        [Tooltip("The amplitude of the controller's rumble when releasing an object.")]
        public float releaseRumbleAmplitude = 0.2f;

        [SerializeField]
        [Tooltip("The duration of the amplitude in seconds when releasing an object.")]
        public float releaseRumbleDuration = 0.1f;

        [Header("Models")]
        [Tooltip("Set the model of the hand.")]
        public GameObject handModel;
        protected GameObject modelClone;

        [Header("Unity Events")]
        public UnityEvent onObjectHold;
        public UnityEvent onObjectRelease;

        private float prevFrameGripForce = 0;
        private Vector3 prevFrameHandPosition;

        private SphereCollider grabTrigger;

        InputAction gripButton;
        InputAction triggerButton;
        InputControl control;
        XRControllerWithRumble rumble;

        void Awake()
        {
            grabTrigger = GetComponent<SphereCollider>();
            gripForce = 0;
            prevFrameGripForce = 0;
            prevFrameHandPosition = Vector3.zero;
            gripButton = m_ActionAsset.FindActionMap(handSide + "Hand", true).FindAction("Grip", true);
            triggerButton = m_ActionAsset.FindActionMap(handSide + "Hand", true).FindAction("Trigger", true);
        }

        private void OnEnable()
        {
            if (m_ActionAsset != null)
            {
                m_ActionAsset.Enable();
            }
            else
            {
                Debug.LogWarning("WARNING: Input Action Asset not set!");
            }
        }

        void Update()
        {
            GetGripTriggerForce();
            CheckReleaseForce();

            prevFrameHandPosition = transform.position;
        }

        void ControllerRumble(float amplitude, float duration)
        {
            if (control != null && rumble != null)
            {
                rumble.SendImpulse(amplitude, duration);
            }
        }

        void GetItemMass()
        {
            if (enableWeightSystem)
            {
                heldItemMass = heldItem.mass;
            }
        }

        void GetGripTriggerForce()
        {
            prevFrameGripForce = GetHighestForce();
            gripForce = gripButton.ReadValue<float>();
            triggerForce = triggerButton.ReadValue<float>();
        }

        void OnObjectHold()
        {
            if (onObjectHold != null)
            {
                onObjectHold.Invoke();
            }
        }

        void OnObjectRelease()
        {
            if (onObjectRelease != null)
            {
                onObjectRelease.Invoke();
            }
        }

        public void ObjectGrab(Grabbable grabbable)
        {
            if (grabbable == null) return;
            else if (grabbable.isHeld)
            {
                if (grabbable.holder.GetComponent<HandController>() != null)
                {
                    grabbable.holder.GetComponent<HandController>().ObjectRelease();
                }
                grabbable.Release();
                grabbable.previousParent = null;
            }
            if (grabbable.GetComponent<Magazine>() != null)
            {
                Magazine tmpMag = grabbable.GetComponent<Magazine>();
                tmpMag.SetIsLoaded(false, null);
                tmpMag.transform.parent = null;
                grabbable.Release();
                grabbable.previousParent = null;
            }

            heldItem = grabbable;
            modelClone = Instantiate(handModel, grabbable.transform, true);

            handModel.SetActive(false);

            Physics.IgnoreCollision(characterRig.GetComponent<CharacterController>(), heldItem.GetComponent<Collider>(), true);

            heldItem.Hold(this);
            GetItemMass();
            ControllerRumble(grabRumbleAmplitude, grabRumbleDuration);
            OnObjectHold();
        }

        public void ObjectRelease()
        {
            if (modelClone != null) Destroy(modelClone);
            handModel.SetActive(true);
            if (heldItem == null) return;
            Physics.IgnoreCollision(characterRig.GetComponent<CharacterController>(), heldItem.GetComponent<Collider>(), false);
            
            Magazine heldItemMag = heldItem.GetComponent<Magazine>();
            if (heldItemMag == null)
            {
                heldItem.Release();
                GetThrowForce();
            }
            else 
            {
                if (!heldItemMag.isLoaded)
                {
                    heldItem.Release();
                    GetThrowForce();
                }
            }
            heldItem = null;
            heldItemMass = 0;
            ControllerRumble(releaseRumbleAmplitude, releaseRumbleDuration);
            OnObjectRelease();
        }

        void GetThrowForce()
        {
            Vector3 throwForceDirection = transform.position - prevFrameHandPosition;
            heldItem.GetComponent<Rigidbody>().AddForce(throwForceDirection * throwForce);
        }

        void CheckReleaseForce()
        {
            if (heldItem != null && GetHighestForce() <= releaseForce)
            {
                ObjectRelease();
            }
        }

        /// <summary>
        /// Returns the highest force value from either grip or trigger buttons.
        /// </summary>
        public float GetHighestForce()
        {
            if (gripForce > triggerForce)
            {
                return gripForce;
            }
            else return triggerForce;
        }


        private void OnTriggerStay(Collider other)
        {
            Grabbable tmpGrabbable = other.GetComponent<Grabbable>();
            GrabPoint tmpGrabPoint = other.GetComponent<GrabPoint>();
            if (heldItem == null && TriggerIsFullyHeld() &&
                GetHighestForce() > prevFrameGripForce && prevFrameGripForce < holdForce)
            {
                if (tmpGrabbable != null)
                {
                    if (!tmpGrabbable.hasGrabPoints)
                    {
                        ObjectGrab(tmpGrabbable);
                    }
                }
                else if (tmpGrabPoint != null)
                {
                    if(tmpGrabPoint.grabbableObjectOrigin.transform.GetComponent<Gun>() != null && tmpGrabPoint.grabbableObjectOrigin.isHeld)
                    {
                        return;
                    }
                    ObjectGrab(tmpGrabPoint.grabbableObjectOrigin);
                }
            }
        }

        private bool TriggerIsFullyHeld()
        {
            return GetHighestForce() >= holdForce;
        }
    }
}

