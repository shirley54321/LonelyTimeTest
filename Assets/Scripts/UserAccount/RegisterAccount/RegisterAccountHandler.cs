using System.Collections;
using System.Net.Http;
using System.Text;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UserAccount.Tool;
using PlayFabErrorCode = PlayFab.PlayFabErrorCode;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;

namespace UserAccount
{
    /// <summary>
    /// Handles the registration of a user account using PlayFab services.
    /// </summary>
    public class RegisterAccountHandler : MonoBehaviour
    {
        /// <summary>
        /// Event triggered on successful account registration.
        /// Subscribers can attach methods to this event.
        /// </summary>
        public static UnityEvent OnRegisterAccountSuccess = new UnityEvent();

        /// <summary>
        /// Event triggered on failed account registration.
        /// Subscribers receive a specific failure code (RegisterFailedCode) indicating the reason for the failure.
        /// Subscribers can attach methods to this event to handle different failure scenarios.
        /// </summary>
        public static UnityEvent<RegisterFailedCode> OnRegisterAccountFailed = new UnityEvent<RegisterFailedCode>();

     
        /// <summary>
        /// Coroutine for registering account.
        /// </summary>
        private Coroutine _registerAccountCoroutine;

        /// <summary>
        /// This class serves as the base class for creating new accounts and binding accounts,
        /// defining common functionality for both operations.
        /// </summary>        
        [SerializeField] private AccountRequesterBase accountRequester;
        
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="accountName">Username for the new account.</param>
        /// <param name="password">Password for the new account.</param>
        /// <param name="confirmPassword">Confirmation of the password.</param>
        public void RegisterAccount(string accountName, string password, string confirmPassword)
        {
            Debug.Log($"Creating account for {accountName}, {password}");

            _registerAccountCoroutine = StartCoroutine(
                RegisterAccountCoroutine(accountName, password, confirmPassword));

        }

        /// <summary>
        /// Coroutine for registering a new user account.
        /// </summary>
        /// <param name="accountName">Username for the new account.</param>
        /// <param name="password">Password for the new account.</param>
        /// <param name="confirmPassword">Confirmation of the password.</param>
        private IEnumerator RegisterAccountCoroutine(string accountName, string password, string confirmPassword)
        {
            // Open Loading Screen
            LoadingManger.Instance.Open_Loading_animator();
            
            // In order to wait for this coroutine, save it in _resetPasswordCoroutine variable
            yield return new WaitForNextFrameUnit();

            ValidateAccountAndPassword(accountName, password, confirmPassword);

            // Wait until a response is received
            yield return CheckRegisterAccountLimited();

            RegisterAccountWithPlayFab(accountName, password);
        }

        
        /// <summary>
        /// Validates the format of the account name and password.
        /// </summary>
        /// <param name="accountName">Username to validate.</param>
        /// <param name="password">Password to validate.</param>
        /// <param name="confirmPassword">Confirmation of the password.</param>
        /// <returns>True if validation succeeds, otherwise false.</returns>
        private void ValidateAccountAndPassword(string accountName, string password, string confirmPassword)
        {
            if (!AccountValidator.ValidateAccountAndPasswordFormat(accountName, password))
            {
                RegisterAccountFailed(RegisterFailedCode.AccountOrPasswordFormatWrong);
                return;
            }

            if (!AccountValidator.ValidatePasswordAndConfirmPassword(password, confirmPassword))
            {
                RegisterAccountFailed(RegisterFailedCode.PasswordUnequal);
                return;
            }
        }
        
        /// <summary>
        /// Checks if there are any limitations on registering a new account.
        /// </summary>
        private IEnumerator CheckRegisterAccountLimited()
        {
            bool isResponseReceived = false;
            string _CheckLimiteCode = "KfwS0IX4dvR_FGM2iuDHPBP49Q_r3m9Cx5WRl7dem4rgAzFu9ylwiw==";
            var functionUrl = "https://playerregister.azurewebsites.net/api/LimitDeviceCheck";
            
            var content = new StringContent(SystemInfo.deviceUniqueIdentifier, Encoding.UTF8, "application/json");
            // TODO: Implement CheckRegisterAccountLimited  logic
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-functions-key", _CheckLimiteCode);
                using (HttpResponseMessage response = client.PostAsync(functionUrl, content).Result)
                using (HttpContent respContent = response.Content)
                {
                    var sentString = respContent.ReadAsStringAsync().Result;

                    try
                    {
                        isResponseReceived = bool.Parse(sentString);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Device Limit Error: {ex.Message}");
                    }
                }
            }

