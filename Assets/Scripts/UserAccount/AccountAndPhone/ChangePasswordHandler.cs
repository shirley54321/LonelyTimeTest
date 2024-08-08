using LitJson;
using Loading;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UserAccount.Tool;


namespace UserAccount
{
    [SerializeField]
    public class CheckAccountAndPhoneText
    {
        public string username;

        public string phone;
    }

    [SerializeField]
    public class ResetAccountText
    {
        public string username;

        public string verifyCode;

        public string password;
    }
    /// <summary>
    /// Handles the change password process.
    /// </summary>
    public class ChangePasswordHandler : MonoBehaviour
    {
        [SerializeField] private SMSHandler smsHandler;
        [SerializeField] private string _phoneNumber;
        [SerializeField] private string _userName;

        private string captchaNumberCheck;
        public Captcha captcha;

        public Text CountDownText;
        public Button ReSendButton;

        private int Time = 0;

        #region Event

        /// <summary>
        /// Event invoked when the validation of an AccountInfo is successful.
        /// Subscribers can attach methods to this event to respond to successful SMS validation.
        /// </summary>
        public UnityEvent OnValidatAccountInfoSuccessful = new UnityEvent();

        /// <summary>
        /// Event invoked when the validation of an AccountInfo fails.
        /// Subscribers receive a specific failure code (ValidateAccountInfoFailedCode) indicating the reason for the failure.
        /// Subscribers can attach methods to this event to handle different failure scenarios.
        /// </summary>
        public UnityEvent<ValidateAccountInfoFailedCode> OnValidateAccountFailed =
            new UnityEvent<ValidateAccountInfoFailedCode>();

        /// <summary>
        /// Event invoked when the password reset is successful.
        /// Subscribers can attach methods to this event
        /// </summary>
        public UnityEvent ResetPasswordSuccessful = new UnityEvent();

        /// <summary>
        /// Event invoked when the password reset fails.
        /// Subscribers receive a specific failure code (ResetPasswordFailedCode) indicating the reason for the failure.
        /// Subscribers can attach methods to this event to handle different failure scenarios.
        /// </summary>
        public UnityEvent<ResetPasswordFailedCode> OnResetPasswordFailed =
            new UnityEvent<ResetPasswordFailedCode>();


        #endregion
        
        /// <summary>
        /// Coroutine for validating account information in the forget account page.
        /// </summary>
        private Coroutine _validateInfoCoroutine;

        /// <summary>
        /// Coroutine for the password reset process.
        /// </summary>
        private Coroutine _resetPasswordCoroutine;


        #region ValidateAccountInfo (In Forget Account Page)

        /// <summary>
        /// Initiates the validation of account information in the forget account page.
        /// </summary>
        public void ValidateAccountInfo(string account, string phone, string imageValidateCode)
        {
            _validateInfoCoroutine = StartCoroutine(StartValidateInfoCoroutine(account, phone, imageValidateCode));
        }

        /// <summary>
        /// Coroutine for validating account information in the forget account page.
        /// </summary>
        /// <param name="account">The account to validate.</param>
        /// <param name="phone">The phone number to validate.</param>
        /// <param name="imageValidateCode">The image validation code to validate.</param>
        private IEnumerator StartValidateInfoCoroutine(string username, string phone, string imageValidateCode)
        {
            // Wait until a response is received
            yield return CheckImageValidateCode(imageValidateCode);

            _phoneNumber = phone;
            _userName = username;
            LoadingManger.Instance.Open_Loading_animator();
            // Wait until a response is received
            yield return CheckAccountAndPhone(username, phone);

            yield return smsHandler.SendSMS(phone, username);
            LoadingManger.Instance.Close_Loading_animator();
            if (smsHandler.IsSendSMSSuccessful())
            {
                Time = 60;
                StartCoroutine(TimeCountDown());
                Debug.Log("Validate Account Successful");
                OnValidatAccountInfoSuccessful.Invoke();
            }
        }

