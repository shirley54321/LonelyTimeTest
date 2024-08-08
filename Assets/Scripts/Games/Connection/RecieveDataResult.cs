using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SlotTemplate
{
    public class RecieveDataResult 
    {
        public byte[] Result { get; set; }
        public decimal Win { get; set; }
        public double TotalWin { get; set; }
        public int RecordId { get; set; }
        public int BonusRecordId { get; set; }
        public int BonusCount { get; set; }
        public int SpecialTime { get; set; }
        public int IconCount { get; set; } // AL / ED /CT game
        public int EDLockReelStatus { get; set; }
    }
}
