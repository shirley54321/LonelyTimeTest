using System;
using Games.Data;

namespace VIPSetting
{
    [Serializable]
    public class VipRight 
    {
        #region Json Parameter From PlayFab Title Data
        public string Name { get; set; }

        public string Description { get; set; }

        public int TopUp { get; set; }

        public int Bet { get; set; }

        public int Reclaim { get; set; }

        public int Gift { get; set; }

        public int Received { get; set; }

        public int Lottery { get; set; }

        public int Money { get; set; }

        public int List { get; set; }

        public Hall SlotMachine { get; set; }

        public Hall Card { get; set; }

        public Hall Fish { get; set; }

        public Hall West { get; set; }

        public Hall Happy { get; set; }

        public int ReserveMachineTime { get; set; }

        public int ReserveMachineCount { get; set; }

        public int Message { get; set; }

        public int period { get; set; }
        #endregion
    }
}

