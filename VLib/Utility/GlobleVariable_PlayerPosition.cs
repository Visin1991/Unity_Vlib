using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GlobleVariable_PlayerPosition : MonoBehaviour {

    // Update is called once per frame
    void Update () {
        Shader.SetGlobalVector("_PlayerPosition", transform.position);
    }
}
