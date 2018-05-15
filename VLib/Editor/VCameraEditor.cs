using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class VCameraEditor : EditorWindow
    {

        bool OnSynch = false;
        bool OnMain = false;
        CameraSynchronizeTF csTF;

        const string openSynch = "打开摄像机跟随";
        const string closeSynch = "关闭色相机跟随";
        string synchString = "  ---- ";

        [MenuItem("Tools/V/Camera %_q")]
        static void Init()
        {
            VCameraEditor window = (VCameraEditor)EditorWindow.GetWindow<VCameraEditor>();
            window.Show();
        }



        private void OnGUI()
        {

            OnMain = Camera.main == null ? false : true;
            if (OnMain)
            {
                csTF = Camera.main.gameObject.GetComponent<CameraSynchronizeTF>();
                OnSynch = csTF == null ? false : true;
            }
            else
            {
                OnSynch = false;
            }
            synchString = OnSynch == true ? closeSynch : openSynch;


            if (GUILayout.Button(synchString))
            {
                if (!OnMain)
                {
                    Debug.Log("场景中没有 MainCamera");
                }
                else {

                    if (!OnSynch)
                    {
                        Camera.main.gameObject.AddComponent<CameraSynchronizeTF>();
                        synchString = closeSynch;
                        OnSynch = true;
                    }
                    else {
                        GameObject.DestroyImmediate(Camera.main.gameObject.GetComponent<CameraSynchronizeTF>());
                        synchString = openSynch;
                        OnSynch = false;
                    }
                    
                }
            }

            if (GUILayout.Button("搜索Camera"))
            {
                Camera cam = GameObject.FindObjectOfType<Camera>();
                if (cam != null)
                {
                    Selection.activeGameObject = cam.gameObject;
                }
                else
                {
                    EditorGUILayout.HelpBox("当前场景中没有Camera", MessageType.Warning);
                }
            }


            GUILayout.Space(20);
            GUI.color = Color.red;
            if (GUILayout.Button("清除所有Camera"))
            {
                Camera[] cams = GameObject.FindObjectsOfType<Camera>();
                foreach (Camera cam in cams)
                {
                    GameObject.DestroyImmediate(cam);
                }
            }
            GUI.color = Color.white;

        }

    }
}
