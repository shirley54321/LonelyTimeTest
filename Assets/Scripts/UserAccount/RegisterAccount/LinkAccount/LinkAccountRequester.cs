using Player;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

namespace UserAccount.LinkAccount
{
    /// <summary>
    /// Class responsible for handling the linking of an existing account to PlayFab.
    /// Inherits from the AccountRequesterBase class.
    /// </summary>
    public class LinkAccountRequester : AccountRequesterBase
    {
        
        
        /// <summary>
        /// Implementation of the abstract method from the base class.
        /// Links an existing PlayFab user account with the specified account name and password.
        /// </summary>
        /// <param name="accountName">The existing account name to be linked.</param>
        /// <param name="password">The password associated with the existing user account.</param>
        public override void CreateUserAccountWithPlayFab(string accountName, string password)
        {
            // Using PlayFabClientAPI to register a new PlayFab user.
            PlayFabClientAPI.AddUsernamePassword(
                new AddUsernamePasswordRequest()
                {
                    Password = password,
                    Username = accountName,
                    Email = $"{accountName}@thisisfakeemail.justforchange.com",
                },
                result =>
                {
                    // Log a successful account creation.
                    Debug.Log($"<color=green>Successful Account Linked</color>: {accountName}");
                    registerAccountHandler.HandlePlayFabRegisterSuccess(false);
                    
                    PlayerInfoManager.Instance.SetLocalUserName(accountName);
                    LinkAccountEvent.InvokeLinkAccountSuccessful();
                    
                    // Unlink the customer ID after successful account linking.
                    UnlinkAccountHandler.UnlinkAccountWithDevice();
                },
                error =>
                {
                    // Log an unsuccessful account creation with error details.
                    Debug.Log($"<color=red>Unsuccessful Account Linked</color>: {accountName} \n " +
                              $"{error.Error}\n" +
                              $"{error.ErrorMessage}");
                    registerAccountHandler.HandlePlayFabRegisterFail(error.Error);
                }
            );
        }
    }
}