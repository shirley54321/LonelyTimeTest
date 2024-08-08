using System;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UserAccount
{
    /// <summary>
    /// Handles the quick login process for the user.
    /// </summary>
    public class QuickLoginHandler : MonoBehaviour
    {
        private readonly QuickLoginIdHandler quickLoginIdHandler = new QuickLoginIdHandler();

        [SerializeField] private bool quickLoginOnStart;

        private void Start()
        {
            if (quickLoginOnStart)
                StartQuickLogin();
        }

        /// <summary>
        /// Initiates the quick login process.
        /// </summary>
        public void StartQuickLogin()
        {
            if (!UndermaintenanceNotification.isMaintenant)
            {
                if (quickLoginIdHandler.CheckDeviceHaveQuickLoginId())
                {
                    CheckIsDeviceHasAccountRecord();
                }
                else
                {
                    OnDeviceNotContainId();
                }
            }
            else
            {
                Debug.Log("System is in maintenance");
            }
        }

        /// <summary>
        /// Uses the Quick Login ID to check whether the device has an account record in PlayFab.
        /// </summary>
        private void CheckIsDeviceHasAccountRecord()
        {
            var request = new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CustomId = quickLoginIdHandler.QuickLoginID,
                CreateAccount = false,
            };

            PlayFabClientAPI.LoginWithCustomID(request,
                // Success
                (LoginResult result) =>
                {
                    Debug.Log("Device contains quickLoginId, entering login process");
                    UserAccountManager.Instance.HandlePlayFabLoginSuccess(result);
                },
                // Failure
                (PlayFabError error) =>
                {
                    // If the account is banned, it means there is an account record but login failed (due to a ban).
                    if (error.Error == PlayFabErrorCode.AccountBanned)
                    {
                        // TODO handle ban
                    }
                    else
                    {
                        OnDeviceNotContainId();
                    }
                });
        }

        /// <summary>
        /// Handles the case when the device does not contain the Quick Login ID.
        /// </summary>
        private void OnDeviceNotContainId()
        {
            // TODO: Use Scene Loader
            Debug.Log("Device doesn't contain quickLoginId, entering login scene");
            SceneManager.LoadScene("Login");
        }

        /// <summary>
        /// Clears the Quick Login ID.
        /// </summary>
        [ContextMenu("Clear QuickLoginId")]
        public void ClearQuickLoginId()
        {
            quickLoginIdHandler.ClearGuID();
        }
    }
}
