using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HandFollower))]
    public class HandFollowerEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("handToTrack"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("handController"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerLayer"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if(!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("positionSmooth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationSmooth"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
