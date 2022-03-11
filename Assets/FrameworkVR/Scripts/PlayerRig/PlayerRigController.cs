using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FrameworkVR
{
    public class PlayerRigController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField]
        [Tooltip("Input Action Asset. Set from ActionMap folder.")]
        private InputActionAsset m_ActionAsset;
        public InputActionAsset actionAsset { get => m_ActionAsset; set => m_ActionAsset = value; }
        
        [Tooltip("Set the input action reference to trigger jump.")]
        public InputActionReference jumpInput;

        [Header("Player Properties")]
        [SerializeField]
        [Tooltip("Player speed for X and Z axis relative to Time.deltaTime.")]
        public float playerSpeed = 0f;

        [SerializeField]
        [Tooltip("Multiplier to player's speed for rotating the camera.")]
        public float playerTurnSpeed = 1f;

        [SerializeField]
        [Tooltip("Forced applied to player jump.")]
        public float jumpForce = 1f;

        [SerializeField]
        [Tooltip("Player's real height in meters (Positive value).")]
        private float m_playerHeight = 0f;
        public float playerHeight { get => Mathf.Abs(m_playerHeight); set => m_playerHeight = Mathf.Abs(value); }

        [SerializeField]
        [Tooltip("Rig height in Unity scene.")]
        public float rigHeight = 0f;

        [SerializeField]
        [Tooltip("Player gravity (Positive value).")]
        public float m_gravity = 9.81f;
        public float gravity { get => Mathf.Abs(m_gravity); set => m_gravity = Mathf.Abs(value); }

        [Header("Ground Collision")]
        [SerializeField]
        [Tooltip("Layers that count as ground.")]
        public LayerMask groundLayers;
        public bool touchingGround = false;

        private Vector2 m_LeftJoystick;
        private Vector2 m_RightJoystick;
        private bool m_Jump;
        private CharacterController charController;
        private Camera cam;
        private Transform headTransform;
        private Transform HMDTransform;
        private Transform leftHandTransform;
        private Transform rightHandTransform;

        private float rotationValue;

        public float gravityVelocity = 0f;
        

        private InputAction leftJoystickInput;
        private InputAction rightJoystickInput;

        public bool disableWalk = false;
        public bool disableGravity = false;
        public bool disableRotation = false;

        public void Awake()
        {
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 0;

            charController = GetComponent<CharacterController>();
            headTransform = transform.GetChild(0);
            HMDTransform = headTransform.GetChild(0);
            leftHandTransform = transform.GetChild(1);
            rightHandTransform = transform.GetChild(2);
            leftJoystickInput = m_ActionAsset.FindActionMap("LeftHand", true).FindAction("Primary2DAxis", true);
            rightJoystickInput = m_ActionAsset.FindActionMap("RightHand", true).FindAction("Primary2DAxis", true);
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
            GetInputs();
            AdjustRigHeight();
            MoveAndRotation();
        }

        private void GetInputs()
        {
            if (m_ActionAsset != null)
            {
                m_LeftJoystick = leftJoystickInput.ReadValue<Vector2>();
                m_RightJoystick = rightJoystickInput.ReadValue<Vector2>();
            }

            jumpInput.action.performed += (ctx) =>
            {
                if (touchingGround)
                {
                    m_Jump = true;
                    touchingGround = false;
                }
            };
        }

        private void AdjustRigHeight()
        {
            float playerHeightScaledToRig = Mathf.InverseLerp(0, playerHeight, HMDTransform.localPosition.y);
            charController.height = rigHeight * playerHeightScaledToRig;
        }

        private void MoveAndRotation()
        {
            if (!disableGravity)
            {
                touchingGround = Physics.Raycast(transform.position - Vector3.up * charController.height * 0.5F, -transform.up, 0.3f, groundLayers);

                gravityVelocity += -gravity * Time.deltaTime;
                if (m_Jump) gravityVelocity = jumpForce;
                if (touchingGround)
                {
                    if (gravity < 0) gravityVelocity = 0;
                }
            }
            
            if(!disableWalk)
            {
                Vector3 moveX = new Vector3(HMDTransform.forward.x, 0, HMDTransform.forward.z) * playerSpeed;
                float moveY = gravityVelocity;
                Vector3 moveZ = new Vector3(HMDTransform.right.x, 0, HMDTransform.right.z) * playerSpeed;

                Vector3 moveTowards = (moveX * m_LeftJoystick.y +
                                        Vector3.up * moveY +
                                        moveZ * m_LeftJoystick.x)
                                        * Time.deltaTime;
                charController.Move(moveTowards);
            }

            if(!disableRotation)
            {
                rotationValue += playerTurnSpeed * m_RightJoystick.x;
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
                                                        rotationValue,
                                                        transform.eulerAngles.z);
            }

            m_Jump = false;
        }

        public void EnablePlayerWalk(bool input)
        {
            disableWalk = !input;
        }

        public void EnablePlayerGravity(bool input)
        {
            disableGravity = !input;
        }

        public void EnablePlayerRotation(bool input)
        {
            disableRotation = !input;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Cuttable cuttable = collision.transform.GetComponent<Cuttable>();
            if (cuttable != null)
            {
                if (cuttable.noCollisionOnCut)
                {
                    Physics.IgnoreCollision(collision.transform.GetComponent<Collider>(), GetComponent<Collider>());
                }
            }
        }
    }
}
