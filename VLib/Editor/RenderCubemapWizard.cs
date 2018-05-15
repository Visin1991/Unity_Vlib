﻿using UnityEngine;
using UnityEditor;
using System.Collections;

namespace ProjectK.Graphics.V
{
    public class RenderCubemapWizard : ScriptableWizard
    {
        public Transform renderFromPosition;
        public Cubemap cubemap;

        void OnWizardCreate()
        {
            // create temporary camera for rendering
            GameObject go = new GameObject("CubemapCamera");
            go.AddComponent<Camera>();
            // place it on the object
            go.transform.position = renderFromPosition.position;
            go.transform.rotation = Quaternion.identity;
            // render into cubemap
            go.GetComponent<Camera>().RenderToCubemap(cubemap);

            // destroy temporary camera
            DestroyImmediate(go);
        }

        [MenuItem("GameObject/Render into Cubemap")]
        static void RenderCubemap()
        {
            ScriptableWizard.DisplayWizard<RenderCubemapWizard>(
                "Render cubemap", "Render!");
        }
    }
}