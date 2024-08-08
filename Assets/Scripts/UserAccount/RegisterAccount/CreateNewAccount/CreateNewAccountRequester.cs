using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UserAccount
{
    /// <summary>
    /// Class responsible for handling the creation of a new user account using PlayFab.
    /// Inherits from the AccountRequesterBase class.
    /// </summary>
    public class CreateNewAccountRequester : AccountRequesterBase
    {
        /// <summary>
        /// Implementation of the abstract method from the base class.
        /// Creates a new PlayFab user account with the specified account name and password.
        /// </summary>
        /// <param name="accountName">The desired account name for the new user.</param>
        /// <param name="password">The password associated with the new user account.</param>
        public override void CreateUserAccountWithPlayFab(string accountName, string password)
        {
            // Using PlayFabClientAPI to register a new PlayFab user.
            PlayFabClientAPI.RegisterPlayFabUser(
                new RegisterPlayFabUserRequest()
                {
                    Password = password,
                    Username = accountName,
                    Email = $"{accountName}@thisisfakeemail.justforchange.com",
                    RequireBothUsernameAndEmail = false
                },
                result =>
                {
                    // Log a successful account creation.
                    Debug.Log($"<color=green>Successful Account Creation</color>: {accountName}");
                    registerAccountHandler.HandlePlayFabRegisterSuccess(true);
                },
                error =>
                {
                    // Log an unsuccessful account creation with error details.
                    Debug.Log($"<color=red>Unsuccessful Account Creation</color>: {accountName} \n " +
                              $"{error.Error}\n" +
                              $"{error.ErrorMessage}");
                    registerAccountHandler.HandlePlayFabRegisterFail(error.Error);
                }
            );
        }
    }
}