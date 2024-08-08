using UnityEngine;

namespace UserAccount.UI
{
    public class LoginLobbyUI : MonoBehaviour
    {
        public void CustomerLogin()
        {
            LoginMediator.Instance.StartCustomerLogin();
        }

        public void AccountLogin()
        {
            LoginMediator.Instance.OpenLoginAccountUI();
        }

        public void FBLogin()
        {
            LoginMediator.Instance.OpenFBLoginUI();
        }

        public void GoogleLogin()
        {
            LoginMediator.Instance.OpenGoogleLoginUI();
        }

        public void AppleLogin()
        {
            LoginMediator.Instance.OpenAppleLoginUI();
        }
    }
}