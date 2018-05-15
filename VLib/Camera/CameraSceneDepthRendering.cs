using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class CameraSceneDepthRendering : MonoBehaviour
    {
        public Transform target;
        public float height;

        [ContextMenu("SetCameraToDstHeight")]
        public void SetCameraToDstHeight()
        {
            transform.position = target.transform.position;
            transform.position += new Vector3(0,height,0);
        }

        Camera cam;

        public void SetUpCamera()
        {
            if (gameObject.vCheckAndCache_Camera(ref cam))
            {
                
            }
        }
    }

}