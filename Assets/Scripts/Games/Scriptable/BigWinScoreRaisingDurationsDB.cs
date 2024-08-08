using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Big Win Score Raising Durations DB", menuName = "ScriptableObject/Big Win Score Raising Durations DB")]
    public class BigWinScoreRaisingDurationsDB : ScriptableObject
    {
        [SerializeField] BigWinScoreRaisingDurationInfo[] _bigWinScoreRaisingDurationInfos;

        public float GetDurationByType(BigWinType type)
        {
            foreach (BigWinScoreRaisingDurationInfo info in _bigWinScoreRaisingDurationInfos)
            {
                if (info.bigWinType == type)
                {
                    return info.duration;
                }
            }
            return 0f;
        }

        [Serializable]
        public class BigWinScoreRaisingDurationInfo
        {
            public BigWinType bigWinType;
            public float duration;
        }
    }
}

