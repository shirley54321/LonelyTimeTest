using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Game Info DB", menuName = "ScriptableObject/Game Info DB")]
    public class GameInfoDB : ScriptableObject
    {
        [SerializeField] int _reelsAmount = 5;
        public int reelsAmount => _reelsAmount;
        [SerializeField] int _showedIconsAmountPerReel = 3; 
        public int showedIconsAmountPerReel => _showedIconsAmountPerReel;

        public BonusOccurredType bonusOccurredType = BonusOccurredType.SitOnLine;

        // The index of the first reel which contains the icon
        [SerializeField] int[] _firstReelIndicesWhichContainsTheIcon;
        public int[] firstReelIndicesWhichContainsTheIcon => _firstReelIndicesWhichContainsTheIcon;

        [SerializeField] int _wildIconId = 0;
        public int wildIconId => _wildIconId;

        [SerializeField] int _bonusIconId = 1;
        public int bonusIconId => _bonusIconId;

        [SerializeField] int _requiredIconAmountForBonusOccurring = 3;
        public int requiredIconAmountForBonusOccurring => _requiredIconAmountForBonusOccurring;

        [SerializeField] bool[] _waitingForBonusReel;
        public bool[] waitingForBonusReel => _waitingForBonusReel;

        //¤½ª©¨S¦³swildIconId
        public SwildOccurredType swildOccurredType = SwildOccurredType.noSwild;

        [SerializeField] int _swildIconId = -1;
        public int swildIconId => _swildIconId;

        //enum 
        public enum BonusOccurredType
        {
            SitOnLine,
            AnyPlace
        }

        public enum SwildOccurredType
        {
            hasSwild,
            noSwild
        }
    }
}

