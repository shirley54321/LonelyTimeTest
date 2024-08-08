using Loading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the UI for changing the password.
    /// </summary>
    public class ChangePasswordUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TMP_InputField passwordInputField, confirmPasswordInputField,
            phoneValidationCodeInputField;

        [SerializeField] private ChangePasswordHandler changePasswordHandler;

        private void OnEnable()
        {
            changePasswordHandler.ResetPasswordSuccessful.AddListener(ChangePasswordSuccessful);
        }

        /// <summary>
        /// Unsubscribe from events when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            changePasswordHandler.ResetPasswordSuccessful.RemoveListener(ChangePasswordSuccessful);
        }

        /// <summary>
        /// Opens the panel.
        /// </summary>
        [ContextMenu("Open Panel")]
        public virtual void OpenPanel()
        {
            passwordInputField.text = "";
            confirmPasswordInputField.text = "";
            phoneValidationCodeInputField.text = "";
            mainPanel.SetActive(true);
        }

        /// <summary>
        /// Closes the panel.
        /// </summary>
        [ContextMenu("Close Panel")]
        public virtual void ClosePanel()
        {
            mainPanel.SetActive(false);
        }

        /// <summary>
        /// Initiates the process of sending an SMS for password reset.
        /// </summary>
        public void SendSMS()
        {
            changePasswordHandler.SendSMS();
        }

        /// <summary>
        /// Initiates the process of resetting the password.
        /// </summary>
        public void ResetPassword()
        {
            var password = passwordInputField.text;
            var confirmPassword = confirmPasswordInputField.text;
            var phoneValidationCode = phoneValidationCodeInputField.text;
            changePasswordHandler.StartResetPassword(password, confirmPassword, phoneValidationCode);
        }

        private void ChangePasswordSuccessful()
        {
            // Open the change password panel and close the forget password panel.
            passwordInputField.text = "";
            confirmPasswordInputField.text = "";
            phoneValidationCodeInputField.text = "";
            InstanceRemindPanel.Instance.OpenPanel("修改成功");
            ClosePanel();
        }
    }
}