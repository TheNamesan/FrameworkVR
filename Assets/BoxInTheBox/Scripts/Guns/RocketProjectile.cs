using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrameworkVR;

namespace BoxInTheBox
{
    public class RocketProjectile : MonoBehaviour
    {
        Rigidbody rb;
        public float speed = 0;
        public GameObject model;
        public ParticleSystem effect;
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        void Start()
        {
            Launch();
        }

        public void Launch()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
        }

        public void Explotion()
        {
            model.SetActive(false);
            StartCoroutine(ExplotionEffect());

        }

        IEnumerator ExplotionEffect()
        {
            effect.Play();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            yield return new WaitForSeconds(effect.main.duration);
            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Explotion();
        }
    }
}

