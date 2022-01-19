using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameworkVR
{
    public class Cuttable : MonoBehaviour
    {
        [Header("Cut Properties")]
        [SerializeField]
        [Tooltip("If true, triangles inside the mesh cut will not be filled.")]
        public bool isHollow = false;

        [SerializeField]
        [Tooltip("If true, the cut objects will keep the same type of collider instead of a Mesh Collider.")]
        public bool keepSameCollider = false;

        [SerializeField]
        [Tooltip("If true, the player rig will no longer have collision with this once the object is cut.")]
        public bool noCollisionOnCut = false;

        [SerializeField]
        [Tooltip("Read only.")]
        private bool m_isCut = false;
        public bool isCut { get => m_isCut; }

        [HideInInspector]
        public Grabbable grabbable;

        [Header("Unity Events")]
        public UnityEvent onCut;

        private void Awake()
        {
            grabbable = GetComponent<Grabbable>();
        }
        public void SetIsCut(bool cut)
        {
            m_isCut = cut;
            if (cut) OnCut();
        }
        void OnCut()
        {
            if (onCut != null)
            {
                onCut.Invoke();
            }
        }
    }
}
