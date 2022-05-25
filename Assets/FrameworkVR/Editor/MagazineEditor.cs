using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Magazine))]
    public class MagazineEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("firearmType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ammo"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("grabbable"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("modelLoaded"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onMagazineEmpty"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if (!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isLoaded"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("gunLoadedTo"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
