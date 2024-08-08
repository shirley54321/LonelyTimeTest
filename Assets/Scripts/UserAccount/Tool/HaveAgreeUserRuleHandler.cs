using System;
using UnityEngine;


namespace UserAccount.Tool
{
    /// <summary>
    /// Provides methods to set and check whether a device has agreed to user rules.
    /// </summary>
    public static class HaveAgreeUserRuleHandler
    {
        /// <summary>
        /// Sets the agreement status for user rules on the device.
        /// </summary>
        /// <param name="haveRule">A boolean indicating whether the user has agreed to the rules.</param>
        public static void SetDeviceHaveAgreeRule(bool haveRule)
        {
            PlayerPrefs.SetInt("DeviceHaveAgreeRule", Convert.ToInt16(haveRule));
        }
        
        /// <summary>
        /// Checks whether the device has agreed to user rules.
        /// </summary>
        /// <returns>True if the device has agreed to user rules; otherwise, false.</returns>
        public static bool IsDeviceHaveAgreeRule()
        {
            var haveAgreeRule = Convert.ToBoolean(PlayerPrefs.GetInt("DeviceHaveAgreeRule", 0));
            Debug.Log($"Have Agree Rule: {haveAgreeRule}");
            return haveAgreeRule;
        }
    }
}