using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class SliderObj : MonoBehaviour
    {

        public Slider slider;
        public Text text;
        public Slider_Text_Pos texPos = Slider_Text_Pos.Left;

        public System.Action<float> onValueChangeEvent_Handler;


        private void Start()
        {
            slider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        }

        public void OnValueChanged()
        {
            if (onValueChangeEvent_Handler != null)
            {
                onValueChangeEvent_Handler(slider.value);
            }
        }

        public void ReSetTitle(string newTitle)
        {
            text.text = newTitle;
        }
    }

    public class SliderValueChangeEventArgs : System.EventArgs
    {
        public float value;
        public SliderValueChangeEventArgs(float v)
        {
            value = v;
        }
    }

    public enum Slider_Text_Pos
    {
        Left,
        Top,
        Non
    }
}
