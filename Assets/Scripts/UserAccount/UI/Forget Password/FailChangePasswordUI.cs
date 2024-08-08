using UnityEngine;

namespace UserAccount.UI
{
    public class FailChangePasswordUI : MonoBehaviour
    {
        [SerializeField] private ChangePasswordHandler changePasswordHandler;
        
        private void OnEnable()
        {
            changePasswordHandler.OnValidateAccountFailed.AddListener(OpenValidateAccountFailedPanel);
            changePasswordHandler.OnResetPasswordFailed.AddListener(OpenResetAccountFailedPanel);
        }

        private void OnDisable()
        {
            changePasswordHandler.OnValidateAccountFailed.RemoveListener(OpenValidateAccountFailedPanel);
            changePasswordHandler.OnResetPasswordFailed.RemoveListener(OpenResetAccountFailedPanel);
        }

        private void OpenValidateAccountFailedPanel(ValidateAccountInfoFailedCode failedCode)
        {
            Debug.Log($"ValidateAccountFailed {failedCode}");
            switch (failedCode)
            {
                case ValidateAccountInfoFailedCode.AccountOrPhoneWrong:
                    InstanceRemindPanel.Instance.OpenPanel("帳號或手機輸入錯誤");
                    break;
                case ValidateAccountInfoFailedCode.ImageValidationCodeWrong:
                    InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                    break;
            }
        }
        
        private void OpenResetAccountFailedPanel(ResetPasswordFailedCode failedCode)
        {
            Debug.Log($"ResetAccountFailed {failedCode}");
            switch (failedCode)
            {
                case ResetPasswordFailedCode.PasswordUnequal:
                    InstanceRemindPanel.Instance.OpenPanel("確認新密碼輸入錯誤");
                    break;
                case ResetPasswordFailedCode.PasswordFormatWrong:
                    InstanceRemindPanel.Instance.OpenPanel("帳號或密碼須為8~12位英文數字組合");
                    break;
                case ResetPasswordFailedCode.ValidationCodeWrong:
                    InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                    break;
                case ResetPasswordFailedCode.PasswordIsSameAsOldPassword:
                    InstanceRemindPanel.Instance.OpenPanel("新密碼不得與舊密碼相同");
                    break;
            }
        }
        
    }
}