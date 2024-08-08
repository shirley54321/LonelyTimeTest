using System;

namespace Player
{
    [Serializable]
    public class VIP
    {
        public int level = 1;
        public UpgradeType type; 
        public DateTime UpgradeTime;
    }
    
    public enum UpgradeType
    {
        CreateAccount = 1,
        VerifyPhoneNumber = 2,
        FirstTimeTopUp = 3,
        Bet = 4,
        TopUp = 5
    }
}