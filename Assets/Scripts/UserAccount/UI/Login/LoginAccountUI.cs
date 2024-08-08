using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the UI elements for logging in user accounts.
    /// </summary>
    public class LoginAccountUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private TMP_InputField accountNameInputField, passwordInputField;

        [SerializeField] private Button confirmButton;

        [SerializeField] private AccountLoginHandler accountLoginHandler;

        /// <summary>
        /// Called when the object becomes enabled and active.
        /// </summary>
        void OnEnable()
        {
            UpdateConfirmButtonEnableClick();
        }

        
        /// <summary>
        /// Opens the panel.
        /// </summary>
        [ContextMenu("Open Panel")]
        public virtual void OpenPanel()
        {
            accountNameInputField.text = "";
            passwordInputField.text = "";
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
        /// Initiates the login process using the provided account name and password.
        /// </summary>
        public void LoginAccount()
        {
            var accountName = accountNameInputField.text;
            var password = passwordInputField.text;

            if(accountName.Length < 8 | accountName.Length > 12 | password.Length < 8 | password.Length > 12)
                InstanceRemindPanel.Instance.OpenPanel("帳號或密碼需為8~12位\n英文數字組合");
            else
                accountLoginHandler.StartLogin(accountName, password);
        }

        /// <summary>
        /// Updates the confirmation button's interactability based on input field values.
        /// </summary>
        public void UpdateConfirmButtonEnableClick()
        {
            bool enableClick = (accountNameInputField.text != "") || (passwordInputField.text != "");
            confirmButton.interactable = enableClick;
        }

        /// <summary>
        /// Opens the Forget Password UI.
        /// </summary>
        public void OpenForgetAccountUI()
        {
            LoginMediator.Instance.OpenForgetPasswordUI();
        }
        
        /// <summary>
        /// Opens the Register Account UI.
        /// </summary>
        public void OpenRegisterAccountUI()
        {
            LoginMediator.Instance.OpenRegisterAccountUI();
        }
    }
}