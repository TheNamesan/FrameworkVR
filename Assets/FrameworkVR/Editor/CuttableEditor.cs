using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FrameworkVR
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Cuttable))]
    public class CuttableEditor : Editor
    {
        bool showDebug = false;
        bool enableModification = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("isHollow"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("keepSameCollider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("noCollisionOnCut"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onCut"));

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if (showDebug)
            {
                EditorGUI.indentLevel++;
                enableModification = EditorGUILayout.Toggle("Enable Modification", enableModification);
                if (!enableModification) GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_isCut"));
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}