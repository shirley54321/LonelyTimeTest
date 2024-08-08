using UnityEngine;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles UI elements for displaying failure messages during account registration.
    /// </summary>
    public class FailRegisterAccountUI : MonoBehaviour
    {
        /// <summary>
        /// Subscribe to the event when the script is enabled.
        /// </summary>
        private void OnEnable()
        {
            RegisterAccountHandler.OnRegisterAccountFailed.AddListener(OpenLoginFailPanel);
        }

        /// <summary>
        /// Unsubscribe from the event when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            RegisterAccountHandler.OnRegisterAccountFailed.RemoveListener(OpenLoginFailPanel);
        }

        /// <summary>
        /// Open the appropriate failure panel based on the reason for failed account registration.
        /// </summary>
        /// <param name="failedCode">Reason for the failed registration.</param>
        private void OpenLoginFailPanel(RegisterFailedCode failedCode)
        {
            switch (failedCode)
            {
                case RegisterFailedCode.PasswordUnequal:
                    InstanceRemindPanel.Instance.OpenPanel("確認密碼不相符");
                    break;
                case RegisterFailedCode.AccountHaveRegistered:
                    InstanceRemindPanel.Instance.OpenPanel("此帳號已被註冊");
                    break;
                case RegisterFailedCode.AccountOrPasswordFormatWrong:
                    InstanceRemindPanel.Instance.OpenPanel("帳號或密碼須為 8~12 位\n英文數字組合");
                    break;
                case RegisterFailedCode.RegisterCountReachLimited:
                    InstanceRemindPanel.Instance.OpenPanel("裝置已綁定上限，請聯繫客服協助");
                    break;
                case RegisterFailedCode.Other:
                    InstanceRemindPanel.Instance.OpenPanel("註冊帳號失敗");
                    break;
            }
        }
    }
}
