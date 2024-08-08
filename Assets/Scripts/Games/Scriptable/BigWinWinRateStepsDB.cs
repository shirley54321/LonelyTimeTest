using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Big Win Win Rate Steps DB", menuName = "ScriptableObject/Big Win Win Rate Steps DB")]
    public class BigWinWinRateStepsDB : ScriptableObject
    {
        public BigWinWinRateStep[] bigWinWinRateSteps;
        public BigWinType GetTargetType(float rate)
        {
            BigWinType type = BigWinType.None;
            float targetThresholdRate = 0f;

            foreach(BigWinWinRateStep step in bigWinWinRateSteps)
            {
                if(step.thresholdRate <= rate && step.thresholdRate >= targetThresholdRate)
                {
                    targetThresholdRate = step.thresholdRate;
                    type = step.bigWinType;
                }
            }
            return type;
        }

        public BigWinType GetLowestType()
        {
            BigWinType type = BigWinType.None;

            float targetThresholdRate = Mathf.Infinity;
            foreach (BigWinWinRateStep step in bigWinWinRateSteps)
            {
                if (step.thresholdRate < targetThresholdRate)
                {
                    targetThresholdRate = step.thresholdRate;
                    type = step.bigWinType;
                }
            }
            return type;
        }

        [Serializable]
        public class BigWinWinRateStep
        {
            public BigWinType bigWinType;
            public float thresholdRate;
        }
    }
}
