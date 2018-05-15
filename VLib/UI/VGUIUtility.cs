using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VGUIUtility
    {

        public static Rect Rect_Create(float PosX, float PosY, float sizeX, float sizeY)
        {
            return new Rect(new Vector2(PosX, PosY), new Vector2(sizeX, sizeY));
        }
    }
}
