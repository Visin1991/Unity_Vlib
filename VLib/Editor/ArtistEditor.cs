using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public class ArtistEditor : EditorWindow
    {
        static Texture headTexture;
        static Texture emailTexture;

        [MenuItem("Tools/V/Artist/ Artist Helper Doc")]
        static void CreateWindow()
        {
            EditorWindow.GetWindow<ArtistEditor>();
            headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
            emailTexture = Resources.Load<Texture>("EditorWindowTextures/emailIcon");
        }

        private void OnGUI()
        {
            Header();

            GUILayout.Space(30);

            if (GUILayout.Button("FBX export guide"))
            {
                Application.OpenURL("https://docs.unity3d.com/Manual/HOWTO-exportFBX.html");
            }

            if (GUILayout.Button("How to make a Prefab"))
            {
                Application.OpenURL("https://youtu.be/vzjWzUENGzQ");
            }


        }

        void Header()
        {
            GUILayout.BeginHorizontal();
            {
                //Load Textures...............
                if (headTexture == null || emailTexture == null)
                {
                    headTexture = Resources.Load<Texture>("EditorWindowTextures/headTexture");
                    if (headTexture == null) { Debug.Log("No headTexture"); }
                    emailTexture = Resources.Load<Texture>("EditorWindowTextures/emailIcon");
                    if (emailTexture == null) { Debug.Log("No emailTexture"); }
                }

                //Draw Head
                GUI.DrawTexture(new Rect(10, 10, 75, 75), headTexture);
                GUILayout.Space(90);

                //Draw my Information
                GUILayout.BeginVertical();
                {
                    GUILayout.Space(10);

                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.Label("「Informations」", EditorStyles.boldLabel);

                        GUILayout.BeginHorizontal();
                        {
                            GUI.DrawTexture(new Rect(95, 35, 15, 15), emailTexture);
                            GUILayout.Space(25);
                            GUILayout.Label("zhuzhanhao1991@Gmail.com");
                        }
                        GUILayout.EndHorizontal();

                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();


            }
            GUILayout.EndHorizontal();
        }
    }
}
