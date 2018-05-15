using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class UiEditorHelper : MonoBehaviour
    {

        [ContextMenu("Alling Anchor")]
        void AlingTheAnchor()
        {
            RectTransform rt = GetComponent<RectTransform>();
            //V.UIHelper.MatchCornersToAnchors(ref rt);
            rt.Set_Match_Anchors_To_Anchors();
        }
    }
}
