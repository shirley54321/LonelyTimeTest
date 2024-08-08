using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate {

    public class WaitingForBonusIconEffectController : MonoBehaviour {


        public void StopThenKill () {
            Destroy(gameObject);
        }

    }
}