            if (isResponseReceived) // Register Account Success
            {
                Debug.Log("Register Success!");
            }
            else
            {
                RegisterAccountFailed(RegisterFailedCode.RegisterCountReachLimited);
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        /// <summary>
        /// Registers a new user account with PlayFab.
        /// </summary>
        private void RegisterAccountWithPlayFab(string accountName, string password)
        {
            accountRequester.CreateUserAccountWithPlayFab(accountName, password);
        }

        #region Handle Register Successful or failed


        /// <summary>
        /// Handles successful account registration.
        /// </summary>
        public void HandlePlayFabRegisterSuccess(bool needCreateNickName)
        {
            StartCoroutine(HandlePlayFabRegisterSuccessCoroutine(needCreateNickName));
        }
        
        private IEnumerator HandlePlayFabRegisterSuccessCoroutine(bool needCreateNickName)
        {
            RecordRegisterAccountToDB();
            yield return ADDLoginDevice();

            if (needCreateNickName)
            {
                yield return AccountInitializer.AccountInitializer.CreateNickNameCoroutine();
            }
            
            OnRegisterAccountSuccess.Invoke();
            // Close Loading Screen
            LoadingManger.Instance.Close_Loading_animator();
        }



        private IEnumerator ADDLoginDevice()
        {
            bool isResponseReceived = false;
            
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType,
                },
                FunctionName = "LoginDevice",
                FunctionParameter = new Dictionary<string,object>() 
                {
                    {"machineId", SystemInfo.deviceUniqueIdentifier},
                    {"playerId", PlayFabSettings.staticPlayer.EntityId}
                },
                GeneratePlayStreamEvent = false 
            }, async(ExecuteFunctionResult result) =>
                {
                    isResponseReceived = true;
                Debug.Log($"Account Record Status: {result.FunctionResult.ToString()}");
            }, (PlayFabError error) =>
            {
                Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
            }

            );
            Debug.Log("ADDLoginDevice : OK");   
            
            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        private void RecordRegisterAccountToDB()
        {
            // TODO : RecordRegisterAccountToDB
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType,
                },
                FunctionName = "RecordRegisterRecord",
                FunctionParameter = new Dictionary<string,object>() 
                {
                    {"machineId", SystemInfo.deviceUniqueIdentifier}
                },
                GeneratePlayStreamEvent = false 
            }, async(ExecuteFunctionResult result) =>
            {
                Debug.Log($"Account Record Status: {result.FunctionResult.ToString()}");
            }, (PlayFabError error) =>
            {
                Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
            }

            );
            Debug.Log("RecordRegisterAccountToDB");
        }


        /// <summary>
        /// Handles failed account registration.
        /// </summary>
        /// <param name="errorCode">PlayFab error code.</param>
        public void HandlePlayFabRegisterFail(PlayFabErrorCode errorCode)
        {
            switch (errorCode)
            {
                case PlayFabErrorCode.UsernameNotAvailable:
                    RegisterAccountFailed(RegisterFailedCode.AccountHaveRegistered);
                    break;
                default:
                    RegisterAccountFailed(RegisterFailedCode.Other);
                    break;
            }
        }
        
        /// <summary>
        /// Invokes the event for a failed account registration.
        /// </summary>
        /// <param name="failedCode">Reason for the failed registration.</param>
        private void RegisterAccountFailed(RegisterFailedCode failedCode)
        {
            // Stop the coroutine handling the account registration
            StopCoroutine(_registerAccountCoroutine);
            Debug.Log($"<color=red>Register Account failed! {failedCode}</color>");

            // Invoke the OnLinkAccountFailed event with the specified failure code
            OnRegisterAccountFailed.Invoke(failedCode);
            
            // Close Loading Screen
            LoadingManger.Instance.Close_Loading_animator();
        }

        #endregion

        
    }
}
