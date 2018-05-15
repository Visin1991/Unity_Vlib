using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class SettingButton : MonoBehaviour
    {

        Button settingButton;

        void Start()
        {
            settingButton = GetComponent<Button>();
            if (settingButton != null)
            {
                settingButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameUIPr.Instance.SettingButtonOnClick();
        }
    }
}
