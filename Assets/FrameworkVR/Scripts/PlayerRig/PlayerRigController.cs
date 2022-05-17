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

        public enum RotationMode { SmoothRotation = 0, QuickRotation = 1, FlickStick = 2 };
        [Tooltip("The rotation type to use for rotation the camera.\n\n" +
            "Smooth Rotation: Camera rotates continuosly until the player stops pushing the joystick.\n\n" +
            "Quick Rotation: Camera rotates a certain amount of degrees depending on the joystick's direction.\n\n" +
            "Flick Stick: Camera rotates immediately towards the joystick's direction.")]
        public RotationMode rotationMode = RotationMode.SmoothRotation;
        [SerializeField]
        [Tooltip("Multiplier to player's speed for rotating the camera.")]
        public float playerTurnSpeed = 1f;
        [Tooltip("The amount of degrees to turn for the camera with Quick Rotation.")]
        public float rotationDegree = 30f;

        [SerializeField]
        [Tooltip("Force applied to player jump.")]
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

        [SerializeField] private float rotationValue;
        private Vector3 lastHMDPosition;

        public float gravityVelocity = 0f;
        
        private InputAction leftJoystickInput;
        private InputAction rightJoystickInput;

        private bool rightJoystickHold = false;

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
            lastHMDPosition = HMDTransform.position;
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
            lastHMDPosition = HMDTransform.position;
        }

        void RightJoystickDown(InputAction.CallbackContext ctx)
        {

            if(rotationMode == RotationMode.QuickRotation)
            {
                if (rotationMode == RotationMode.QuickRotation && Mathf.Abs(ctx.ReadValue<Vector2>().x) > 0.4f)
                {
                    if (rightJoystickHold) return;
                    rotationValue += rotationDegree * Mathf.Sign(ctx.ReadValue<Vector2>().x);
                    rightJoystickHold = true;
                }
                else rightJoystickHold = false;
            }
            else if(rotationMode == RotationMode.FlickStick)
            {
                float threshold = 0.4f;
                bool holdingThreshold = Mathf.Abs(ctx.ReadValue<Vector2>().x) > threshold || Mathf.Abs(ctx.ReadValue<Vector2>().y) > threshold;
                if (!rightJoystickHold && holdingThreshold)
                {
                    rotationValue += Mathf.Atan2(ctx.ReadValue<Vector2>().x, ctx.ReadValue<Vector2>().y) * Mathf.Rad2Deg;
                    Debug.Log("RotationVal: " + rotationValue + " | VEC2: " + ctx.ReadValue<Vector2>());
                    rightJoystickHold = true;
                }
                else if(!holdingThreshold) rightJoystickHold = false;
            }
        }

        private void GetInputs()
        {
            if (m_ActionAsset != null)
            {
                m_LeftJoystick = leftJoystickInput.ReadValue<Vector2>();
                m_RightJoystick = rightJoystickInput.ReadValue<Vector2>();

                rightJoystickInput.performed += RightJoystickDown;
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
                Vector3 HMDDelta = new Vector3(HMDTransform.position.x - lastHMDPosition.x, 0, HMDTransform.position.z - lastHMDPosition.z);
                Vector3 moveX = new Vector3(HMDTransform.forward.x, 0, HMDTransform.forward.z) * playerSpeed;
                float moveY = gravityVelocity;
                Vector3 moveZ = new Vector3(HMDTransform.right.x, 0, HMDTransform.right.z) * playerSpeed;
                Vector3 moveDelta = (moveX * m_LeftJoystick.y +
                                        Vector3.up * moveY +
                                        moveZ * m_LeftJoystick.x) * Time.deltaTime;

                Vector3 moveTowards = moveDelta + HMDDelta;
                charController.Move(moveTowards);
            }

            if(!disableRotation)
            {
                if(rotationMode == RotationMode.SmoothRotation) rotationValue += playerTurnSpeed * m_RightJoystick.x;
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
