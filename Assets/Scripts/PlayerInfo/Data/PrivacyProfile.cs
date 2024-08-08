using System;

namespace Player
{
    /// <summary>
    /// Player Private Data
    /// </summary>
    [Serializable]
    public class PrivacyProfile
    {
        public string RealName;
        public DateTime Birthday;
        public string IdentityID;
        public string PhoneNumber;
        public string Address;
    }
}