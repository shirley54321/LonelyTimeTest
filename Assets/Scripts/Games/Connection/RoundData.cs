using UnityEngine;

namespace SlotTemplate {

    public class RoundData {

        public decimal remainBalance;
        public decimal bet;
        public decimal betRate;
        public RoundResultData resultData;

        public RoundData (decimal remainBalance, decimal bet, decimal betRate, RoundResultData resultData) {
            this.remainBalance = remainBalance;
            this.bet = bet;
            this.betRate = betRate;
            this.resultData = resultData;
        }

        public float WonRate {
            get {
                if (resultData != null && bet != 0) {
                    return (float) (resultData.totalWin / bet);
                }
                return 0f;
            }
        }
        
    }
}
