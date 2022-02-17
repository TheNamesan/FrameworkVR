using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkVR;

namespace BoxInTheBox
{
    public class TransformGun : Gun
    {
        //public CreateGunMenu menu;
        public LayerMask propLayerMask;
        public Transform muzzlePosition;
        public Transform heldProp;
        public LineRenderer line;
        public Gradient standByGradient;
        public Gradient propDetectedGradient;
        public Gradient propHeldGradient;
        [SerializeField] private bool hadGravity;
        [SerializeField] private RigidbodyConstraints originalConstrains;
        protected override void Update()
        {
            base.Update();
            CheckLine();
        }

        private void CheckLine()
        {
            if (grabbableObjectOrigin != null)
            {
                if (grabbableObjectOrigin.isHeld)
                {
                    line.gameObject.SetActive(true);
                    if(heldProp == null)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(muzzlePosition.position, muzzlePosition.forward, out hit, Mathf.Infinity, propLayerMask.value))
                        {
                            SetLineColor(propDetectedGradient);
                        }
                        else SetLineColor(standByGradient);
                    }
                    else SetLineColor(propHeldGradient);
                }
                else
                {
                    line.gameObject.SetActive(false);
                }
            }
        }

        void SetLineColor(Gradient grad)
        {
            line.colorGradient = grad;
        }
        
        protected override void CheckTriggerHeld()
        {
            if (triggerHeld)
            {
                /*if (mag == null && !heldAfterShot)
                {
                    //menu.OpenCloseMenu();
                    heldAfterShot = true;
                }
                else*/
                if(!heldAfterShot)
                {
                   Shoot();
                }
            }
            else if(heldProp != null)
            {
                ReleaseProp();
            }
        }

        public override void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(muzzlePosition.position, muzzlePosition.forward, out hit, Mathf.Infinity, propLayerMask.value))
            {
                Debug.Log("Found Prop " + hit.transform.name);
                GrabProp(hit.transform);
            }
            cooldownTime = rateOfFire;
            heldAfterShot = true;
        }

        void GrabProp(Transform grabbedProp)
        {
            heldProp = grabbedProp;
            heldProp.parent = transform;
            Rigidbody grabRb = heldProp.GetComponent<Rigidbody>();
            if (grabRb != null)
            {
                if (grabRb.useGravity)
                {
                    hadGravity = true;
                }
                originalConstrains = grabRb.constraints;
                grabRb.constraints = RigidbodyConstraints.FreezeAll;
                grabRb.useGravity = false;
            }
        }

        void ReleaseProp()
        {
            Rigidbody grabRb = heldProp.GetComponent<Rigidbody>();
            if (hadGravity)
            {
                grabRb.useGravity = true;
                hadGravity = false;
            }
            grabRb.constraints = originalConstrains;
            heldProp.parent = null;
            heldProp = null;
        }
    }
}

