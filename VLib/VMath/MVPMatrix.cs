using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class MVPMatrix : MonoBehaviour
    {

        public float outlineScale = 1.1f;
        //Matrix4x4 m = Matrix4x4.identity;
        Material mat;
        Renderer m_render;
        // Use this for initialization

        void Start()
        {
            gameObject.vCheckAndCache_RenderMat(ref m_render, ref mat);
        }

        // Update is called once per frame
        void Update()
        {
            if (gameObject.vCheckAndCache_RenderMat(ref m_render, ref mat))
            {
                bool d3d = SystemInfo.graphicsDeviceVersion.IndexOf("Direct3D") > -1;
                //Matrix4x4 M = transform.localToWorldMatrix;
                Matrix4x4 M = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(outlineScale, outlineScale, outlineScale));
                Matrix4x4 V = Camera.main.worldToCameraMatrix;
                Matrix4x4 P = Camera.main.projectionMatrix;
                if (d3d)
                {
                    // Invert Y for rendering to a render texture
                    for (int i = 0; i < 4; i++)
                    {
                        P[1, i] = -P[1, i];
                    }
                    // Scale and bias from OpenGL -> D3D depth range
                    for (int i = 0; i < 4; i++)
                    {
                        P[2, i] = P[2, i] * 0.5f + P[3, i] * 0.5f;
                    }
                }
                Matrix4x4 MVP = P * V * M;

                mat.SetMatrix("V_MVP", MVP);
            }
        }
    }
}
