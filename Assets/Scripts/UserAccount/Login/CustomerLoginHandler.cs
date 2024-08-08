using System.Collections;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Events;

namespace UserAccount
{
    /// <summary>
    /// Handles user device type detection and login with device-specific information.
    /// </summary>
    public class CustomerLoginHandler : MonoBehaviour
    {
        private string _deviceID;

        public UnityEvent NotContainAccountWithDeviceId;

        /// <summary>
        /// Initiates the account creation and login process based on the detected device type.
        /// </summary>
        [ContextMenu("Start Login")]
        public void StartLogin()
        {
            LoadingManger.Instance.Open_Loading_animator();
            CheckContainAccountWithDeviceId();
        }
        
        #region Check Contain Account With DeviceId

        /// <summary>
        /// Checks if an account exists for the detected device type and initiates login accordingly.
        /// </summary>
        private void CheckContainAccountWithDeviceId()
        {
            var deviceIdGetter = new DeviceIdGetter();
            bool hasPhoneDevice = deviceIdGetter.GetDeviceID(out string androidID, out string iosID, out string customID);

            if (hasPhoneDevice)
            {
                if (!string.IsNullOrEmpty(androidID))
                {
                    _deviceID = androidID;
                    CheckContainAccountWithAndroidDevice(androidID);
                }
                else if (!string.IsNullOrEmpty(iosID))
                {
                    _deviceID = iosID;
                    CheckContainAccountWithIOSDevice(iosID);
                }
            }
            else
            {
                _deviceID = customID;
                CheckContainAccountWithCustomID(customID);
            }
        }

        /// <summary>
        /// Initiates a login request with the iOS device ID.
        /// </summary>
        private void CheckContainAccountWithIOSDevice(string iosID)
        {
            PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
            {
                DeviceId = iosID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = false
            }, CheckContainSuccess, CheckContainFail);
        }

        /// <summary>
        /// Initiates a login request with the Android device ID.
        /// </summary>
        private void CheckContainAccountWithAndroidDevice(string androidID)
        {
            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
            {
                AndroidDeviceId = androidID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = false
            }, CheckContainSuccess, CheckContainFail);
        }

        /// <summary>
        /// Initiates a login request with the custom device ID.
        /// </summary>
        private void CheckContainAccountWithCustomID(string customID)
        {
            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                CustomId = customID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = false
            }, CheckContainSuccess, CheckContainFail);
        }
        
        /// <summary>
        /// Handles the success response of the account containment check.
        /// </summary>
        private void CheckContainSuccess(LoginResult result)
        {
            Debug.Log($"Device contains an account with {_deviceID}");
            CreateAccountWithDevice();
        }

        /// <summary>
        /// Handles the failure response of the account containment check.
        /// </summary>
        private void CheckContainFail(PlayFabError error)
        {
            LoadingManger.Instance.Close_Loading_animator();
            Debug.Log($"Device does not contain an account");
            NotContainAccountWithDeviceId.Invoke();
        }

        #endregion

        #region Create Account

        /// <summary>
        /// Creates an account using the device-specific information.
        /// </summary>
        public void CreateAccountWithDevice()
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            var deviceIdGetter = new DeviceIdGetter();
            bool hasPhoneDevice = deviceIdGetter.GetDeviceID(out string androidID, out string iosID, out string customID);

            if (hasPhoneDevice)
            {
                if (!string.IsNullOrEmpty(androidID))
                {
                    _deviceID = androidID;
                    CreateAccountWithAndroidDevice(androidID);
                }
                else if (!string.IsNullOrEmpty(iosID))
                {
                    _deviceID = iosID;
                    CreateAccountWithIOSDevice(iosID);
                }
            }
            else
            {
                _deviceID = customID;
                CreateAccountWithCustomID(customID);
            }
        }

        /// <summary>
        /// Initiates an account creation request with the iOS device ID.
        /// </summary>
        private void CreateAccountWithIOSDevice(string iosID)
        {
            Debug.Log("Using iOS Device ID: " + iosID);

            PlayFabClientAPI.LoginWithIOSDeviceID(new LoginWithIOSDeviceIDRequest()
            {
                DeviceId = iosID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, OnDeviceLoginSuccess, OnDeviceLoginFail);
        }

        /// <summary>
        /// Initiates an account creation request with the Android device ID.
        /// </summary>
        private void CreateAccountWithAndroidDevice(string androidID)
        {
            Debug.Log("Using Android Device ID: " + androidID);

            PlayFabClientAPI.LoginWithAndroidDeviceID(new LoginWithAndroidDeviceIDRequest()
            {
                AndroidDeviceId = androidID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, OnDeviceLoginSuccess, OnDeviceLoginFail);
        }

        /// <summary>
        /// Initiates an account creation request with the custom device ID.
        /// </summary>
        private void CreateAccountWithCustomID(string customID)
        {
            Debug.Log("Using custom device ID: " + customID);

            PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest()
            {
                CustomId = customID,
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true
            }, OnDeviceLoginSuccess, OnDeviceLoginFail);
        }
        
        /// <summary>
        /// Handles the success response of the device login.
        /// </summary>
        private void OnDeviceLoginSuccess(LoginResult result)
        {
            StartCoroutine(DeviceLoginSuccessCoroutine(result));
            
        }

        private IEnumerator DeviceLoginSuccessCoroutine(LoginResult result)
        {
            // Create Nick Name if create New Account
            if (result.NewlyCreated)
            {
                yield return AccountInitializer.AccountInitializer.CreateNickNameCoroutine();
            }
            
            Debug.Log($"Successful login with {_deviceID}");
            UserAccountManager.Instance.HandlePlayFabLoginSuccess(result);
        }

        /// <summary>
        /// Handles the failure response of the device login.
        /// </summary>
        private void OnDeviceLoginFail(PlayFabError error)
        {
            UserAccountManager.Instance.HandlePlayFabLoginFailed(error);
        }

        #endregion

    }
}