        /// <summary>
        /// Coroutine for checking the image validation code.
        /// </summary>
        private IEnumerator CheckImageValidateCode(string captchaTextCheck)
        {
            // TODO: Implement image validation logic
            bool isResponseReceived = false;
            captcha = FindAnyObjectByType<Captcha>();
            captchaNumberCheck = "";
            captchaNumberCheck = captcha.captchaTextOne + captcha.captchaTextTwo + captcha.captchaTextThree + captcha.captchaTextFour;

            if (true) // if pass
            {
                //判斷字串是否為空
                if (captchaTextCheck.Length != 0)
                {                    //判斷字串是否相同
                    if (string.Compare(captchaNumberCheck, captchaTextCheck) == 0)
                    {
                        isResponseReceived = true;
                    }
                    else
                    {
                        captchaTextCheck = "";
                        InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                    }
                }
                else
                {
                    InstanceRemindPanel.Instance.OpenPanel("請輸入驗證碼");
                }
            }
            else
            {
                ValidateAccountInfoFailed(ValidateAccountInfoFailedCode.ImageValidationCodeWrong);
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }
        /// <summary>
        /// Coroutine for checking the account and phone.
        /// </summary>
        private IEnumerator CheckAccountAndPhone(string username, string phone)
        {
            // TODO: Implement account and phone validation logic 
            bool isResponseReceived = false;
            Debug.Log(phone);
            CheckAccountAndPhoneText sendText = new CheckAccountAndPhoneText();
            sendText.username = username;
            sendText.phone = phone;
            string jsonString = JsonMapper.ToJson(sendText);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/VerifyUserNameAndPhone?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
            {
                //wait result
                webRequest.SetRequestHeader("ContentType", "application/json;charset=utf-8");
                webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
                yield return webRequest.SendWebRequest();

                // TODO: Implement PlayFab password reset logic
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    // TODO: Check the reason for password reset failure
                    Debug.Log(webRequest.downloadHandler.text);
                    Debug.Log(webRequest.error);
                    ValidateAccountInfoFailed(ValidateAccountInfoFailedCode.AccountOrPhoneWrong);
                    LoadingManger.Instance.Close_Loading_animator();
                }
                else // Reset Account Success
                {
                    Debug.Log(webRequest.downloadHandler.text);
                    if (webRequest.downloadHandler.text == "1")
                    {
                        isResponseReceived = true;
                    }
                    else
                    {
                        InstanceRemindPanel.Instance.OpenPanel("驗證錯誤");
                        LoadingManger.Instance.Close_Loading_animator();
                    }
                }
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        /// <summary>
        /// Handles the failure of validating account information.
        /// </summary>
        /// <param name="failedCode">Code indicating the reason for the failure.</param>
        private void ValidateAccountInfoFailed(ValidateAccountInfoFailedCode failedCode)
        {
            Debug.Log($"<color=red>Validate Account Failed {failedCode}</color>");
            StopCoroutine(_validateInfoCoroutine);
            OnValidateAccountFailed.Invoke(failedCode);
        }

        #endregion

        #region Reset Password

        /// <summary>
        /// Sends an SMS to the saved phone number.
        /// </summary>
        public void SendSMS()
        {
            if (Time == 0)
            {
                StartCoroutine(ReSendSMS());
            }
        }

        IEnumerator ReSendSMS()
        {
            yield return smsHandler.SendSMS(_phoneNumber, _userName);
            if (smsHandler.IsSendSMSSuccessful())
            {
                Time = 60;
                StartCoroutine(TimeCountDown());
            }
        }

        IEnumerator TimeCountDown()
        {
            int tempTime = Time;
            CountDownText.text = $"重新發送({Time}秒)";
            ReSendButton.interactable = false;

            while (tempTime > 0)
            {
                yield return new WaitForSecondsRealtime(1);
                tempTime--;
                Time--;
                CountDownText.text = $"重新發送({Time}秒)";
            }

            yield return new WaitForSecondsRealtime(1);
            CountDownText.text = "寄送驗證碼";
            ReSendButton.interactable = true;
        }


        /// <summary>
        /// Initiates the reset password process.
        /// </summary>
        public void StartResetPassword(string password, string confirmPassword, string verificationCode)
        {
            _resetPasswordCoroutine =
                StartCoroutine(ResetPasswordCoroutine(password, confirmPassword, verificationCode));

            Debug.Log(_resetPasswordCoroutine);
        }

        private IEnumerator ResetPasswordCoroutine(string password, string confirmPassword, string verificationCode)
        {
            // In order to wait for this coroutine, save it in _resetPasswordCoroutine variable
            yield return new WaitForNextFrameUnit();

            // Validate the format of the password and confirmation password
            ValidatePasswordFormat(password, confirmPassword);

            // Validate the SMS verification code
            // Wait until a response is received
            yield return ResetAccount(password, _userName, verificationCode);
            //HandleValidateSMSOutcome();

            // Reset the account password
            // Wait until a response is received

            Debug.Log("Reset Password Successful");

            // Invoke the event to notify subscribers about successful password reset
            ResetPasswordSuccessful.Invoke();
        }

        /// <summary>
        /// Handles the outcome of SMS validation.
        /// </summary>
        private void HandleValidateSMSOutcome()
        {
            // Check if SMS validation was not successful
            if (!smsHandler.IsValidateSuccessful())
            {
                // If the validation code is wrong, handle it as a validation failure
                if (smsHandler.GetFailedCode() == ValidateSMSFailedCode.ValidationCodeWrong)
                {
                    ResetPasswordFailed(ResetPasswordFailedCode.ValidationCodeWrong);
                }
                // TODO: Add other failure reasons for SMS
            }
        }

        /// <summary>
        /// Validates the format of the password
        /// </summary>
        /// <param name="password">Password to validate.</param>
        /// <param name="confirmPassword">Confirmation of the password.</param>
        /// <returns>True if validation succeeds, otherwise false.</returns>
        private void ValidatePasswordFormat(string password, string confirmPassword)
        {
            if (!AccountValidator.ValidateFormat(password))
            {
                ResetPasswordFailed(ResetPasswordFailedCode.PasswordFormatWrong);
                return;
            }

            if (!AccountValidator.ValidatePasswordAndConfirmPassword(password, confirmPassword))
            {
                ResetPasswordFailed(ResetPasswordFailedCode.PasswordUnequal);
                return;
            }
        }

        // TODO ResetPassword with PlayFab
        /// <summary>
        /// Coroutine for resetting the account password using PlayFab.
        /// </summary>
        private IEnumerator ResetAccount(string password, string username, string verifyCode)
        {
            bool isResponseReceived = false;
            LoadingManger.Instance.Open_Loading_animator();
            ResetAccountText sendText = new ResetAccountText();
            sendText.username = username;
            sendText.verifyCode = verifyCode;
            sendText.password = password;
            string jsonString = JsonMapper.ToJson(sendText);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/ResetPassword?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
            {
                //wait result
                webRequest.SetRequestHeader("ContentType", "application/json;charset=utf-8");
                webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
                yield return webRequest.SendWebRequest();

                // TODO: Implement PlayFab password reset logic
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    // TODO: Check the reason for password reset failure
                    Debug.Log(webRequest.error);
                    ResetPasswordFailed(ResetPasswordFailedCode.ValidationCodeWrong);
                }
                else // Reset Account Success
                {
                    switch (webRequest.downloadHandler.text)
                    {
                        case "true":// Reset Account Success
                            InstanceRemindPanel.Instance.OpenPanel("修改成功");
                            isResponseReceived = true;
                            break;
                        case "SMSHasExpired":
                            InstanceRemindPanel.Instance.OpenPanel("驗證碼過期");
                            break;
                        case "SMSError":
                            InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                            break;
                        default:
                            break;
                    }
                    Debug.Log(webRequest.downloadHandler.text);
                }
                LoadingManger.Instance.Close_Loading_animator();
            }
            yield return new WaitUntil(() => isResponseReceived);
        }
        
        /// <summary>
        /// Handles the failure of the password reset process.
        /// </summary>
        /// <param name="failCode">Code indicating the reason for the failure.</param>
        private void ResetPasswordFailed(ResetPasswordFailedCode failCode)
        {
            Debug.Log($"<color=red>Reset Password Failed {failCode}</color>");
            if (_resetPasswordCoroutine != null)
                StopCoroutine(_resetPasswordCoroutine);
            OnResetPasswordFailed.Invoke(failCode);
        }

        #endregion
    }
}