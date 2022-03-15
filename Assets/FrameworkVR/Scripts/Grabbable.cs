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
        public Rigidbody previousJointBody = null;

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

        void FixedUpdate()
        {
            UpdateGrabbablePosition();
        }

        protected void UpdateGrabbablePosition()
        {
            if (holder != null)
            {
                Vector3 moveTowards = holder.transform.position;
                //Vector3 distanceToTarget = moveTowards - transform.position;
                //Vector3 targetVelocity = (distanceToTarget * 2500f) * Time.fixedDeltaTime;
                //rb.velocity = Vector3.MoveTowards(rb.velocity, targetVelocity, 1f);
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                //rb.MovePosition(moveTowards);
                //rb.velocity = Vector3.zero;
            }
        }

        public void Hold(GameObject whoHolds)
        {
            holder = whoHolds;
            m_isHeld = true;
            //rb.useGravity = false;
            
            if(holder.transform.parent != null)
            {
                if (holder.transform.parent.GetComponent<Collider>() != null)
                    Physics.IgnoreCollision(holder.transform.parent.GetComponent<Collider>(), coll);
            }
            FixedJoint fj;
            if (gameObject.GetComponent<FixedJoint>() == null) fj = gameObject.AddComponent<FixedJoint>();
            else fj = gameObject.GetComponent<FixedJoint>();
            previousJointBody = fj.connectedBody;
            Rigidbody holderRb = holder.GetComponent<Rigidbody>(); ;
            if(holderRb != null) fj.connectedBody = holderRb;
            else fj.connectedBody = holder.transform.parent.GetComponent<Rigidbody>();

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
            FixedJoint fj = gameObject.GetComponent<FixedJoint>();
            if(fj != null) Destroy(fj);
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
