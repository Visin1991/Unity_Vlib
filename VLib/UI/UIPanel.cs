using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public abstract class UIPanel : MonoBehaviour
    {
        public abstract GameUIPr.PanelType GetPanelType();
    }
}