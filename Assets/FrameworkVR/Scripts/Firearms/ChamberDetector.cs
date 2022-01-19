using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkVR
{
    public class ChamberDetector : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to the magazine.")]
        public Magazine magazineOrigin;
    }
}

