using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class AudioManager : Singleton<AudioManager>
    {
        protected AudioManager() { }

        public enum AudioChannel { Master, Sfx, Music }

        [Range(0.0f, 1.0f)]
        public float masterVolumePercent = .5f;
        [Range(0.0f, 1.0f)]
        public float sfxVolumePercent = 1f;
        [Range(0.0f, 1.0f)]
        public float musicVolumePercent = .5f;

        AudioSource sfx2DSource;
        AudioSource[] musicSources;
        int activeMusicSourceIndex;

        Transform audioListener;
        Transform targetT;

        SoundLib library;


        public SliderObj masterSlider;
        public SliderObj sfxSlider;
        public SliderObj musicSlider;

        void Awake()
        {
            library = GetComponent<SoundLib>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            //playerT = FindObjectOfType<Player>().transform;

            masterVolumePercent = PlayerPrefs.GetFloat("master Volume", masterVolumePercent);
            musicVolumePercent = PlayerPrefs.GetFloat("music Volume", masterVolumePercent);
            sfxVolumePercent = PlayerPrefs.GetFloat("Sfx Volume", sfxVolumePercent);

        }

        private void Start()
        {
            if (masterSlider != null)
            {
                masterSlider.onValueChangeEvent_Handler -= SetMaster;
                masterSlider.onValueChangeEvent_Handler += SetMaster;
            }

            if (sfxSlider != null)
            {
                sfxSlider.onValueChangeEvent_Handler -= SetSFX;
                sfxSlider.onValueChangeEvent_Handler += SetSFX;
            }

            if (sfxSlider != null)
            {
                musicSlider.onValueChangeEvent_Handler -= SetMusic;
                musicSlider.onValueChangeEvent_Handler += SetMusic;
            }
        }

        void Update()
        {
            if (targetT != null)
            {
                audioListener.position = targetT.position;
            }
        }

        public void SetTarget(Transform tf)
        {
            targetT = tf;
        }

        public void SetMaster(float v)
        {
            SetVolume(v, AudioChannel.Master);
        }

        public void SetSFX(float v)
        {
            SetVolume(v, AudioChannel.Sfx);
        }

        public void SetMusic(float v)
        {
            SetVolume(v, AudioChannel.Music);
        }

        public void SetVolume(float volumePercent, AudioChannel channel)
        {
            //Debug.LogFormat("Set Volume {0} : {1}", channel, volumePercent);
            switch (channel)
            {
                case AudioChannel.Master:
                    masterVolumePercent = volumePercent;
                    break;
                case AudioChannel.Sfx:
                    sfxVolumePercent = volumePercent;
                    break;
                case AudioChannel.Music:
                    musicVolumePercent = volumePercent;
                    break;
            }
            musicSources[0].volume = musicVolumePercent * masterVolumePercent;
            musicSources[1].volume = musicVolumePercent * masterVolumePercent;

            PlayerPrefs.SetFloat("master Volume", masterVolumePercent);
            PlayerPrefs.SetFloat("music Volume", musicVolumePercent);
            PlayerPrefs.SetFloat("Sfx Volume", sfxVolumePercent);
        }

        public void PlayMusic(AudioClip clip, float fadeDuration = 1)
        {
            activeMusicSourceIndex = 1 - activeMusicSourceIndex;
            musicSources[activeMusicSourceIndex].clip = clip;
            musicSources[activeMusicSourceIndex].Play();
            StartCoroutine(AnimateMusicCrossfade(fadeDuration));
        }

        public void PlaySound(AudioClip clip, Vector3 pos)
        {
            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }

        public void PlaySoundRandom(string soundName, Vector3 pos)
        {
            PlaySound(library.GetClipFromName_Random(soundName), pos);
        }

        public void PlaySound(string soundName, int index, Vector3 pos)
        {
            PlaySound(library.GetClipFromName(soundName, index), pos);
        }

        public void PlaySound2DRandom(string soundName)
        {
            sfx2DSource.PlayOneShot(library.GetClipFromName_Random(soundName), sfxVolumePercent * masterVolumePercent);
        }

        public void PlaySound2D(string soundName, int index)
        {
            sfx2DSource.PlayOneShot(library.GetClipFromName(soundName, index), sfxVolumePercent * masterVolumePercent);
        }

        IEnumerator AnimateMusicCrossfade(float duration)
        {
            float percent = 0;
            while (percent < 1)
            {
                percent += Time.deltaTime * 1 / duration;
                musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
                musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
                yield return null;
            }
        }
    }
}
