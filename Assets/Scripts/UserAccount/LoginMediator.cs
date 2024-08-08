using UnityEngine;
using UserAccount.UI;

namespace UserAccount
{
    /// <summary>
    /// Mediator class responsible for managing UI interactions related to login.
    /// </summary>
    public class LoginMediator : MonoBehaviour
    {
        #region Instance(Singleton)

        private static LoginMediator instance;

        /// <summary>
        /// Singleton instance of LoginMediator.
        /// </summary>
        public static LoginMediator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoginMediator>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(LoginMediator)} does not exist in the scene, yet its method is being called.\n" +
                                       $"Please add {typeof(LoginMediator)} to the scene.");
                    }
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        [SerializeField] private CustomerLoginUI customerLoginUI;
        [SerializeField] private LoginAccountUI loginAccountUI;
        [SerializeField] private RegisterAccountUI registerAccountUI;
        [SerializeField] private ForgetPasswordUI forgetPasswordUI;
        [SerializeField] private UserRuleUI UserRuleUI;
        public void OpenLoginAccountUI()
        {
            loginAccountUI.OpenPanel();
        }
        
        /// <summary>
        /// Opens the Forget Password UI.
        /// </summary>
        public void OpenForgetPasswordUI()
        {
            forgetPasswordUI.OpenPanel();
        }

        /// <summary>
        /// Opens the Register Account UI.
        /// </summary>
        public void OpenRegisterAccountUI()
        {
            registerAccountUI.CheckThenOpenRegisterPanel();
        }

        public void StartCustomerLogin()
        {
           customerLoginUI.StartCustomerLogin();
        }

        public void OpenFBLoginUI()
        {
            UserRuleUI.OpenPanelForFB();
        }

        public void OpenGoogleLoginUI()
        {
            UserRuleUI.OpenPanelForGoogle();
        }

        public void OpenAppleLoginUI()
        {
            UserRuleUI.OpenPanelForApple();
        }
    }
}