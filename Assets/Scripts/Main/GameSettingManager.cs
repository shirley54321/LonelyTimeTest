using System;
using UnityEngine;

namespace Main.Setting
{
    /// <summary>
    /// Game Setting Manager
    /// Need to load setting when game start
    /// </summary>
    public static class GameSettingManager
    {
        /// <summary>
        /// Setting Key for PlayerPrefs key
        /// </summary>
        private enum SettingKey
        {
            MusicVolume,
            SoundVolume,
            Notify,
            BigRewardShare,
            BigRewardBroadcast,
            LevelUpBroadcast
        }

        public static GameSetting Setting;

        // 委託和事件
        public delegate void MusicVolumeChangedEventHandler(float newVolume);
        public static event MusicVolumeChangedEventHandler OnMusicVolumeChanged;
        public delegate void SoundVolumeChangedEventHandler(float newVolume);
        public static event SoundVolumeChangedEventHandler OnSoundVolumeChanged;        

        #region Save and Load Setting Data
        public static void Load()
        {
            Setting = new GameSetting
            {
                MusicVolume = PlayerPrefs.GetFloat(SettingKey.MusicVolume.ToString(), 0.5f),
                SoundVolume = PlayerPrefs.GetFloat(SettingKey.SoundVolume.ToString(), 0.5f),
                Notify = Convert.ToBoolean(PlayerPrefs.GetInt(SettingKey.Notify.ToString(), 1)),
                BigRewardShare = Convert.ToBoolean(PlayerPrefs.GetInt(SettingKey.BigRewardShare.ToString(), 1)),
                BigRewardBroadcast = Convert.ToBoolean(PlayerPrefs.GetInt(SettingKey.BigRewardBroadcast.ToString(), 1)),
                LevelUpBroadcast = Convert.ToBoolean(PlayerPrefs.GetInt(SettingKey.LevelUpBroadcast.ToString(), 1))
            };
            Debug.Log($"Load Setting {Setting}");    
        }

        public static void Save()
        {
            PlayerPrefs.SetFloat(SettingKey.MusicVolume.ToString(), Setting.MusicVolume);
            PlayerPrefs.SetFloat(SettingKey.SoundVolume.ToString(), Setting.SoundVolume);

            PlayerPrefs.SetInt(SettingKey.Notify.ToString(), Convert.ToInt32(Setting.Notify));
            PlayerPrefs.SetInt(SettingKey.BigRewardShare.ToString(), Convert.ToInt32(Setting.BigRewardShare));
            PlayerPrefs.SetInt(SettingKey.BigRewardBroadcast.ToString(), Convert.ToInt32(Setting.BigRewardBroadcast));
            PlayerPrefs.SetInt(SettingKey.LevelUpBroadcast.ToString(), Convert.ToInt32(Setting.LevelUpBroadcast));

            PlayerPrefs.Save();
            Debug.Log($"Save Setting {Setting}");
            // 觸發 MusicVolume 變更事件
            OnMusicVolumeChanged?.Invoke(Setting.MusicVolume);

            // 觸發 SoundVolume 變更事件
            OnSoundVolumeChanged?.Invoke(Setting.SoundVolume);        
        }

        #endregion
    }
}