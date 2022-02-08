using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameworkVR
{
    public class Grabbable : MonoBehaviour
    {
        [Header("Grabbable Properties")]
        [SerializeField]
        [Tooltip("Read only.")]
        private bool m_isHeld = false;
        public bool isHeld { get => m_isHeld; }

        [SerializeField]
        [Tooltip("Disables the ability to grab anywhere. Use this if your grabbable object has grab points.")]
        public bool hasGrabPoints = false;

        [SerializeField]
        [Tooltip("Makes the grabbable object able to be held by both hands.")]
        public bool grabWithTwoHands = false;

        [Header("Weight System")]
        [SerializeField]
        [Tooltip("If true, the player's hands will move slower while this object (Determined by the grabbable's Rigidbody mass).")]
        public bool enableWeightSystem = true;

        [HideInInspector]
        private float m_mass = 0;
        public float mass { get => m_mass; }

        //TODO: Hacer clase holder
        [SerializeField]
        [Tooltip("Current holder.")]
        public GameObject holder = null;

        public Transform previousParent = null;

        [HideInInspector]
        public Rigidbody rb;
        [HideInInspector]
        public Collider coll;

        [Header("Unity Events")]
        public UnityEvent onObjectHold;
        public UnityEvent onObjectRelease;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
            if (enableWeightSystem) m_mass = rb.mass;
        }

        public void Hold(GameObject whoHolds)
        {
            holder = whoHolds;
            m_isHeld = true;
            rb.isKinematic = true;
            coll.enabled = false;
            if (enableWeightSystem) m_mass = rb.mass;
            OnObjectHold();
        }

        public void Release()
        {
            holder = null;
            m_isHeld = false;
            transform.parent = previousParent;
            rb.isKinematic = false;
            coll.enabled = true;
            if (enableWeightSystem) m_mass = rb.mass;
            OnObjectRelease();
        }

        public void SetCollider(Collider newcoll)
        {
            coll = newcoll;
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
    }
}
