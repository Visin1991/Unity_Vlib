using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ProjectK.Graphics.V
{
    [CustomEditor(typeof(Water2018))]
    [CanEditMultipleObjects]
    public class Water2018Editor : Editor
    {
        //Water2018 water2018;
        //SerializedProperty refType;
        //SerializedProperty useDepth;
        //SerializedProperty depthInfoType;
        //SerializedProperty depthRenderType;
        //SerializedProperty renderDepthOnly;
        //SerializedProperty gui_Debug_FSRT;
        //SerializedProperty gui_Debug_R16;
        //SerializedProperty gui_Debug_RefleRT;
        //SerializedProperty gui_Debug_RefraRT;
        //SerializedProperty waveOff;
        //bool showDebug = false;

        //private void OnEnable()
        //{
        //    water2018 = target as Water2018;
        //    refType = serializedObject.FindProperty("refType");
        //    waveOff = serializedObject.FindProperty("waveOff");
        //    useDepth = serializedObject.FindProperty("useDepth");
        //    depthInfoType = serializedObject.FindProperty("depthInfoType");
        //    depthRenderType = serializedObject.FindProperty("depthRenderType");
        //    renderDepthOnly = serializedObject.FindProperty("renderDepthOnly");
        //    gui_Debug_FSRT = serializedObject.FindProperty("gui_Debug_FSRT");
        //    gui_Debug_R16 = serializedObject.FindProperty("gui_Debug_R16");
        //    gui_Debug_RefleRT = serializedObject.FindProperty("gui_Debug_RefleRT");
        //    gui_Debug_RefraRT = serializedObject.FindProperty("gui_Debug_RefraRT");
        //}

        //public override void OnInspectorGUI()
        //{
        //    serializedObject.Update();
        //    ShaderDebug();
        //    SetReflectionType();
        //    FullScreenRTRender();
        //    RealTime_Reflection_Refraction();
        //}

        //void SetReflectionType()
        //{
        //    GUILayout.BeginVertical("Box");
        //    {
        //        EditorGUI.BeginChangeCheck();
        //        EditorGUILayout.PropertyField(refType, new GUIContent("Reflection Type"));
        //        if (EditorGUI.EndChangeCheck())
        //        {
        //            serializedObject.ApplyModifiedProperties();
        //            water2018.ResetReflectionType();
        //        }
        //    }
        //    GUILayout.EndVertical();
        //}

        //void FullScreenRTRender()
        //{
        //    GUILayout.BeginVertical("Box");
        //    {
        //        if (refType.enumValueIndex == (int)FlageWaterRefType.FullScreenRT)
        //        {
        //            EditorGUI.BeginChangeCheck();
        //            EditorGUILayout.PropertyField(useDepth, new GUIContent("Use Depth"));
        //            if (EditorGUI.EndChangeCheck())
        //            {
        //                serializedObject.ApplyModifiedProperties();
        //                water2018.SetDepthOnKeyWorld();
        //            }
        //            Set_Depth_Info_Render_Type();
        //        }
        //    }
        //    GUILayout.EndVertical();
        //}

        //void RealTime_Reflection_Refraction()
        //{  
        //    GUILayout.BeginVertical("Box");
        //    {
        //        if (refType.enumValueIndex == (int)FlageWaterRefType.RealTime_Water)
        //        {
        //            EditorGUI.BeginChangeCheck();
        //            EditorGUILayout.PropertyField(useDepth, new GUIContent("Use Depth"));
        //            if (EditorGUI.EndChangeCheck())
        //            {
        //                serializedObject.ApplyModifiedProperties();
        //                water2018.SetDepthOnKeyWorld();
        //            }
        //            Set_Depth_Info_Render_Type();
        //        }
        //    }
        //    GUILayout.EndVertical();
        //}

        //void Set_Depth_Info_Render_Type()
        //{
        //    if (water2018.useDepth == true)
        //    {
        //        EditorGUI.BeginChangeCheck();
        //        EditorGUILayout.PropertyField(depthInfoType, new GUIContent("Depth Info Type"));
        //        if (EditorGUI.EndChangeCheck())
        //        {
        //            serializedObject.ApplyModifiedProperties();
        //            water2018.SetDepthInfo();
        //        }
                 
        //        //Depth Alpha 8, Do not support Related Depth Rendering
        //        if (water2018.depthInfoType == DepthInfoType.Depth_A8)
        //        {
        //            water2018.depthRenderType = DepthRenderType.RawDepthRender;
        //        }
        //        else
        //        {
        //            EditorGUI.BeginChangeCheck();
        //            EditorGUILayout.PropertyField(depthRenderType, new GUIContent("Depth Rendering Type"));
        //            if (EditorGUI.EndChangeCheck())
        //            {
        //                serializedObject.ApplyModifiedProperties();
        //                water2018.SetDepthRenderingType();
        //            }
        //        }
        //    }
        //}

        //void ShaderDebug()
        //{         
        //    GUILayout.BeginVertical("Box");
        //    {
        //        GUI.color = Color.red;
        //        showDebug = EditorGUILayout.Foldout(showDebug, "Water Debug");
        //        if (showDebug)
        //        {
        //            EditorGUI.BeginChangeCheck();
        //            EditorGUILayout.PropertyField(waveOff, new GUIContent("Water Wave Toggle"));
        //            EditorGUILayout.PropertyField(renderDepthOnly, new GUIContent("Render Depth Only"));
        //            EditorGUILayout.PropertyField(gui_Debug_R16, new GUIContent("Show Depth Texture"));
        //            FullScreenDebug();
        //            RealTime_Refle_Refra_Debug();
        //            if (EditorGUI.EndChangeCheck())
        //            {
        //                serializedObject.ApplyModifiedProperties();
        //                water2018.WaterDebug();
        //            }
        //        }
        //        GUI.color = Color.white;
        //    }
        //    GUILayout.EndVertical();
        //    GUILayout.Space(10);
        //}

        //void FullScreenDebug()
        //{
        //    if (refType.enumValueIndex == (int)FlageWaterRefType.FullScreenRT)
        //    {
        //        //Open Full Screen RT
        //        EditorGUILayout.PropertyField(gui_Debug_FSRT, new GUIContent("Show FullScreen RT"));
        //        //Close.....
        //        water2018.gui_Debug_RefleRT = false;
        //        water2018.gui_Debug_RefraRT = false;
        //    }
        //}

        //void RealTime_Refle_Refra_Debug()
        //{
        //    if (refType.enumValueIndex == (int)FlageWaterRefType.RealTime_Water)
        //    {
        //        //Open Full Screen RT
        //        EditorGUILayout.PropertyField(gui_Debug_RefleRT, new GUIContent("Show Reflection RT"));
        //        EditorGUILayout.PropertyField(gui_Debug_RefraRT, new GUIContent("Show Refraction RT"));

        //        //Close.....
        //        water2018.gui_Debug_FSRT = false;
        //    }
        //}

    }
}
