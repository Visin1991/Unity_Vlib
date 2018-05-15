using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{

    public class MasterVolumeSlider : MonoBehaviour
    {

        Slider masterSlider;

        private void Start()
        {
            masterSlider = GetComponent<Slider>();
            if (masterSlider != null)
            {
                masterSlider.onValueChanged.AddListener(delegate { ValueChange(); });
            }
            masterSlider.value = AudioManager.Instance.masterVolumePercent;
        }

        public void ValueChange()
        {
            AudioManager.Instance.SetVolume(masterSlider.value, AudioManager.AudioChannel.Master);
        }
    }
}
