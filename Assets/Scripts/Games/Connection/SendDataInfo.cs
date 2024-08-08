using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    public class SendDataInfo
    {
        public int GameID { get; set; }
        public int setting { get; set; }
        public int ResultAmount { get; set; }
        public int BonusCount { get; set; }
        public int IconCount { get; set; } // AL / ED /CT game
        public int EDLockReelStatus { get; set; } //ED, CH
    }
}
