using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class InventoryButton : MonoBehaviour
    {
        Button inventoryButton;

        private void Start()
        {
            inventoryButton = GetComponent<Button>();
            if (inventoryButton != null)
            {
                inventoryButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameUIPr.Instance.InventoryPanelEvent();
        }
    }
}
