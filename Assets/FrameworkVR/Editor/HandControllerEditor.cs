using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandController))]
    public class HandControllerEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ActionAsset"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handSide"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("holdForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("throwForce"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("characterRig"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handCol"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableWeightSystem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("grabRumbleAmplitude"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("grabRumbleDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseRumbleAmplitude"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("releaseRumbleDuration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handModel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectHold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectRelease"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if (!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gripForce"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerForce"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heldItem"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("heldItemMass"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
