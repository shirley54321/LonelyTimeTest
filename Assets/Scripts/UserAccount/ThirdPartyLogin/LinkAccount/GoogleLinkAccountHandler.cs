using Player;
using UserAccount.LinkAccount;

namespace UserAccount.ThirdPartyLogin.LinkAccount
{
    /// <summary>
    /// Handles the linking of user accounts with Google for third-party login.
    /// </summary>
    public class GoogleLinkAccountHandler
    {
        /// <summary>
        /// Initiates the process of linking the user account with Google.
        /// </summary>
        public void StartLinkAccount()
        {
            // TODO: Implement method
            
            OnLinkAccountSuccessful();
        }

        /// <summary>
        /// Handles successful linking of the Google account.
        /// </summary>
        public void OnLinkAccountSuccessful()
        {
            // Set Local Google Id
            string googleId = "googleId";
            PlayerInfoManager.Instance.SetLocalGoogleId(googleId);
            
            // Unlink the customer ID from PlayFab
            UnlinkAccountHandler.UnlinkAccountWithDevice();

            // Invoke an event to notify successful account linking.
            LinkAccountEvent.InvokeLinkAccountSuccessful();
        }
    }
}