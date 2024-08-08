using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class BaseGameAutoplayingHandler : AutoplayHandler {


        // Message from GameStatesManager
        protected override void ReceiveRoundData (RoundData data) {
            base.ReceiveRoundData(data);
            if (data is BaseGameRoundData) {
                CurrentRoundData = data;
            }
        }


    }
}
