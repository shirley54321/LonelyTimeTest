using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserAccount;

public class OpenVerificationProvisional : MonoBehaviour
{
    #region Instance(Singleton)
    private static OpenVerificationProvisional instance;

    public static OpenVerificationProvisional Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<OpenVerificationProvisional>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject with {typeof(OpenVerificationProvisional)} does not exist in the scene, " +
                                   $"yet its method is being called.\n" +
                                   $"Please add {typeof(OpenVerificationProvisional)} to the scene.");
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

    public GameObject Panel;
    public SMSHandler SMSHandler;
    public TMP_InputField InputText;
    public Text CountDownText;
    public Button ReSendButton;

    private string Phone;
    private string UserName;
    private int Time;

    public void OpenPanel()
    {
        Panel.SetActive(true);
        StartCoroutine(TimeCountDown());
    }

    public void ClosePanel()
    {
        Panel.SetActive(false);
        UserAccountManager.Instance.LoginFailed(LoginFailedCode.OtherReason);
    }

    public void TakePhoneAndName(string phone, string userName)
    {
        Phone = phone;
        UserName = userName;
        Debug.Log(userName);
    }

    public void SendSMS()
    {
        StartCoroutine(ReSendSMS());
        StartCoroutine(TimeCountDown());
    }

    IEnumerator ReSendSMS()
    {
        yield return SMSHandler.SendSMS(Phone, UserName);
    }

    public void CheckVerificationCode()
    {
        if (string.IsNullOrEmpty(InputText.text)) return;
        string VerificationCode = InputText.text;
        VerificationCode = VerificationCode.Replace("\n", string.Empty);

        StartCoroutine(TestCheckSMS());
    }

    IEnumerator TestCheckSMS()
    {
        string VerificationCode = InputText.text;
        yield return SMSHandler.ValidateSMS(UserName, VerificationCode);
        if(!SMSHandler.IsValidateSuccessful())
        {
            InstanceRemindPanel.Instance.OpenPanel("驗證碼輸入錯誤");
        }
    }
    IEnumerator TimeCountDown()
    {
        Time = 60;
        CountDownText.text = $"重新發送({Time}秒)";
        ReSendButton.interactable = false;

        while (Time>0)
        {
            yield return new WaitForSecondsRealtime(1);
            Time--;
            CountDownText.text = $"重新發送({Time}秒)";
        }

        yield return new WaitForSecondsRealtime(1);
        CountDownText.text = "發送手機驗證碼";
        ReSendButton.interactable = true;
    }
}
