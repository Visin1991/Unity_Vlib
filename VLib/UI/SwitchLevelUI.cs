using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class SwitchLevelUI : MonoBehaviour
    {

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameUIPr.Instance.OpenLevelPanel();
            }
        }
    }
}
