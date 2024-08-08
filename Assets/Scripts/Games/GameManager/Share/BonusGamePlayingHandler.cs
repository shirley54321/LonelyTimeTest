using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class BonusGamePlayingHandler : AutoplayHandler {

        public static AutoPlayOptions GetAutoPlayOptions (int gainedRounds) {
            AutoPlayOptions options = AutoPlayOptions.defaultValue;
            options.targetPlayRounds = gainedRounds;
            return options;
        }


        // Message from GameStatesManager
        protected override void ReceiveRoundData (RoundData data) {
            base.ReceiveRoundData(data);
            if (data is BonusGameRoundData) {
                CurrentRoundData = data;
            }
        }



        public void AddTargetTotalRounds (int gainedRounds) {
             AutoPlayOptions options = CurrentAutoPlayOptions;
             options.targetPlayRounds += gainedRounds;
             CurrentAutoPlayOptions = options;
        }



    }
}
