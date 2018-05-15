using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class SaveGamePanel : UIPanel
    {
        public override GameUIPr.PanelType GetPanelType()
        {
            return GameUIPr.PanelType.Setting;
        }
    }
}
