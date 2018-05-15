using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class SfxVolumSlider : MonoBehaviour
    {

        Slider sfxSlider;

        private void Start()
        {
            sfxSlider = GetComponent<Slider>();
            if (sfxSlider != null)
            {
                sfxSlider.onValueChanged.AddListener(delegate { ValueChange(); });
            }
            sfxSlider.value = AudioManager.Instance.sfxVolumePercent;
        }

        public void ValueChange()
        {
            AudioManager.Instance.SetVolume(sfxSlider.value, AudioManager.AudioChannel.Sfx);
        }
    }
}
