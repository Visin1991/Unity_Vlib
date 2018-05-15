using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{

    public class ShaderLODTest : MonoBehaviour
    {
        public float switchDistance = 30f;
        Renderer m_renderer;
        Material m_mat;
        // Update is called once per frame
        void Update()
        {
            //if (RenderUtil.CacheRenderMat(gameObject, ref m_renderer, ref m_mat))
            //{
            //    if(gameObject.DistanceToMainCamera() > switchDistance)
            //        m_mat.shader.maximumLOD = 200;
            //    else
            //        m_mat.shader.maximumLOD = 500;
            //}

        }
    }
}
