using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace V
{
    public class ClosePanel : MonoBehaviour
    {
        public Button button;
        // Use this for initialization
        void Start()
        {
            button = GetComponent<Button>().GetComponent<Button>();
            button.onClick.AddListener(MyFunc);
        }

        public void MyFunc()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}
  