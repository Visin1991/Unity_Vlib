using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class SettingPanel : UIPanel
    {
        public override GameUIPr.PanelType GetPanelType()
        {
            return GameUIPr.PanelType.Setting;
        }

        [ContextMenu("Fixed Pixel Size")]
        public void FixedPixels()
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(200f, 200f);
        }

        public void FixedPixels(float width, float height)
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector3(width, height);
        }
    }
}
