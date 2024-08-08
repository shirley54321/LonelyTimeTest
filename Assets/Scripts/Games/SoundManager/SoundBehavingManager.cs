using System;
using System.Collections;
using UnityEngine;

namespace SlotTemplate {

    public class SoundBehavingManager : MonoBehaviour {
        [SerializeField] SoundEventInfo[] _soundEventInfos;


        public void TriggerEvent(Event triggeredEvent, EventType eventType = EventType.Starting, int clipId = -1) {

            SoundEventInfo triggeredEventInfo = SoundEventInfo.FindByEvent(_soundEventInfos, triggeredEvent);

            if (eventType == EventType.Starting) {
                triggeredEventInfo?.PlayAudio(clipId);
            }
            else if (eventType == EventType.Stopping) {
                triggeredEventInfo?.EndAudio();
            }

        }


        [Serializable]
        public class SoundEventInfo {
            public Event soundEvent = Event.None;
            public AudioSource targetAudioSource = null;
            public AudioClipsDB clipsDB;

            public bool isOneShot = false;

            [Tooltip("Unavailable if one shot is enabled.")]
            public bool loop = false;

            [Tooltip("Unavailable if one shot is enabled.")]
            public FadeInfo fadeInInfo;
            [Tooltip("Unavailable if one shot is enabled.")]
            public FadeInfo fadeOutInfo;


            public static SoundEventInfo FindByEvent (SoundEventInfo[] infos, Event soundEvent) {
                foreach (SoundEventInfo info in infos) {
                    if (info.soundEvent == soundEvent) {
                        return info;
                    }
                }
                return null;
            }

            public void PlayAudio (int clipId) {
                if (targetAudioSource != null && clipsDB != null) {
                    AudioClip clip;

                    if (clipId == -1) {
                        clip = clipsDB.GetRandomAudioClip();
                    }
                    else {
                        clip = clipsDB.GetAudioClipById(clipId);
                    }

                    if (clip != null) {
                        AudioSourceVolumeController audioSourceVolumeController = targetAudioSource.GetComponent<AudioSourceVolumeController>();

                        if (audioSourceVolumeController != null) {
                            audioSourceVolumeController.ResetVolume();
                        }

                        if (isOneShot) {
                            targetAudioSource.PlayOneShot(clip);
                        }
                        else {
                            if (fadeInInfo.isFade) {
                                if (audioSourceVolumeController != null) {
                                    fadeInInfo.ApplyFading(audioSourceVolumeController);
                                }
                            }
                            targetAudioSource.clip = clip;
                            targetAudioSource.loop = loop;
                            targetAudioSource.Play();
                        }
                    }
                }
            }

            public void EndAudio () {
                if (targetAudioSource.isPlaying) {
                    if (fadeOutInfo.isFade) {
                        AudioSourceVolumeController audioSourceVolumeController = targetAudioSource.GetComponent<AudioSourceVolumeController>();
                        if (audioSourceVolumeController != null) {
                            fadeOutInfo.ApplyFading(audioSourceVolumeController, StopAudio);
                        }
                    }
                    else {
                        StopAudio();
                    }
                }
            }

            void StopAudio () {
                targetAudioSource.Stop();
            }


            [Serializable]
            public class FadeInfo {

                public static FadeInfo defaultFadeInInfo = new FadeInfo {
                    isFade = false,
                    startVolume = 0f,
                    endVolume = 1f,
                    duration = 1f
                };

                public static FadeInfo defaultFadeOutInfo = new FadeInfo {
                    isFade = false,
                    startVolume =1f,
                    endVolume = 0f,
                    duration = 1f
                };

                public bool isFade = false;
                public float startVolume = 0f;
                public float endVolume = 1f;
                public float duration = 0f;


                public void ApplyFading (AudioSourceVolumeController audioSourceVolumeController, Action endCallback = null) {
                    audioSourceVolumeController.FadeVolume(startVolume, endVolume, duration, endCallback);
                }

            }
        }



        public struct ReelStoppingEventParameters {
            public int reelIndex;
            public bool isWaitingForBonusIconEffectOn;
        }


        public enum Event {
            None,
            Rolling, // music event
            ReelStopping, // sfx event
            ReelStoppingWithBonusIcon, // sfx event
            ScoreRaising, // music event
            ScoreRaisingEnded, // music event
            SpecialIconAnimatinginTotalWinAnimation, // sfx event
            BonusIconAnimatingInTotalWinAnimation, // sfx event
            WaitingForBonusIconEffect, // sfx event
            BigWinAnimating, // music event
            BonusGameTriggered, // sfx event
            BonusGamePlaying, // music event
            BonusGameMultiplierAnimating, // sfx event
            EnteringBonusGameStart, //sfx event
            EnteringBonusGameEnd, //sfx event
            BonusGameTotalWinsDisplay
        }

        public enum EventType {
            Starting,
            Stopping
        }

    }
}
