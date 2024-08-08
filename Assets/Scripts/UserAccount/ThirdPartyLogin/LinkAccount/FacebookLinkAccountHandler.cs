using Player;
using UserAccount.LinkAccount;

namespace UserAccount.ThirdPartyLogin.LinkAccount
{
    /// <summary>
    /// Handles the linking of user accounts with Facebook for third-party login.
    /// </summary>
    public class FacebookLinkAccountHandler
    {
        /// <summary>
        /// Initiates the process of linking the user account with Facebook.
        /// </summary>
        public void StartLinkAccount()
        {
            // TODO: Implement method
            
            OnLinkAccountSuccessful();
        }

        /// <summary>
        /// Handle successful linking of the Facebook account.
        /// </summary>
        public void OnLinkAccountSuccessful()
        {
            // Set Local Facebook Id
            string facebookId = "facebookId";
            PlayerInfoManager.Instance.SetLocalFacebookId(facebookId);
            
            // Unlink the customer ID from playFab
            UnlinkAccountHandler.UnlinkAccountWithDevice();

            // Invoke an event to notify successful account linking.
            LinkAccountEvent.InvokeLinkAccountSuccessful();
        }
    }
}