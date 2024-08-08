using System;
using System.Collections;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace UserAccount
{
    /// <summary>
    /// Handles the login functionality for user accounts using PlayFab services.
    /// </summary>
    [RequireComponent(typeof(AccountLoginErrorCount))]
    public class AccountLoginHandler : MonoBehaviour
    {
        private string _accountName, _password;
        
        
        private AccountLoginErrorCount _errorCountHandler;

        private void Awake()
        {
            _errorCountHandler = GetComponent<AccountLoginErrorCount>();
        }

        /// <summary>
        /// Coroutine for the password reset process.
        /// </summary>
        private Coroutine _loginCoroutine;
        
        /// <summary>
        /// Initiates the login process with the provided account name and password.
        /// </summary>
        /// <param name="accountName">Username for the login.</param>
        /// <param name="password">Password for the login.</param>
        public void StartLogin(string accountName, string password)
        {
            _accountName = accountName;
            _password = password;

            _loginCoroutine = StartCoroutine(LoginCoroutine());
        }


        /// <summary>
        /// Coroutine for handling the login process asynchronously.
        /// </summary>
        private IEnumerator LoginCoroutine()
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            // Wait until a response is received
            yield return CheckLoginFailLimitCount();

            LoginWithPlayFabAccount();
        }

        /// <summary>
        /// Checks the login fail limit count.
        /// </summary>
        private IEnumerator CheckLoginFailLimitCount()
        {
            bool isResponseReceived = false;
            yield return _errorCountHandler.CheckTimes();

            if (_errorCountHandler.isOverCount)
            {
                LoginFailed(LoginFailedCode.IncorrectPasswordTimesExceedLimited);
            }
            else
            {
                isResponseReceived = true;
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        /// <summary>
        /// Initiates the login with the provided account credentials.
        /// </summary>
        private void LoginWithPlayFabAccount()
        {
            PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
                {
                    InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
                    {
                        GetUserAccountInfo = true
                    },
                    Username = _accountName,
                    Password = _password
                },
                // Success
                result =>
                {
                    Debug.Log($"Login with Account: {_accountName} <color=green>success(in PlayFab)</color>");
                    UserAccountManager.Instance.HandlePlayFabLoginSuccess(result, isAccountLogin:true);
                },
                // Failure
                error =>
                {
                    if(!(error.Error == PlayFabErrorCode.APIClientRequestRateLimitExceeded))
                        _errorCountHandler.PlusCount();
                    UserAccountManager.Instance.HandlePlayFabLoginFailed(error);
                }
            );
        }

        /// <summary>
        /// Handles the failure of user login, delegates the handling to the UserAccountManager,
        /// and stops the coroutine responsible for the login process.
        /// </summary>
        /// <param name="failedCode">Reason for the failed login.</param>
        private void LoginFailed(LoginFailedCode failedCode)
        {
            // Delegate the handling of login failure to the UserAccountManager
            UserAccountManager.Instance.LoginFailed(failedCode);

            // Stop the coroutine handling the login process
            StopCoroutine(_loginCoroutine);
        }

    }
}
