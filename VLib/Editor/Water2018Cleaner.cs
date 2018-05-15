//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class Water2018Cleaner : EditorWindow {


//    [MenuItem("Tools/zzzzz清除场景Water2018脚本")]
//    static void Init()
//    {
//        Water2018Cleaner window = (Water2018Cleaner)EditorWindow.GetWindow(typeof(Water2018Cleaner));
//        window.Show();
//    }

//    private void OnGUI()
//    {
//        if (GUILayout.Button("搜寻"))
//        {
//            ProjectK.Graphics.V.Water2018 water2018 = GameObject.FindObjectOfType<ProjectK.Graphics.V.Water2018>();
//            if (water2018 != null)
//            {
//                Selection.activeGameObject = water2018.gameObject;
//            }
//        }

//        if (GUILayout.Button("一键清除"))
//        {
//            ProjectK.Graphics.V.Water2018[] water2018s = GameObject.FindObjectsOfType<ProjectK.Graphics.V.Water2018>();
//            foreach (ProjectK.Graphics.V.Water2018 w in water2018s)
//            {
//                Object.DestroyImmediate(w);
//            }
//        }
//    }
//}
