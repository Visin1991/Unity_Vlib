using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class MusicVolumeSlider : MonoBehaviour
    {

        Slider musicSlider;

        private void Start()
        {
            musicSlider = GetComponent<Slider>();
            if (musicSlider != null)
            {
                musicSlider.onValueChanged.AddListener(delegate { ValueChange(); });
            }
            musicSlider.value = AudioManager.Instance.musicVolumePercent;
        }

        public void ValueChange()
        {
            AudioManager.Instance.SetVolume(musicSlider.value, AudioManager.AudioChannel.Music);
        }
    }
}
