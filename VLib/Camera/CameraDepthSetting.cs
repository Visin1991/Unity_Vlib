using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraDepthSetting : MonoBehaviour {

   public DepthTextureMode mode = DepthTextureMode.Depth;
	// Use this for initialization
	void Start () {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = mode;
    }
	
}
