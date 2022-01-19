using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameworkVR
{
    public class Chamber : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to the position of the loaded magazine.")]
        public Transform magTransform;

        [SerializeField]
        [Tooltip("Set the GameObject that corresponds to the gun.")]
        public Gun gunOrigin;

        [SerializeField]
        [Tooltip("Currently loaded magazine.")]
        public Magazine heldItem;

        private void OnTriggerStay(Collider other)
        {
            ChamberDetector tmp = other.GetComponent<ChamberDetector>();
            if (tmp != null)
            {
                if (tmp.magazineOrigin.firearmType == gunOrigin.firearmType &&
                    !tmp.magazineOrigin.grabbable.isHeld && !tmp.magazineOrigin.isLoaded)
                {
                    tmp.magazineOrigin.SetIsLoaded(true, gunOrigin);
                    tmp.magazineOrigin.grabbable.Hold(gunOrigin.gameObject);
                    tmp.magazineOrigin.transform.position = magTransform.position;
                    tmp.magazineOrigin.transform.rotation = magTransform.rotation;
                    tmp.magazineOrigin.transform.parent = magTransform;
                    gunOrigin.mag = tmp.magazineOrigin;
                }
            }
        }
    }

}
