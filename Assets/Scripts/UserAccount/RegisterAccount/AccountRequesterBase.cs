using System;
using UnityEngine;

namespace UserAccount
{
    /// <summary>
    /// This class serves as the base class for creating new accounts and binding accounts,
    /// defining common functionality for both operations.
    /// </summary>
    [RequireComponent(typeof(RegisterAccountHandler))]
    public abstract class AccountRequesterBase : MonoBehaviour
    {
        protected RegisterAccountHandler registerAccountHandler;
        
        private void Awake()
        {
            registerAccountHandler = GetComponent<RegisterAccountHandler>();
            Debug.Log($"SystemInfo.deviceUniqueIdentifier, {SystemInfo.deviceUniqueIdentifier}");
        }

        /// <summary>
        /// An abstract method to be implemented by derived classes.
        /// Creates a user account using PlayFab with the provided account name and password.
        /// </summary>
        /// <param name="accountName">The desired account name for the new user.</param>
        /// <param name="password">The password associated with the new user account.</param>
        public abstract void CreateUserAccountWithPlayFab(string accountName, string password);
    }
}