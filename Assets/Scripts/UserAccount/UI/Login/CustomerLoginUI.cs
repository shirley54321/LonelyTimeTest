using System;
using UnityEngine;
using UserAccount.Tool;

namespace UserAccount.UI
{
    /// <summary>
    /// Handles the UI for customer login.
    /// </summary>
    public class CustomerLoginUI : MonoBehaviour
    {
        [SerializeField] private UserRuleUI userRuleUI;
        [SerializeField] private CustomerLoginHandler customerLoginHandler;

        private void OnEnable()
        {
            customerLoginHandler.NotContainAccountWithDeviceId.AddListener(HandlerNotContainAccountWithDeviceId);
        }

        private void OnDisable()
        {
            customerLoginHandler.NotContainAccountWithDeviceId.RemoveListener(HandlerNotContainAccountWithDeviceId);
        }

        /// <summary>
        /// Initiates the customer login process.
        /// </summary>
        public void StartCustomerLogin()
        {
            customerLoginHandler.StartLogin();
        }

        /// <summary>
        /// Opens the UI for user rules.
        /// </summary>
        private void HandlerNotContainAccountWithDeviceId()
        {
            // If Device have click agree rule button, directly login, else open userRuleUI
            if (HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule())
            {
                customerLoginHandler.CreateAccountWithDevice();
            }
            else
            {
                userRuleUI.OpenPanel();
            }
        }
    }
}