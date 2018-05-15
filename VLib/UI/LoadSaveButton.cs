using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class LoadSaveButton : MonoBehaviour
    {

        public int loadIndex;
        Button loadSaveButton;
        // Use this for initialization
        void Start()
        {
            loadSaveButton = GetComponent<Button>();
            if (loadSaveButton != null)
            {
                loadSaveButton.onClick.AddListener(delegate { OnClick(); });
            }
        }

        public void OnClick()
        {
            GameCentalPr.Instance.LoadSave_by_Index(loadIndex);
        }
    }
}
