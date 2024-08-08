using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserAccount.Tool;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the user interface for registering a new account.
    /// </summary>
    public class RegisterAccountUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField accountNameInputField, passwordInputField, confirmPasswordInputField;
        [SerializeField] private string accountName, password, confirmPassword;

        [SerializeField] private Button confirmButton;
        
        [SerializeField] private GameObject mainPanel;
        
        [SerializeField] private RegisterAccountHandler registerAccountHandler;

        [SerializeField] private UserRuleUI userRuleUI;

        [Header("是否為綁定帳號用的 UI")]
        public bool isLinkAccount;

        /// <summary>
        /// Subscribe to events when the script is enabled.
        /// </summary>
        void OnEnable()
        {
            RegisterAccountHandler.OnRegisterAccountSuccess.AddListener(OpenRegisterAccountSuccessPanel);
            ResetUI();
        }

        /// <summary>
        /// Unsubscribe from events when the script is disabled.
        /// </summary>
        void OnDisable()
        {
            RegisterAccountHandler.OnRegisterAccountSuccess.RemoveListener(OpenRegisterAccountSuccessPanel);
        }

        #region Open Close Panel
        public void CheckThenOpenRegisterPanel()
        {
            // If Device have click agree rule button, directly enter register panel, else open userRuleUI
            if (HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule())
            {
                OpenPanel();
            }
            else
            {
                userRuleUI.OpenPanel();
            }
        }

        /// <summary>
        /// Open the main registration panel.
        /// </summary>
        [ContextMenu("Open Panel")]
        public virtual void OpenPanel()
        {
            accountNameInputField.text = ""; 
            passwordInputField.text= ""; 
            confirmPasswordInputField.text= ""; 
            mainPanel.SetActive(true);
        }

        /// <summary>
        /// Close the main registration panel.
        /// </summary>
        [ContextMenu("Close Panel")]
        public virtual void ClosePanel()
        {
            mainPanel.SetActive(false);
            ClearField();
        }

        private void ClearField()
        {
            accountNameInputField.text = null;
            passwordInputField.text = null;
            confirmPasswordInputField.text = null;
        }

        #endregion

        #region Method for UI object

        /// <summary>
        /// Register a new account based on the provided input.
        /// </summary>
        public void RegisterAccount()
        {
            SetAccountNameAndPassword();
            registerAccountHandler.RegisterAccount(accountName, password, confirmPassword);
        }

        /// <summary>
        /// Set the account name and password from input fields.
        /// </summary>
        private void SetAccountNameAndPassword()
        {
            accountName = accountNameInputField.text;
            password = passwordInputField.text;
            confirmPassword = confirmPasswordInputField.text;
            ResetUI();
        }

        /// <summary>
        /// Update the state of the confirm button based on input field values.
        /// </summary>
        public void UpdateConfirmButtonEnableClick()
        {
            bool enableClick = (accountNameInputField.text != "") &&
                               (passwordInputField.text != "") && (confirmPasswordInputField.text != ""); ;
            confirmButton.interactable = enableClick;
        }

        #endregion

        #region Register Account Success, Failed

        /// <summary>
        /// Open the registration success panel and close the main panel.
        /// </summary>
        private void OpenRegisterAccountSuccessPanel()
        {
            InstanceRemindPanel.Instance.OpenPanel(isLinkAccount ? "綁定帳號成功" : "帳號註冊成功");

            ClosePanel();
        }
        

        #endregion

        #region Update UI

        /// <summary>
        /// Reset UI elements to their default state.
        /// </summary>
        private void ResetUI()
        {
            UpdateConfirmButtonEnableClick();
        }

    

        #endregion
    }
}
