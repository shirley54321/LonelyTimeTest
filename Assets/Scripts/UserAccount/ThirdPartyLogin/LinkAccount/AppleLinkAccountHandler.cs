using Player;
using UserAccount.LinkAccount;

namespace UserAccount.ThirdPartyLogin.LinkAccount
{
    /// <summary>
    /// Handles the linking of user accounts with Apple for third-party login.
    /// </summary>
    public class AppleLinkAccountHandler
    {
        /// <summary>
        /// Initiates the process of linking the user account with Apple.
        /// </summary>
        public void StartLinkAccount()
        {
            // TODO: Implement method
            
            OnLinkAccountSuccessful();
        }

        /// <summary>
        /// Handles successful linking of the Apple account.
        /// </summary>
        public void OnLinkAccountSuccessful()
        {
            // Set Local Apple Id
            string appleId = "appleId";
            PlayerInfoManager.Instance.SetLocalAppleId(appleId);
            
            // Unlink the customer ID from PlayFab
            UnlinkAccountHandler.UnlinkAccountWithDevice();

            // Invoke an event to notify successful account linking.
            LinkAccountEvent.InvokeLinkAccountSuccessful();
        }
    }
}