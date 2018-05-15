using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class QuitButton : MonoBehaviour
    {

        Button quitButton;

        private void Start()
        {
            quitButton = GetComponent<Button>();
            if (quitButton != null)
            {
                quitButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameCentalPr.Instance.Quit();
        }
    }
}
