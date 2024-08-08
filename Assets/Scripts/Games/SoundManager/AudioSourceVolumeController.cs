using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlotTemplate {

    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class AudioSourceVolumeController : MonoBehaviour {

        AudioSource _audioSource;
        public AudioSource audioSource {
            get {
                if (_audioSource == null) {
                    _audioSource = GetComponent<AudioSource>();
                }
                return _audioSource;
            }
        }
        private void OnEnable()
        {
            if(gameObject.name=="Music") {
                GameSettings.instance.MusicSwitch += HandleMusicSwitch;
            }
            else{
                GameSettings.instance.SoundSwitch += HandleSoundSwitch;
            }
        }
        private void OnDisable()
        {
            if(gameObject.name=="Music") {
                GameSettings.instance.MusicSwitch -= HandleMusicSwitch;
            }
            else{
                GameSettings.instance.SoundSwitch -= HandleSoundSwitch;
            }
        }
        private void HandleMusicSwitch()
        {
            ResetVolume();
        }
        private void HandleSoundSwitch()
        {
            ResetVolume();
        }
        public void ResetVolume () {
            StopAllCoroutines();
            if(gameObject.name=="Music"){
                audioSource.volume =GameSettings.instance.Music ? 1f : 0f;
            }
            else{
                audioSource.volume =GameSettings.instance.Sound ? 1f : 0f;
            }
            
        }

        public void Mute () {
            StopAllCoroutines();
            audioSource.volume = 0f;
        }

        public void FadeVolume (float startVolume, float endVolume, float duration, Action endCallback = null) {
            StopAllCoroutines();
            StartCoroutine(FadeControlling(startVolume, endVolume, duration, endCallback));
        }


        IEnumerator FadeControlling (float startVolume, float endVolume, float duration, Action endCallback = null) {
            float startTime = Time.time;

            float elapsedTimeRate = 0f;
            while (elapsedTimeRate < 1f) {

                if (duration > 0) {
                    elapsedTimeRate = (Time.time - startTime) / duration;
                }
                else {
                    elapsedTimeRate = 1f;
                }

                //audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTimeRate);
                if(gameObject.name=="Music") {
                    audioSource.volume =GameSettings.instance.Music ? 1f : 0f;
                }
                else{
                    audioSource.volume =GameSettings.instance.Sound ? 1f : 0f;
                }

                yield return null;
            }

            if (endCallback != null) {
                endCallback();
            }
        }

    }
}
