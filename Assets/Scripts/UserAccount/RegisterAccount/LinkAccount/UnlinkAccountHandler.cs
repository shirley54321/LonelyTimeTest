using Loading;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
namespace UserAccount.LinkAccount
{
    /// <summary>
    /// Handles the unlinking of user accounts with different types of devices or custom IDs.
    /// </summary>
    public static class UnlinkAccountHandler
    {
        private static readonly  QuickLoginIdHandler quickLoginIdHandler = new QuickLoginIdHandler();

        /// <summary>
        /// Unlinks the user account based on the type of device (iOS, Android, or custom).
        /// </summary>
        public static void UnlinkAccountWithDevice()
        {
            var deviceIdGetter = new DeviceIdGetter();
            bool hasPhoneDevice = deviceIdGetter.GetDeviceID(out string androidID, out string iosID, out string customID);

            if (hasPhoneDevice)
            {
                if (!string.IsNullOrEmpty(androidID))
                {
                    UnlinkAccountWithAndroidDevice(androidID);
                }
                else if (!string.IsNullOrEmpty(iosID))
                {
                    UnlinkAccountWithIOSDevice(iosID);
                }
            }
            else
            {
                UnlinkAccountWithCustomID(customID);
            }
        }

        /// <summary>
        /// Unlinks the user account associated with the iOS device ID.
        /// </summary>
        /// <param name="iosID">iOS device ID</param>
        private static  void UnlinkAccountWithIOSDevice(string iosID)
        {
            Debug.Log("Using iOS Device ID: " + iosID);

            PlayFabClientAPI.UnlinkIOSDeviceID(new UnlinkIOSDeviceIDRequest()
            {
                DeviceId = iosID,
            }, result =>
            {
                // quickLoginIdHandler.ClearGuID();
                Debug.Log($"Unlink IOS Device {iosID} successful");
            }, OnUnlinkAccountFail);
        }

        /// <summary>
        /// Unlinks the user account associated with the Android device ID.
        /// </summary>
        /// <param name="androidID">Android device ID</param>
        private static void UnlinkAccountWithAndroidDevice(string androidID)
        {
            Debug.Log("Using Android Device ID: " + androidID);

            PlayFabClientAPI.UnlinkAndroidDeviceID(new UnlinkAndroidDeviceIDRequest()
            {
                AndroidDeviceId = androidID
            }, result =>
            {
                // quickLoginIdHandler.ClearGuID();
                Debug.Log($"Unlink Android Device {androidID} successful");
            }, OnUnlinkAccountFail);
        }

        /// <summary>
        /// Unlinks the user account associated with the custom device ID.
        /// </summary>
        /// <param name="customID">Custom device ID</param>
        private static void UnlinkAccountWithCustomID(string customID)
        {
            Debug.Log("Using custom device ID: " + customID);

            PlayFabClientAPI.UnlinkCustomID(new UnlinkCustomIDRequest()
            {
                CustomId = customID
            }, result =>
            {
                quickLoginIdHandler.ClearGuID();
                Debug.Log($"Unlink Customer Id {customID} successful");
            }, OnUnlinkAccountFail);
        }

        /// <summary>
        /// Handles the failure response of the device unlinking operation.
        /// </summary>
        /// <param name="error">PlayFab error object</param>
        private static  void OnUnlinkAccountFail(PlayFabError error)
        {
            Debug.Log($"<color=red>Unsuccessful unlink Customer ID</color>\n " +
                      $"{error.Error}\n" +
                      $"{error.ErrorMessage}");
            
        }
    }
}
