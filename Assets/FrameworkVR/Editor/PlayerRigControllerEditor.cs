using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

namespace FrameworkVR
{
    [CustomEditor(typeof(PlayerRigController))]
    public class PlayerRigControllerEditor : Editor
    {
        bool showDebug = false;
        override public void OnInspectorGUI()
        {
            GUI.enabled = false;
            SerializedProperty prop = serializedObject.FindProperty("m_Script");
            EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
            GUI.enabled = true;

            SerializedProperty acs = serializedObject.FindProperty("m_ActionAsset");
            EditorGUILayout.PropertyField(acs);
            SerializedProperty jin = serializedObject.FindProperty("jumpInput");
            EditorGUILayout.PropertyField(jin);
            SerializedProperty spd = serializedObject.FindProperty("playerSpeed");
            EditorGUILayout.PropertyField(spd);
            SerializedProperty rtm = serializedObject.FindProperty("rotationMode");
            EditorGUILayout.PropertyField(rtm);
            SerializedProperty tsp = serializedObject.FindProperty("playerTurnSpeed");
            EditorGUILayout.PropertyField(tsp);
            SerializedProperty rtd = serializedObject.FindProperty("rotationDegree");
            EditorGUILayout.PropertyField(rtd);
            SerializedProperty jfc = serializedObject.FindProperty("jumpForce");
            EditorGUILayout.PropertyField(jfc);
            SerializedProperty phe = serializedObject.FindProperty("m_playerHeight");
            EditorGUILayout.PropertyField(phe);
            SerializedProperty rhe = serializedObject.FindProperty("rigHeight");
            EditorGUILayout.PropertyField(rhe);
            SerializedProperty grv = serializedObject.FindProperty("m_gravity");
            EditorGUILayout.PropertyField(grv);
            SerializedProperty gly = serializedObject.FindProperty("groundLayers");
            EditorGUILayout.PropertyField(gly);
            SerializedProperty tgr = serializedObject.FindProperty("touchingGround");
            EditorGUILayout.PropertyField(tgr);

            showDebug = EditorGUILayout.BeginFoldoutHeaderGroup(showDebug, "Debug");
            if(showDebug)
            {
                EditorGUI.indentLevel++;
                GUI.enabled = false;
                SerializedProperty dwl = serializedObject.FindProperty("disableWalk");
                EditorGUILayout.PropertyField(dwl);
                SerializedProperty dgr = serializedObject.FindProperty("disableGravity");
                EditorGUILayout.PropertyField(dgr);
                SerializedProperty drt = serializedObject.FindProperty("disableRotation");
                EditorGUILayout.PropertyField(drt);
                SerializedProperty rtv = serializedObject.FindProperty("rotationValue");
                EditorGUILayout.PropertyField(rtv);
                GUI.enabled = true;
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
            
    }
}


