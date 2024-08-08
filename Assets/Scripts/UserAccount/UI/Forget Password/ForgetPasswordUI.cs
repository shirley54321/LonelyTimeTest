using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the user interface for the forget password functionality.
    /// </summary>
    public class ForgetPasswordUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TMP_InputField accountInputField, phoneInputField,
            imageValidationCodeInputField;

        [SerializeField] private ChangePasswordHandler changePasswordHandler;
        [SerializeField] private Captcha captcha;

        [SerializeField] private ChangePasswordUI changePasswordUI;

        /// <summary>
        /// Subscribe to events when the script is enabled.
        /// </summary>
        private void OnEnable()
        {
            changePasswordHandler.OnValidatAccountInfoSuccessful.AddListener(OnValidateSuccessful);
        }

        /// <summary>
        /// Unsubscribe from events when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            changePasswordHandler.OnValidatAccountInfoSuccessful.RemoveListener(OnValidateSuccessful);
        }

        /// <summary>
        /// Opens the panel.
        /// </summary>
        [ContextMenu("Open Panel")]
        public virtual void OpenPanel()
        {
            accountInputField.text = "";
            phoneInputField.text = "";
            imageValidationCodeInputField.text = "";
            mainPanel.SetActive(true);
            captcha.ChangeCaptcha();
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
        /// Validates the provided account information.
        /// </summary>
        public void ValidateAccountInfo()
        {
            if (phoneInputField.text.Length == 10)
            {
                var account = accountInputField.text;
                var phone = phoneInputField.text;
                var imageValidateCode = imageValidationCodeInputField.text;
                changePasswordHandler.ValidateAccountInfo(account, phone, imageValidateCode);
            }
            else
            {
                InstanceRemindPanel.Instance.OpenPanel("手機格式有誤，請您重新輸入");
            }
        }

        /// <summary>
        /// Handles the event when account information validation is successful.
        /// </summary>
        private void OnValidateSuccessful()
        {
            // Open the change password panel and close the forget password panel.
            changePasswordUI.OpenPanel();
            accountInputField.text = "";
            phoneInputField.text = "";
            imageValidationCodeInputField.text = "";
            ClosePanel();
        }
    }
}
