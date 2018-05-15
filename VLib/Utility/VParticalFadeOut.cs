using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public class VParticalFadeOut : MonoBehaviour {

        public float fadeOutTime = 5.0f;

        [HideInInspector]
        public ParticleSystem[] particleSystems;

        [ContextMenu("Cache Partical System")]
        void CheckAndCacheParticalSystem()
        {
            if (gameObject.vCheckAndCache_Components_InChildren(ref particleSystems))
            {
                IEnumerator coroutine = FadeOutThePartical();
                StartCoroutine(coroutine);
            }
        }

        IEnumerator FadeOutThePartical()
        {

            int psCount = particleSystems.Length;
            Color[] colors = new Color[psCount];

            int index = 0;
            foreach(ParticleSystem ps in particleSystems)
            {
                colors[index] = ps.main.startColor.color;
                index += 1;
            }

            float percent = 1.0f;
            //float currentVelocity = 0.0f;
            
            while (percent > 0.05f)
            {
                VTime.LinearFadeOut(ref percent, fadeOutTime);
                //VTime.SmoothDampFadeOut(ref percent, ref currentVelocity, fadeOutTime);

                for (int i = 0; i < particleSystems.Length; i++)
                {
                    ParticleSystem.MainModule main = particleSystems[i].main;
                    main.startColor = colors[i] * percent;
                }

                yield return null;
            }   
        }

    }
}
