using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Majaja.Utilities;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Bonus Game Info DB", menuName = "ScriptableObject/Bonus Game Info DB")]
    public class BonusGameInfoDB : ScriptableObject
    {
        [SerializeField] int _maxBonusRounds = 25;
        public int maxBonusRounds => _maxBonusRounds;

        [SerializeField] int[] _bonusGameMultiplierNumbers = new int[] { 1 };
        public int[] bonusGameMultiplierNumbers => _bonusGameMultiplierNumbers;

        public int GetMultiplierNumberByBonusRoundNumber (int bonusRoundNumber, out bool isTheMaxNumber)
        {
            isTheMaxNumber = false;
            int index = MathTools.Clamp(bonusRoundNumber -1, 0, bonusGameMultiplierNumbers.Length-1);
            isTheMaxNumber = index == bonusGameMultiplierNumbers.Length-1;

            return bonusGameMultiplierNumbers[index];
        }
    }
}
