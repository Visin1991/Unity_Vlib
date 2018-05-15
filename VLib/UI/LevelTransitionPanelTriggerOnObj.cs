using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class LevelTransitionPanelTriggerOnObj : MonoBehaviour
    {

        private void OnMouseDown()
        {

            if (Input.GetMouseButton(0))
            {
                GameUIPr.Instance.OpenLevelPanel();
            }
        }
    }
}
