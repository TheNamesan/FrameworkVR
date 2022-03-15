using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameworkVR
{
    public class Cutter : MonoBehaviour
    {
        [Header("Grabbbale")]
        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to this edge's grabbable object.")]
        public Grabbable grabbableObjectOrigin;
        private Collider grabbableCol;

        [Header("Cutting Position")]
        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to the position of the base of the edge.")]
        private Transform m_base;

        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to the position of the tip of the edge.")]
        private Transform m_tip;

        [Header("Cuttable Rigidbody")]
        [SerializeField]
        [Tooltip("Affects the force that the cut objects are launched with (Affected by Rigidbody mass).")]
        public float forceAppliedToCut = 10;

        public UnityEvent onObjectCut;

        private Vector3 enterBasePosition;
        private Vector3 enterTipPosition;
        private Vector3 exitTipPosition;

        void Awake()
        {
            grabbableCol = grabbableObjectOrigin.GetComponent<Collider>();
        }

        void Update()
        {
            if (grabbableObjectOrigin.isHeld)
            {
                grabbableCol.isTrigger = true;
            }
            else grabbableCol.isTrigger = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Cuttable cuttable = other.GetComponent<Cuttable>();
            if (cuttable != null && grabbableObjectOrigin.isHeld)
            {
                enterBasePosition = m_base.position;
                enterTipPosition = m_tip.position;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            exitTipPosition = m_tip.position;
            Cuttable cuttable = other.GetComponent<Cuttable>();
            if (cuttable != null && grabbableObjectOrigin.isHeld)
            {
                GeneratePlaneIntersection(other.gameObject);
                OnObjectCut();
            }
        }

        void GeneratePlaneIntersection(GameObject other)
        {
            Vector3 sideA = exitTipPosition - enterTipPosition;
            Vector3 sideB = exitTipPosition - enterBasePosition;

            Vector3 normal = Vector3.Cross(sideA, sideB).normalized;
            Vector3 transformedNormal = ((Vector3)(other.transform.localToWorldMatrix.transpose * normal)).normalized;
            Vector3 transformedPosition = other.transform.InverseTransformPoint(enterTipPosition);

            Plane plane = new Plane();

            plane.SetNormalAndPosition(
                    transformedNormal,
                    transformedPosition);

            float normalDirection = Vector3.Dot(Vector3.up, transformedNormal);

            if (normalDirection < 0)
            {
                plane = plane.flipped;
            }

            GameObject[] slices = GetSlices(plane, other);

            Rigidbody sliceArb = slices[0].GetComponent<Rigidbody>();
            Rigidbody sliceBrb = slices[1].GetComponent<Rigidbody>();
            Vector3 forceDirection = transformedNormal + Vector3.up * forceAppliedToCut;
            sliceArb.AddForce(forceDirection, ForceMode.Impulse);
            sliceBrb.AddForce(forceDirection, ForceMode.Impulse);
            Destroy(other);
        }

        GameObject[] GetSlices(Plane plane, GameObject originalObject)
        {
            Mesh mesh = originalObject.GetComponent<MeshFilter>().mesh;
            Cuttable cuttable = originalObject.GetComponent<Cuttable>();

            GameObject sliceA = CreateSliceGameObject(originalObject, "SliceA");
            GameObject sliceB = CreateSliceGameObject(originalObject, "SliceB");

            Mesh[] meshes = MeshCreator.GetMeshes(plane, mesh, cuttable.isHollow);

            sliceA.GetComponent<MeshFilter>().mesh = meshes[0];
            sliceB.GetComponent<MeshFilter>().mesh = meshes[1];

            if (!cuttable.keepSameCollider)
            {
                AddMeshCollider(ref sliceA, mesh);
                AddMeshCollider(ref sliceB, mesh);
            }

            Cuttable sliceACuttable = sliceA.GetComponent<Cuttable>();
            sliceACuttable.SetIsCut(true);
            Cuttable sliceBCuttable = sliceB.GetComponent<Cuttable>();
            sliceBCuttable.SetIsCut(true);

            return new GameObject[] { sliceA, sliceB };
        }

        GameObject CreateSliceGameObject(GameObject originalObject, string sliceName)
        {
            string orgName = originalObject.name;
            GameObject sliceGameObject = Instantiate(originalObject);
            sliceGameObject.name = orgName + sliceName;

            return sliceGameObject;
        }

        void AddMeshCollider(ref GameObject slice, Mesh mesh)
        {
            Collider oldCollider = slice.GetComponent<Collider>();
            MeshCollider meshCollider = slice.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;
            Destroy(oldCollider);
            Grabbable grab = slice.GetComponent<Grabbable>();
            if (grab != null)
            {
                grab.SetCollider(meshCollider);
            }
        }

        void OnObjectCut()
        {
            if (onObjectCut != null)
            {
                onObjectCut.Invoke();
            }
        }
    }
}