using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class SaveGameButton : MonoBehaviour
    {

        public int loadIndex;
        Button saveGameButton;
        // Use this for initialization
        void Start()
        {
            saveGameButton = GetComponent<Button>();
            if (saveGameButton != null)
            {
                saveGameButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameCentalPr.Instance.SaveGame(loadIndex);
        }
    }
}
