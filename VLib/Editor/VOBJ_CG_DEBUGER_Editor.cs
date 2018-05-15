using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class VOBJ_CG_DEBUGER_Editor : EditorWindow
    {
        GameCentalPr centralPr;

        [MenuItem("Tools/V/GraphycsEditor")]
        static void Init()
        {
            VOBJ_CG_DEBUGER_Editor window = (VOBJ_CG_DEBUGER_Editor)EditorWindow.GetWindow<VOBJ_CG_DEBUGER_Editor>();
            window.Show();
        }


        private void OnGUI()
        {
            CheckAndCache();

            if (GUILayout.Button("物件信息 GUI"))
            {
                Toggle();
            }

            
        }

        bool ToggleOn = false;

        void Toggle()
        {
            if (ToggleOn)
            {
                Release();
                ToggleOn = false;
            }
            else
            {

                OBJ_CG_Debuger debuger_GUI = centralPr.GetComponentInChildren<OBJ_CG_Debuger>();
                if (debuger_GUI == null)
                {
                    GameObject obj = new GameObject("OBJ_CG_Debuger");
                    obj.transform.SetParent(centralPr.transform);
                    debuger_GUI = obj.AddComponent<OBJ_CG_Debuger>();
                    debuger_GUI.Create_Item_Graphics_Info_GUIPanel(); 
                }
                ToggleOn = true;
            }
        }

        void Release()
        {
            CheckAndCache();

            OBJ_CG_Debuger debuger = centralPr.GetComponentInChildren<OBJ_CG_Debuger>();
            if (debuger != null)
            {
                debuger.Editor_Release();
                DestroyImmediate(debuger.gameObject);
            }
        }

        void CheckAndCache()
        {
            if (centralPr == null)
            {
                centralPr = GameObject.FindObjectOfType<GameCentalPr>();

                if (centralPr == null)
                {
                    GameObject obj = new GameObject("GameManagers");
                    obj.AddComponent<GameCentalPr>();
                }
            }
        }
    }

}