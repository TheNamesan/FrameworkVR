using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkVR
{
    public class HandFollower : MonoBehaviour
    {
        [Header("Hand Tracking")]
        [SerializeField]
        [Tooltip("Set the GameObject with the Tracked Pose Driver component of the hand to follow.")]
        public Transform handToTrack;

        [SerializeField]
        [Tooltip("Set the GameObject with the Hand Controller component.")]
        public HandController handController;

        [Header("Hand Collision")]
        [SerializeField]
        [Tooltip("Set the layer bound to the GameObject with the Player Rig Controller component. This is used to avoid the hand's collision with the Player Rig.")]
        public int playerLayerNumber;

        [Header("Weight System")]
        [SerializeField]
        [Tooltip("Set the follower's position base speed. The higher the number the slower it moves.")]
        private float m_basePositionSmooth = 0.01f;
        public float basePositionSmooth { get => Mathf.Abs(m_basePositionSmooth); set => m_basePositionSmooth = Mathf.Abs(value); }

        [SerializeField]
        [Tooltip("Set the follower's rotation base speed. The higher the number the slower it turns.")]
        private float m_baseRotationSmooth = 0.01f;
        public float baseRotationSmooth { get => Mathf.Abs(m_baseRotationSmooth); set => m_baseRotationSmooth = Mathf.Abs(value); }

        [SerializeField]
        [Tooltip("Current follower position speed. Affected by the grabbable's weight.")]
        public float positionSmooth = 0;

        [SerializeField]
        [Tooltip("Current follower rotation speed. Affected by the grabbable's weight.")]
        public float rotationSmooth = 0;

        private Rigidbody rb;
        private Vector3 velocity = Vector3.zero;
        private Vector3 angularVelocity = Vector3.zero;


        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Physics.IgnoreLayerCollision(playerLayerNumber, gameObject.layer);
        }

        private void OnEnable()
        {
            velocity = Vector3.zero;
            angularVelocity = Vector3.zero;

            transform.position = handToTrack.position;
            transform.rotation = handToTrack.rotation;
        }

        void Update()
        {
            positionSmooth = handController.heldItemMass <= 0 ? basePositionSmooth : basePositionSmooth * handController.heldItemMass;
            rotationSmooth = handController.heldItemMass <= 0 ? baseRotationSmooth : baseRotationSmooth * handController.heldItemMass;

            Vector3.SmoothDamp(transform.position, handToTrack.position, ref velocity, positionSmooth);
            rb.velocity = velocity;

            transform.eulerAngles = new Vector3(
                Mathf.SmoothDampAngle(transform.eulerAngles.x, handToTrack.eulerAngles.x, ref angularVelocity.x, rotationSmooth),
                Mathf.SmoothDampAngle(transform.eulerAngles.y, handToTrack.eulerAngles.y, ref angularVelocity.y, rotationSmooth),
                Mathf.SmoothDampAngle(transform.eulerAngles.z, handToTrack.eulerAngles.z, ref angularVelocity.z, rotationSmooth));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(handToTrack.position, transform.position);
        }
    }
}
