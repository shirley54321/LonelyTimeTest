using System;

namespace Player
{
    [Serializable]
    public class ActivityEvent
    {
        public int GiftExchangeValue;
        public int TotalPlaytime;
        public int[] WeeklyLogin;
        public int LimitedTimeEvent;
        public int AccumulatedNonLogin;
        public int Reported;
    }

}