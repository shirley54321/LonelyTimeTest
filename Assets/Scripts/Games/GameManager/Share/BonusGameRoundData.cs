using UnityEngine;

namespace SlotTemplate {

    public class BonusGameRoundData : RoundData {

        public decimal previousAccumulatedWin = 0;
        public int wonBonusLinesCountWhenBonusGameTriggered = 0;

        public BonusGameRoundData (decimal remainedPlayerBalance, decimal bet, decimal betRate, RoundResultData resultData, decimal previousAccumulatedWin, int wonBonusLinesCountWhenBonusGameTriggered) : base(remainedPlayerBalance, bet, betRate, resultData) {
            this.previousAccumulatedWin = previousAccumulatedWin;
            this.wonBonusLinesCountWhenBonusGameTriggered = wonBonusLinesCountWhenBonusGameTriggered;
        }

        public static float GetTotalWonRate (BonusGameRoundData[] bonusGameRoundsData) {
            decimal totalbonusWin = 0;
            foreach (BonusGameRoundData roundData in bonusGameRoundsData) {
                totalbonusWin += roundData.resultData.totalWin;
            }
            return (float) (totalbonusWin / bonusGameRoundsData[0].bet);
        }

    }

}
