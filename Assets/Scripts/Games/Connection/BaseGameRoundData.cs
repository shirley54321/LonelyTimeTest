using UnityEngine;

namespace SlotTemplate {

    public class BaseGameRoundData : RoundData {


        public BaseGameRoundData (decimal remainedPlayerBalance, decimal bet, decimal betRate, RoundResultData resultData) : base(remainedPlayerBalance, bet, betRate, resultData) {
            this.bet = bet;
            this.betRate = betRate;
        }

        
    }

}
