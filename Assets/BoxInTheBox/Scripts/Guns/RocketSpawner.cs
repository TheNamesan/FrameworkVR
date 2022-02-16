using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkVR;

namespace BoxInTheBox
{
    public class RocketSpawner : MonoBehaviour
    {
        public GameObject rocketPrefab;
        public Transform rocketPosition;
        public Gun gun;
        public float speed = 0;
        public void LaunchRocket()
        {
            Debug.Log("Pew");
            GameObject rocket = Instantiate(rocketPrefab);
            rocket.transform.position = rocketPosition.position;
            rocket.transform.rotation = transform.rotation;
            rocket.GetComponent<RocketProjectile>().speed = speed;
            Magazine tmpMag = gun.mag;
            gun.mag.SetIsLoaded(false, null);
            Destroy(tmpMag.gameObject);
        }
    }
}

