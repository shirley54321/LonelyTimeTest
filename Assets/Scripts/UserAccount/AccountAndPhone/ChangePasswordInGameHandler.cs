using LitJson;
using Loading;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UserAccount;
using UserAccount.Tool;

[SerializeField]
public class ResetAccountInGameText
{
    public string username;

    public string password;
}
public class ChangePasswordInGameHandler : MonoBehaviour
{
    private Coroutine _resetPasswordCoroutine;
    [SerializeField] private GameObject panel, settingPanel, toggleGroup;
    [SerializeField] private string _phoneNumber;
    [SerializeField] private string _userName;
    [SerializeField] private InputField passwordInputField, oldPasswordInputField, confirmPasswordInputField;
    [SerializeField] private Button comformChangePasswordButton;
    /// <summary>
    /// Event invoked when the password reset fails.
    /// Subscribers receive a specific failure code (ResetPasswordFailedCode) indicating the reason for the failure.
    /// Subscribers can attach methods to this event to handle different failure scenarios.
    /// </summary>
    public UnityEvent<ResetPasswordFailedCode> OnResetPasswordFailed =
        new UnityEvent<ResetPasswordFailedCode>();

    /// <summary>
    /// Event invoked when the password reset is successful.
    /// Subscribers can attach methods to this event
    /// </summary>
    public UnityEvent ResetPasswordSuccessful = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Show/Close Panel

    public void ShowPanel()
    {
        panel.SetActive(true);
    }

    public void ClosePanel()
    {
        passwordInputField.text = "";
        oldPasswordInputField.text = "";
        confirmPasswordInputField.text = "";
        panel.SetActive(false);
        settingPanel.SetActive(true);
        toggleGroup.SetActive(true);
    }
    #endregion

    private IEnumerator LoginWithPlayFabAccount()
    {
        bool isResponseReceived = false;
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true
            },
            Username = PlayerInfoManager.Instance.AccountInfo.Username,
            Password = oldPasswordInputField.text
        },
            // Success
            result =>
            {
                Debug.Log("Success");
                isResponseReceived = true;
            },
            // Failure
            error =>
            {
                Debug.Log("password error");
                InstanceRemindPanel.Instance.OpenPanel("舊密碼錯誤");
                LoadingManger.Instance.Close_Loading_animator();
            }
        );
        yield return new WaitUntil(() => isResponseReceived);
    }

    public void ResetPassword()
    {
        StartResetPassword(passwordInputField.text, confirmPasswordInputField.text, oldPasswordInputField.text);

    }

    public void StartResetPassword(string password, string confirmPassword, string oldPassword)
    {
        _resetPasswordCoroutine =
            StartCoroutine(ResetPasswordCoroutine(password, confirmPassword, oldPassword));

        Debug.Log(_resetPasswordCoroutine);
    }

    private IEnumerator ResetPasswordCoroutine(string password, string confirmPassword, string oldPassword)
    {
        var username = PlayerInfoManager.Instance.AccountInfo.Username;
        // In order to wait for this coroutine, save it in _resetPasswordCoroutine variable
        yield return new WaitForNextFrameUnit();

        // Validate the format of the password and confirmation password
        yield return ValidatePasswordFormat(password, confirmPassword, oldPassword);

        LoadingManger.Instance.Open_Loading_animator();
        yield return LoginWithPlayFabAccount();

        // Validate the SMS verification code
        // Wait until a response is received
        yield return ResetAccount(password, username);
        //HandleValidateSMSOutcome();

        // Reset the account password
        // Wait until a response is received

        Debug.Log("Reset Password Successful");

        // Invoke the event to notify subscribers about successful password reset
        ResetPasswordSuccessful.Invoke();

        ClosePanel();
        LoadingManger.Instance.Close_Loading_animator();
    }

    /// <summary>
    /// Validates the format of the password
    /// </summary>
    /// <param name="password">Password to validate.</param>
    /// <param name="confirmPassword">Confirmation of the password.</param>
    /// <returns>True if validation succeeds, otherwise false.</returns>
    private IEnumerator ValidatePasswordFormat(string password, string confirmPassword, string oldPassword)
    {
        bool isResponseReceived = true;
        if (!AccountValidator.ValidateFormat(password))
        {
            //ResetPasswordFailed(ResetPasswordFailedCode.PasswordFormatWrong);
            InstanceRemindPanel.Instance.OpenPanel("密碼格式不符");
            isResponseReceived = false;
        }

        if (!AccountValidator.ValidatePasswordAndConfirmPassword(password, confirmPassword))
        {
            //ResetPasswordFailed(ResetPasswordFailedCode.PasswordUnequal);
            InstanceRemindPanel.Instance.OpenPanel("確認新密碼與新密碼不符");
            isResponseReceived = false;
        }

        if (AccountValidator.ValidatePasswordAndConfirmPassword(password, oldPassword))
        {
            //ResetPasswordFailed(ResetPasswordFailedCode.PasswordUnequal);
            InstanceRemindPanel.Instance.OpenPanel("新密碼不得與舊密碼相同");
            isResponseReceived = false;
        }
        yield return new WaitUntil(() => isResponseReceived);
    }

    // TODO ResetPassword with PlayFab
    /// <summary>
    /// Coroutine for resetting the account password using PlayFab.
    /// </summary>
    private IEnumerator ResetAccount(string password, string username)
    {
        bool isResponseReceived = false;

        ResetAccountInGameText sendText = new ResetAccountInGameText();
        sendText.username = username;
        sendText.password = password;
        string jsonString = JsonMapper.ToJson(sendText);
        byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/ResetPasswordInGame?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
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
                ResetPasswordFailed(ResetPasswordFailedCode.PasswordIsSameAsOldPassword);
                LoadingManger.Instance.Close_Loading_animator();
            }
            else // Reset Account Success
            {
                InstanceRemindPanel.Instance.OpenPanel("密碼修改完成");
                Debug.Log(webRequest.downloadHandler.text);
                isResponseReceived = true;
            }
        }

        // Wait until a response is received
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
    /// <summary>
    /// check if any input field is empty. If true, make comform change button uninteractable. Will check every time InputField value has changed.
    /// </summary>
    public void CheckInputFieldIsEmpty()
    {
        if(string.IsNullOrEmpty(passwordInputField.text)||
            string.IsNullOrEmpty(oldPasswordInputField.text)||
            string.IsNullOrEmpty(confirmPasswordInputField.text))
        {
            comformChangePasswordButton.interactable = false;
        }
        else
        {
            comformChangePasswordButton.interactable = true;
        }
    }
}
