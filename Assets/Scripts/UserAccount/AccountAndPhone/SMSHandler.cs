using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using System.Net;
using System;
using UnityEngine.Networking;
using LitJson;
using UnityEngine.Events;
using PlayFab;
using PlayFab.ClientModels;
using System.Text.RegularExpressions;
using Share.Tool;

namespace UserAccount
{
    [SerializeField]
    public class SendSMSText
    {
        public string userName;

        public string phone;
    }

    [SerializeField]
    public class ValidateSMSText
    {
        public string userName;

        public string verifyCode;
    }
    /// <summary>
    /// Handles sending and validating SMS messages.
    /// </summary>
    public class SMSHandler : MonoBehaviour
    {
        [SerializeField] private string _phoneNumber;
        [SerializeField] private string _userName;
        [SerializeField] private bool _validateSuccessful;
        [SerializeField] private bool _sendSMSSuccessful;
        [SerializeField] private ValidateSMSFailedCode _validateSmsFailedCode;

        /// <summary>
        /// Sends an SMS message to the specified phoneNumber number.
        /// </summary>
        /// <param name="phoneNumber">Phone number to send the SMS to.</param>
        public IEnumerator SendSMS(string phone, string userName)
        {
            _phoneNumber = phone;
            _userName = userName;
            Debug.Log($"Send SMS with {phone}");
            _sendSMSSuccessful = false;
            bool isResponseReceived = false;
            SendSMSText sendText = new SendSMSText();
            sendText.userName = userName;
            sendText.phone = phone;
            string jsonString = JsonMapper.ToJson(sendText);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/SendSMS?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
            {
                //wait result
                //webRequest.SetRequestHeader("ContentType", "application/json;charset=utf-8");
                webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    if (webRequest.downloadHandler.text == "SendSMSSuccessed")
                    {
                        _sendSMSSuccessful = true;
                    }
                    else if(webRequest.downloadHandler.text == "SendSMSToManyTimes")
                    {
                        InstanceRemindPanel.Instance.OpenPanel("驗證碼因請求次數過多而鎖定，請一個小時後再使用或聯絡客服協助處理");
                    }
                    else
                    {
                        Debug.Log(webRequest.downloadHandler.text);
                    }
                }
                isResponseReceived = true;
            }
            yield return new WaitUntil(() => isResponseReceived);
        }


        /// <summary>
        /// Validates an SMS message asynchronously.
        /// </summary>
        /// <param name="verificationCode">The SMS message to validate.</param>
        public IEnumerator ValidateSMS(string userName, string verificationCode)
        {
            bool isResponseReceived = false;

            // TODO: Implement SMS validation logic
            _validateSuccessful = false; // Simulating successful validation
            isResponseReceived = false;
            ValidateSMSText sendText = new ValidateSMSText();
            sendText.userName = userName;
            sendText.verifyCode = verificationCode;
            string jsonString = JsonMapper.ToJson(sendText);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/VerifySMSCode?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
            {
                //wait result
                webRequest.SetRequestHeader("ContentType", "application/json;charset=utf-8");
                webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.Log(webRequest.error);
                    isResponseReceived = true;
                }
                else
                {
                    if (webRequest.downloadHandler.text == "1")
                    {
                        _validateSuccessful = true;
                        Debug.Log("SMS_ValidateSuccessful");
                    }
                    else if (webRequest.downloadHandler.text == "SMSError")
                    {
                        InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                    }
                    else if (webRequest.downloadHandler.text == "SendSMSToManyTimes")
                    {
                        InstanceRemindPanel.Instance.OpenPanel("驗證碼因請求次數過多而鎖定，請一個小時後再使用或聯絡客服協助處理");
                    }
                    isResponseReceived = true;
                }
            }

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        
        /// <summary>
        /// Gets the success status of the send SMS.
        /// </summary>
        /// <returns>True if SMS was sent successful, otherwise false.</returns>
        public bool IsSendSMSSuccessful()
        {
            return _sendSMSSuccessful;
        }
        /// <summary>
        /// Gets the success status of the SMS validation.
        /// </summary>
        /// <returns>True if validation was successful, otherwise false.</returns>
        public bool IsValidateSuccessful()
        {
            return _validateSuccessful;
        }

        /// <summary>
        /// Gets the failure code in case of unsuccessful SMS validation.
        /// </summary>
        /// <returns>The failure code indicating the reason for validation failure.</returns>
        public ValidateSMSFailedCode GetFailedCode()
        {
            return _validateSmsFailedCode;
        }
    }

    
}
