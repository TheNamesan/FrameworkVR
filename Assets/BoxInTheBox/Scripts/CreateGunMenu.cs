using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using FrameworkVR;

namespace BoxInTheBox
{
    public class CreateGunMenu : MonoBehaviour, IOpenCloseMenu
    {
        public GameObject canvas;
        public CreateGun createGun;
        public Transform buttonsParent;
        public GameObject buttonPrefab;
        public GameObject createMagPrefab;
        public void OpenCloseMenu()
        {
            canvas.SetActive(canvas.activeInHierarchy ? false : true);
            CreateMenuButtons();
        }

        public void CreateMenuButtons()
        {
            ResetMenuButtons();
            for(int i=0; i < createGun.prefabs.Count; i++)
            {
                GameObject go = Instantiate(buttonPrefab, buttonsParent);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                InitializeButton(go, i);
            }
        }

        void ResetMenuButtons()
        {
            foreach (Transform child in buttonsParent)
            {
                Destroy(child.gameObject);
            }
        }

        void InitializeButton(GameObject button, int index)
        {
            button.GetComponent<Button>().onClick.AddListener(() => {
                GameObject go = Instantiate(createMagPrefab);
                go.transform.position = transform.position;
                go.GetComponent<CreateMagazine>().objectToSpawn = createGun.prefabs[index];
                OpenCloseMenu(); 
            });
            //button.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(AssetPreview.GetAssetPreview(createGun.prefabs[index]), new Rect(0.0f, 0.0f, 128, 128), new Vector2(0.5f, 0.5f), 100.0f);
            button.transform.GetChild(0).GetComponent<Image>().sprite = createGun.prefabPreview[index];
        }
    }
}


