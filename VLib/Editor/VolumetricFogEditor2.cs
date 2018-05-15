using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    [CustomEditor(typeof(VolumetricFog2))]
    public class VolumetricFogEditor2 : Editor
    {
        VolumetricFog2 fog;
        SerializedProperty m_mat;

        private void OnEnable()
        {
            fog = (VolumetricFog2)target;
            m_mat = serializedObject.FindProperty("fogMat");
        }

        public override void OnInspectorGUI()
        {
            if (fog == null) return;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            //GUILayout.Label(new GUIContent("Fog Metarial"), GUILayout.Width(120));
            EditorGUILayout.PropertyField(m_mat, new GUIContent("Fog Metarial"), GUILayout.Height(20));
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("开始距离"), GUILayout.Width(120));
            fog.distance = EditorGUILayout.Slider(fog.distance, 0f, 1000f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("可见距离"), GUILayout.Width(120));
            fog.maxDistance = EditorGUILayout.Slider(fog.maxDistance, 0f, 1000f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("雾强度"), GUILayout.Width(120));
            fog.intensity = EditorGUILayout.Slider(fog.intensity, 0f, 5.0f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("最大Opacity"), GUILayout.Width(120));
            fog.min = EditorGUILayout.Slider(fog.min, 0f, 1.0f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("体积雾高度中心"), GUILayout.Width(120));
            fog.baseHeight = EditorGUILayout.Slider(fog.baseHeight, 0f, 1000f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("体积雾高度半径"), GUILayout.Width(120));
            fog.heightRadius = EditorGUILayout.Slider(fog.heightRadius, 0f, 1000f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("雾颜色"), GUILayout.Width(120));
            fog.fogColor = EditorGUILayout.ColorField(fog.fogColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Sun Tint"), GUILayout.Width(120));
            fog._SunTint = EditorGUILayout.Slider(fog._SunTint,0.0f,1.0f);
            EditorGUILayout.EndHorizontal();


            if (EditorGUI.EndChangeCheck())
            {
                fog.ReSetMatValue();
            }

        }

    }
}
