using Games.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    [CreateAssetMenu(fileName = "Bet Rate DB", menuName = "ScriptableObject/Bet Rate DB")]
    public class BetRateDB : ScriptableObject
    {
        [SerializeField] BetRate[] betRates;

        public int GetHallById(int hallId, int step)
        {
            foreach(BetRate betRate in betRates) 
            {
                if((int)betRate.hall == hallId)
                {
                    return betRate.betRate[step];
                }
            }
            return 0;
        }

        public int GetBetRateLength(int hallId)
        {
            foreach (BetRate betRate in betRates)
            {
                if ((int)betRate.hall == hallId)
                {
                    return betRate.betRate.Length;
                }
            }
            return 0;
        }
    }

    [System.Serializable]
    public class BetRate
    {
        public Hall hall;
        public int[] betRate;
    }
}
