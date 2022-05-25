using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Gun))]
    public class GunEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_actionAsset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unloadInput"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("holdForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rateOfFire"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("firearmType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("grabbableObjectOrigin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("muzzleFlash"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onShoot"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onReload"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if (!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerForce"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_mag"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}