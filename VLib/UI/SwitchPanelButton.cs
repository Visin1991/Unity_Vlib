using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    //This is the Setting Button in the Main Panel
    public class SwitchPanelButton : MonoBehaviour
    {

        //GameCentalPr.PanelType parentPanelType;

        public GameUIPr.PanelType target;

        Button switchPanelButton;

        private void Start()
        {
            /* UIPanel uiPanel = GetComponentInParent<UIPanel>();
             if (uiPanel == null) { Debug.LogError("Parent Don't have an UIPanel"); }
             else { parentPanelType = uiPanel.GetPanelType(); }*/

            switchPanelButton = GetComponent<Button>();
            if (switchPanelButton != null)
            {
                switchPanelButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameUIPr.Instance.SwitchActivePanel(target);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
