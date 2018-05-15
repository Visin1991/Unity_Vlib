using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VTime
    {
        public static void LinearFadeOut(ref float percent,float fadeOutTime)
        {
            float elapsePercent = (Time.deltaTime / fadeOutTime);
            percent -= elapsePercent;
        }

        public static void SmoothDampFadeOut(ref float percent, ref float currentVelocity,float fadeOutTime)
        {
            percent =  Mathf.SmoothDamp(percent, 0, ref currentVelocity, fadeOutTime);
        }
    }
}
