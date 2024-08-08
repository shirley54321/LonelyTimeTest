using System;
using UnityEngine;

namespace Main.Setting
{
    /// <summary>
    /// Game Setting Data
    /// </summary>
    [Serializable]
    public class GameSetting
    {
        public float MusicVolume;
        public float SoundVolume;

        public bool Notify;
        public bool BigRewardShare;
        public bool BigRewardBroadcast;
        public bool LevelUpBroadcast;


        public override string ToString()
        {
            return $"{nameof(MusicVolume)}: {MusicVolume}, {nameof(SoundVolume)}: {SoundVolume}\n" +
                   $"{nameof(Notify)}: {Notify}, {nameof(BigRewardShare)}: {BigRewardShare}, {nameof(BigRewardBroadcast)}: {BigRewardBroadcast}, {nameof(LevelUpBroadcast)}: {LevelUpBroadcast}";
        }
    }
}