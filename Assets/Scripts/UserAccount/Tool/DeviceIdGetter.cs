using UnityEngine;

namespace UserAccount
{
    public class DeviceIdGetter
    {
        public string GetDeviceID()
        {
            string _deviceID = "";
            bool hasPhoneDevice = GetDeviceID(out string androidID, out string iosID, out string customID);

            if (hasPhoneDevice)
            {
                if (!string.IsNullOrEmpty(androidID))
                {
                    _deviceID = androidID;
                }
                else if (!string.IsNullOrEmpty(iosID))
                {
                    _deviceID = iosID;
                }
            }
            else
            {
                _deviceID = customID;
            }

            return _deviceID;
        }
        
        
        /// <summary>
        /// Gets the device-specific IDs for Android, iOS, and custom platforms.
        /// </summary>
        /// <param name="androidID">Android device ID.</param>
        /// <param name="iosID">iOS device ID.</param>
        /// <param name="customID">Custom device ID.</param>
        /// <returns>True if the device is a supported mobile platform, false otherwise.</returns>
        public bool GetDeviceID(out string androidID, out string iosID, out string customID)
        {
            androidID = string.Empty;
            iosID = string.Empty;
            customID = string.Empty;

            if (CheckForSupportedMobilePlatform())
            {
#if UNITY_ANDROID
                // http://answers.unity3d.com/questions/430630/how-can-i-get-android-id-.html
                AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
                androidID = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif

#if UNITY_IPHONE
                iosID = UnityEngine.iOS.Device.vendorIdentifier;
#endif
                return true;
            }
            else
            {
                customID = SystemInfo.deviceUniqueIdentifier;
                return false;
            }
        }

        /// <summary>
        /// Checks if the current platform is a supported mobile platform.
        /// </summary>
        /// <returns>True if the platform is Android or iOS, false otherwise.</returns>
        private bool CheckForSupportedMobilePlatform()
        {
            return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
        }

    }

}
