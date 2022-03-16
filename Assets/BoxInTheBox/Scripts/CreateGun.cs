using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using FrameworkVR;

namespace BoxInTheBox
{
    public class CreateGun : Gun
    {
        public List<GameObject> prefabs;
        public CreateGunMenu menu;
        public Transform muzzlePosition;
        protected override void GetHandController()
        {
            base.GetHandController();
        }
        protected override void CheckTriggerHeld()
        {
            if (triggerHeld && cooldownTime <= 0)
            {
                if (mag == null && !heldAfterShot)
                {
                    menu.OpenCloseMenu();
                    heldAfterShot = true;
                }
                else
                {
                    if (rateOfFire <= 0)
                    {
                        if (!heldAfterShot) Shoot();
                    }
                    else Shoot();
                }
            }
        }

        public override void Shoot()
        {
            RaycastHit hit;
            if(Physics.Raycast(muzzlePosition.position, muzzlePosition.forward, out hit, Mathf.Infinity))
            {
                GameObject objectToSpawn = mag.GetComponent<CreateMagazine>().objectToSpawn;
                if(objectToSpawn != null)
                {
                    GameObject go = Instantiate(objectToSpawn, hit.point + new Vector3(0, 0.1f, 0), transform.rotation);
                }
            }            
            cooldownTime = rateOfFire;
            heldAfterShot = true;
        }
    }
}

