using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class RectTransformExtensions
    {

        /// <param name="rectTransform">The RectTransform UI Element</param>
        /// <param name="deltaSize"> The Size of the RectTransform by Pixels</param>
        /// <param name="pos">Position Relative to the Screen Center</param>
        public static void Set_DeltaSize_Anchor_ScreenPos(this RectTransform rectTransform, Vector2 deltaSize, Vector2 pos)
        {
            rectTransform.sizeDelta = deltaSize;
            float width = Camera.main.pixelWidth;
            float height = Camera.main.pixelHeight;
            float x = width / 2.0f;
            float y = height / 2.0f;
            pos.x += x;
            pos.y += y;
            rectTransform.position = pos;
        }

        public static void Set_Anchor_ScreenPos(this RectTransform rectTransform)
        {
            float width = Camera.main.pixelWidth;
            float height = Camera.main.pixelHeight;
        }

        public static void Set_DeltaSize_Anchor_Left_Top(this RectTransform rectTransform)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
        }

        public static void Set_DeltaSize_Anchor_Left_Top(this RectTransform rectTransform, Vector2 pos)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            float height = Camera.main.pixelHeight;
            float rectwidth = rectTransform.sizeDelta.x;
            float rectHeight = rectTransform.sizeDelta.y;
            pos.x += rectwidth / 2.0f;
            pos.y = height - pos.y - rectHeight / 2.0f;
            rectTransform.position = pos;
        }

        public static void Set_DeltaSize_Anchor_Left_Top(this RectTransform rectTransform, Vector2 deltaSize, Vector2 pos)
        {
            rectTransform.sizeDelta = deltaSize;
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            float height = Camera.main.pixelHeight;
            pos.x += deltaSize.x / 2.0f;
            pos.y = height - pos.y - deltaSize.y / 2.0f;
            rectTransform.position = pos;
        }

        public static void Set_Match_Anchors(this RectTransform rectTransform, Vector2 min_LowerLeft, Vector2 max_UpperRight)
        {
            rectTransform.anchorMin = min_LowerLeft;
            rectTransform.anchorMax = max_UpperRight;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static void Set_Move_Anchors(this RectTransform rectTransform, Vector2 move)
        {
            Vector2 anchorMax = rectTransform.anchorMax;
            Vector2 anchorMin = rectTransform.anchorMin;
            rectTransform.anchorMax += new Vector2(move.x, move.y);
            rectTransform.anchorMin += new Vector2(move.x, move.y);
        }

        public static void Set_Match_Anchors_To_Anchors(this RectTransform rectTransform)
        {
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }
    }
}
