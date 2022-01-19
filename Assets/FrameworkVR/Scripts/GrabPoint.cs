using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkVR
{
    public class GrabPoint : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Set the grabbable GameObject this grab point belongs to.")]
        public Grabbable grabbableObjectOrigin;
    }
}
