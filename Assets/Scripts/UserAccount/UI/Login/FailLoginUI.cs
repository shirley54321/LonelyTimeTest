using System;
using UnityEngine;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the UI elements for failed login scenarios and subscribes to the OnLoginFailed event.
    /// </summary>
    public class FailLoginUI : MonoBehaviour
    {
        private void OnEnable()
        {
            // Subscribe to the OnLoginFailed event
            UserAccountManager.OnLoginFailed.AddListener(OpenLoginFailPanel);
        }

        private void OnDisable()
        {
            // Unsubscribe from the OnLoginFailed event to prevent memory leaks
            UserAccountManager.OnLoginFailed.RemoveListener(OpenLoginFailPanel);
        }

        /// <summary>
        /// Opens the appropriate UI panel based on the failed login code.
        /// </summary>
        /// <param name="failedCode">The code indicating the reason for the failed login.</param>
        private void OpenLoginFailPanel(LoginFailedCode failedCode)
        {
            // Display the corresponding UI panel based on the failed login code
            switch (failedCode)
            {
                // TODO Login Fail Reason
                case LoginFailedCode.IncorrectAccountOrPassword:
                    InstanceRemindPanel.Instance.OpenPanel("帳號或密碼輸入錯誤，請重新輸入");
                    break;
                case LoginFailedCode.IncorrectPasswordTimesExceedLimited:
                    InstanceRemindPanel.Instance.OpenPanel("帳號或密碼錯誤已達上限，請5分鐘後再進行嘗試");
                    break;
                case LoginFailedCode.AccountCanceledInConsiderationPeriod:
                    UserAccountManager.Instance.InvokeLoginSuccess();
                    InstanceRemindPanel.Instance.OpenPanel("您已取消帳號註銷");
                    break;
                case LoginFailedCode.AccountCanceled:
                    InstanceRemindPanel.Instance.OpenPanel("無法登入，帳號已註銷");
                    break;
                case LoginFailedCode.AccountBanned:
                    InstanceRemindPanel.Instance.OpenPanel("<size=42>您的帳號因違反使用條款目前無法使用，\n如有相關問題請查閱我們的「遊戲規章」或\n聯絡客服：service@lonelytime777.com</size>");
                    break;
                //case LoginFailedCode.OtherReason:
                //    InstanceRemindPanel.Instance.OpenPanel("帳號或密碼錯誤已達上限，請5分鐘後再進行嘗試");
                //    break;
            }
        }
    }
}