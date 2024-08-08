using System;

namespace UserAccount.AccountAndPhone
{
    [Serializable]
    public class PhoneInfo
    {
        /// <summary>
        /// Indicates whether the user has linked their phone.
        /// </summary>
        public bool haveLinkPhone;

        public string phoneNumber;
    }
}