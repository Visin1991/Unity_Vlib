using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class DynamicLoadObject : MonoBehaviour {

    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial.shader.maximumLOD = 200;
        }
    }
    // Use this for initialization
    void Start () {
        Invoke("CreateNewWSphere", 3.0f);
	}

    private void OnDestroy()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sharedMaterial.shader.maximumLOD = 500;
        }

    }

    void CreateNewWSphere()
    {
        Debug.Log("Call");
        GameObject newObj = new GameObject("Dynamic Game Spher");
        newObj.transform.position = transform.position;
        newObj.transform.position += new Vector3(5, 0, 0);

        MeshFilter mf =  newObj.AddComponent<MeshFilter>();
        mf.sharedMesh = GetComponent<MeshFilter>().sharedMesh;

        MeshRenderer mr = newObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = Resources.Load<Material>("ShaderLOD2");

        //Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/VLib/Materials/ShaderLOD.mat");
        //if( mat != null )
        //{
        //    mr.sharedMaterial = mat;
        //}
    }
}
