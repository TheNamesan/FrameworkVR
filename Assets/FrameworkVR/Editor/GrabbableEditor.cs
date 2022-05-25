using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Grabbable))]
    public class GrabbableEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

           
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasGrabPoints"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enableWeightSystem"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectHold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onObjectRelease"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if (!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isHeld"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("holder"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("holderCol"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}