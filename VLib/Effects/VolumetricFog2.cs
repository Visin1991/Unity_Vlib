using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [ExecuteInEditMode]
    public class VolumetricFog2 : MonoBehaviour
    {

        public Material fogMat;

        public float distance;
        public float maxDistance = 300;
        public float baseHeight;
        public float heightRadius;
        public Color fogColor;
        public float intensity = 1.0f;
        public float min = 0.95f;
        public float _SunTint = 0.5f;


        private void OnEnable()
        {
            //fogMat = Instantiate(Resources.Load<Material>("Materials/VolumetricFog2"));
            //fogMat.hideFlags = HideFlags.DontSave;
            ReSetMatValue();
        }

        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (!enabled || fogMat == null)
            {
                UnityEngine.Graphics.Blit(source, destination); return;
            }
            fogMat.SetMatrix("_ClipToWorld", Camera.main.cameraToWorldMatrix * Camera.main.projectionMatrix.inverse);
            UnityEngine.Graphics.Blit(source, destination, fogMat, 0);
        }

        public void ReSetMatValue()
        {
            fogMat.SetFloat("_FogDistance", distance);
            fogMat.SetFloat("_FogBaseHeight", baseHeight);
            fogMat.SetFloat("_FogHeight", heightRadius);
            fogMat.SetColor("_Color", fogColor);
            fogMat.SetFloat("_Intensity", intensity);
            fogMat.SetFloat("_FogMaxDistance", maxDistance);
            fogMat.SetFloat("_Min", min);
            fogMat.SetFloat("_SunTint", _SunTint); 
        }
    }
}
