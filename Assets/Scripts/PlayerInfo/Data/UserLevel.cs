using System;

namespace Player
{
    [Serializable]
    public class UserLevel
    {
        public int Level;
        public decimal Experience;
        public decimal NextLevelExperience;
        public DateTime UpgradeTime;
    }

    
}