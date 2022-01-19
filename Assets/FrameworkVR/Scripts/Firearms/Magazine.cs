using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FrameworkVR
{
    public class Magazine : MonoBehaviour
    {
        [Header("Magazine Properties")]
        [SerializeField]
        [Tooltip("Loads a weapon if its type name matches the magazine's.")]
        public string firearmType = "";

        [SerializeField]
        [Tooltip("The amount of ammo given to the weapon.")]
        private int m_ammo = 0;
        public int ammo
        {
            get { return m_ammo; }
            set
            {
                m_ammo = value;
                CheckMagazineEmpty();
            }
        }

        [HideInInspector]
        [Tooltip("Read only.")]
        private bool m_isLoaded = false;
        public bool isLoaded { get => m_isLoaded; }

        [Header("References")]
        [HideInInspector]
        [Tooltip("The Gun this is magazine is loaded to.")]
        public Gun gunLoadedTo = null;

        [SerializeField]
        [Tooltip("Set the GameObject for the grabbable.")]
        public Grabbable grabbable = null;

        [Header("Optional")]
        [SerializeField]
        [Tooltip("Set the GameObject for the model when the magazine is loaded. Optional.")]
        public GameObject modelLoaded = null;

        [Header("Unity Events")]
        public UnityEvent onMagazineEmpty;

        private void OnEnable()
        {
            CheckMagazineEmpty();
        }

        void OnMagazineEmpty()
        {
            if (onMagazineEmpty != null)
            {
                onMagazineEmpty.Invoke();
            }
        }

        void CheckMagazineEmpty()
        {
            if (modelLoaded)
            {
                if (ammo > 0) modelLoaded.SetActive(true);
                else modelLoaded.SetActive(false);
            }
            OnMagazineEmpty();
        }

        public void SetIsLoaded(bool loaded, Gun gun)
        {
            m_isLoaded = loaded;

            if (gunLoadedTo != null && !loaded)
            {
                gunLoadedTo.mag = null;
            }
            gunLoadedTo = gun;
        }
    }

}
