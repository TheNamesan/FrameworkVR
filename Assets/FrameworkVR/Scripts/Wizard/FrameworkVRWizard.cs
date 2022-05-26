using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    public class FrameworkVRWizard
    {
        private const string wizardPrefabsPath = "Assets/FrameworkVR/Scripts/Wizard/WizardPrefabs.asset";
        private static WizardPrefabs LocatePrefabs()
        {
            return AssetDatabase.LoadAssetAtPath<WizardPrefabs>(wizardPrefabsPath);
        }

        [MenuItem("FrameworkVR/Create Grabbable Template")]
        public static void CreateGrabbable()
        {
            WizardPrefabs prefabs = LocatePrefabs();
            if (!prefabs)
            {
                Debug.LogWarning($"WizardPrefabs not found at path {wizardPrefabsPath}");
                return;
            }
            CreateItem(prefabs.grabbable);
        }
        [MenuItem("FrameworkVR/Create Cutter Template")]
        public static void CreateCutter()
        {
            WizardPrefabs prefabs = LocatePrefabs();
            if (!prefabs)
            {
                Debug.LogWarning($"WizardPrefabs not found at path {wizardPrefabsPath}");
                return;
            }
            CreateItem(prefabs.cutter);
        }
        [MenuItem("FrameworkVR/Create Cuttable Template")]
        public static void CreateCuttable()
        {
            WizardPrefabs prefabs = LocatePrefabs();
            if (!prefabs)
            {
                Debug.LogWarning($"WizardPrefabs not found at path {wizardPrefabsPath}");
                return;
            }
            CreateItem(prefabs.cuttable);
        }
        [MenuItem("FrameworkVR/Create Firearm Template")]
        public static void CreateGun()
        {
            WizardPrefabs prefabs = LocatePrefabs();
            if (!prefabs)
            {
                Debug.LogWarning($"WizardPrefabs not found at path {wizardPrefabsPath}");
                return;
            }
            CreateItem(prefabs.gun);
        }
        [MenuItem("FrameworkVR/Create Magazine Template")]
        public static void CreateMag()
        {
            WizardPrefabs prefabs = LocatePrefabs();
            if (!prefabs)
            {
                Debug.LogWarning($"WizardPrefabs not found at path {wizardPrefabsPath}");
                return;
            }
            CreateItem(prefabs.mag);
        }

        public static void CreateItem(Object item)
        {
            var instance = PrefabUtility.InstantiatePrefab(item);
            Undo.RegisterCreatedObjectUndo(instance, $"Create {instance.name}");
            Selection.activeObject = instance;
        }
    }
}
