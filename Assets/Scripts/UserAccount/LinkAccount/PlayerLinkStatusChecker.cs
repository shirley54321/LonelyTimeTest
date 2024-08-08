using Player;
using UnityEngine;

namespace UserAccount.LinkAccount
{
    /// <summary>
    /// Helper class for checking the link status of various player account information.
    /// </summary>
    public class PlayerLinkStatusChecker
    {
        /// <summary>
        /// Checks if the player has linked any account (Facebook, Google, Apple, or Username).
        /// </summary>
        /// <returns>True if at least one external account is linked; otherwise, false.</returns>
        public bool HasLinkAnyAccount()
        {
            return HasLinkUserAccount() || HasLinkThirdPartyAccount();
        }

        /// <summary>
        /// Checks if the player has linked UserName Account.
        /// </summary>
        /// <returns>True if account is linked; otherwise, false.</returns>
        public bool HasLinkUserAccount()
        {
            var accountInfo = PlayerInfoManager.Instance.AccountInfo;

            if (accountInfo?.Username != null)
            {
                Debug.Log("Have Link User Account");
                return true;
            }
                

            return false;
        }

        /// <summary>
        /// Checks if the player has linked any third party account (Facebook, Google, Apple).
        /// </summary>
        /// <returns>True if at least one external account is linked; otherwise, false.</returns>
        public bool HasLinkThirdPartyAccount()
        {
            var accountInfo = PlayerInfoManager.Instance.AccountInfo;

            if (accountInfo?.FacebookInfo?.FacebookId != null)
            {
                Debug.Log("Have Link Facebook");
                return true;
            }
                

            if (accountInfo?.GoogleInfo?.GoogleId != null)
            {
                Debug.Log("Have Link Google");
                return true;
            }

            if (accountInfo?.AppleAccountInfo?.AppleSubjectId != null)
            {
                Debug.Log("Have Link Apple");
                return true;
            }

            Debug.Log("Have not link third party account");
            return false;
        }

        /// <summary>
        /// Checks if the player has linked their phone (VIP level 2 requirement).
        /// </summary>
        /// <returns>True if the player has linked their phone; otherwise, false.</returns>
        public bool HasLinkPhone()
        {
            return PlayerInfoManager.Instance.PhoneInfo.haveLinkPhone;
        }

        /// <summary>
        /// Checks if the player has made a top-up (VIP level 3 requirement).
        /// </summary>
        /// <returns>True if the player has made a top-up; otherwise, false.</returns>
        public bool HasTopUp()
        {
            return PlayerInfoManager.Instance.PlayerInfo.vip.level >= 3;
        }
    }
}