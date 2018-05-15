using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
	public class UIHelper
	{

        /// <summary>
        ///     Set the four corner of the rectTransform to it's four Anchors
        /// when we set offsetMax and offsetMin to Vector2.zero; the corner and the size
        /// of the UI element will be fully match to its Anchors
        /// </summary>
        /// <param name="rectTransform"></param>
        [System.Obsolete("This Method is Obsolet, Please Use the RectTransform Extension method: 'Set_Match_Anchors_To_Anchors' instead")]
        public static void MatchCornersToAnchors(ref RectTransform rectTransform)
		{
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.offsetMin = Vector2.zero;
		}

		/// <summary>
		/// Move the RectTransform...
		/// </summary>
		/// <param name="rectTransform"> the rectTransform we wanna to move</param>
		/// <param name="move">the value of move should be in the range[0,1]. It's like a the UV which related to rectTransform's parent rectTransform</param>
        [System.Obsolete("This Method is Obsolet, Please Use the RectTransform Extension method:  'Set_Move_Anchors' instead")]
		public static void MoveAnchors(ref RectTransform rectTransform, Vector2 move)
		{
			Vector2 anchorMax = rectTransform.anchorMax;
			Vector2 anchorMin = rectTransform.anchorMin;
			rectTransform.anchorMax += new Vector2(move.x, move.y);
			rectTransform.anchorMin += new Vector2(move.x, move.y);
		}

        [System.Obsolete("This Method is Obsolet, Please Use the RectTransform Extension method: 'Set_Match_Anchors' instead")]
        public static void SetAndMatchAnchors(ref RectTransform rectTransform,Vector2 min_LowerLeft, Vector2 max_UpperRight)
        { 	
			rectTransform.anchorMin = min_LowerLeft;
            rectTransform.anchorMax = max_UpperRight;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public struct VAnchorRect
        {
            public Vector2 min_LowerLeft;
            public Vector2 max_UpperRight;
            public VAnchorRect(Vector2 lowerLeft, Vector2 upperRight)
            {
                min_LowerLeft = lowerLeft;
                max_UpperRight = upperRight;
            }

            public VAnchorRect(float left, float down, float right, float up)
            {
                min_LowerLeft = new Vector2(left, down);
                max_UpperRight = new Vector2(right, up);
            }

            public void MoveHorizontal(float value)
            {
                min_LowerLeft.x += value;
                max_UpperRight.x += value;
            }

            public void MoveVertical(float value)
            {
                min_LowerLeft.y += value;
                max_UpperRight.y += value;
            }

            /// <summary>
            /// Keep the right Static, and scale from the Left to Right
            /// </summary>
            public void ScaleHorizontal_Left(float value)
            {
                float length =  max_UpperRight.x - min_LowerLeft.x ;
                min_LowerLeft.x += length *(1 - value);
            }

            /// <summary>
            /// Keep the Left Static, and scale from the Right to Left
            /// </summary>
            public void ScaleHorizontal_Right(float value)
            {
                float length = max_UpperRight.x - min_LowerLeft.x;
                max_UpperRight.x -= length * (1 - value);
            }
            
            public void ScaleHorizontal_Mid(float value)
            {
                float length = max_UpperRight.x - min_LowerLeft.x;
                float deltaLength = length * (1 - value) * 0.5f;
                min_LowerLeft.x += deltaLength;
                max_UpperRight.x -= deltaLength;
            }

            public void ScaleVertical_Down(float value)
            {
                float length = max_UpperRight.y - min_LowerLeft.y;
                min_LowerLeft.y += length * (1 - value);
            }

            public void ScaleVertical_Up(float value)
            {
                float length = max_UpperRight.y - min_LowerLeft.y;
                max_UpperRight.y -= length * (1 - value);
            }

            public void ScaleVertical_Mid(float value)
            {
                float length = max_UpperRight.y - min_LowerLeft.y;
                float deltaLength = length * (1 - value) * 0.5f;
                min_LowerLeft.y += deltaLength;
                max_UpperRight.y -= deltaLength;
            }

            public void ScaleUnifom(float value)
            {
                ScaleHorizontal_Mid(value);
                ScaleVertical_Mid(value);
            }

            public static VAnchorRect Fill { get { return new VAnchorRect(0, 0, 1, 1); } }
        }

	}

   
}