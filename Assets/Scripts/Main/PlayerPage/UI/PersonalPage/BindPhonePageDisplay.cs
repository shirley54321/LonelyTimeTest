using LitJson;
using Loading;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UserAccount;
namespace Main.PlayerPage.UI.BindPhonePage
{
    [SerializeField]
    public class BindPhoneText
    {
        public string userName;
        public string userID;

        public string verifyCode;

        public string phone;
    }
    public class BindPhonePageDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private SMSHandler smsHandler;
        [SerializeField] private TMP_Text userAccount;
        [SerializeField]
        private TMP_InputField codeInputField, phoneInputField;
        [SerializeField] private string _phoneNumber;
        [SerializeField] private string _userName;

        public Text CountDownText;
        public Button ReSendButton;

        private int Time = 0;

        public UnityEvent OnValidatAccountInfoSuccessful = new UnityEvent();

        private void OnEnable()
        {
            OnValidatAccountInfoSuccessful.AddListener(OnValidateSuccessful);

            if (PlayerInfoManager.Instance.IsAccountBound())
            {
                _userName = "帳號 : " + PlayerInfoManager.Instance.AccountInfo.Username;
            }
            else
            {
                _userName = "會員ID : " + PlayerInfoManager.Instance.PlayerInfo.userProfile.UID;
            }

            userAccount.text = _userName;
        }



        /// <summary>
        /// Unsubscribe from events when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            OnValidatAccountInfoSuccessful.RemoveListener(OnValidateSuccessful);
        }

        #region Show/Close Panel

        public void ShowPanel()
        {


            InstanceRemindPanel.Instance.OpenPanel("敬請期待");
            return;
            codeInputField.text = "";
            phoneInputField.text = "";
            panel.SetActive(true);
            if (Time != 0)
            {
                StartCoroutine(TimeCountDown());
            }
        }

        public void ClosePanel()
        {
            panel.SetActive(false);
        }
        #endregion

        /// 按下發送驗證碼按鈕
        /// </summary>
        public async void SendSMS()
        {
            if(phoneInputField.text.Length == 10)
            {
                _phoneNumber = phoneInputField.text;
                if (Time == 0)
                {
                    StartCoroutine(ReSendSMS());
                }
            }
            else
            {
                InstanceRemindPanel.Instance.OpenPanel("手機格式有誤，請您重新輸入");
            }
        }

        /*public async Task TestSendSMS(string phone, string userName)
        {
            _phoneNumber = phone;
            _userName = userName;
            Debug.Log($"Send SMS with {phone}");
            bool _sendSMSSuccessful = false;
            bool isResponseReceived = false;
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes("{ \"userName\": \"" + userName + "\",\"phone\": \"" + phone + "\"}");
            UnityWebRequest request = UnityWebRequest.PostWwwForm("https://playeronlinestatus.azurewebsites.net/api/SendSMSFunction?code=WMLAjJZ5f2zVMH1tnbaGBhZD4NPGe-HikTxAdOlduJj8AzFuzmMO_Q==","");
            request.uploadHandler = new UploadHandlerRaw(utf8Bytes);
            await Task.Yield();
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                if (request.downloadHandler.text == "SendSMSSuccessed")
                {
                    _sendSMSSuccessful = true;
                }
                else if (request.downloadHandler.text == "SendSMSToManyTimes")
                {
                    InstanceRemindPanel.Instance.OpenPanel("驗證碼因請求次數過多而鎖定，請一個小時後再使用或聯絡客服協助處理");
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                }
            }
            isResponseReceived = true;
        }*/

        IEnumerator ReSendSMS()
        {
            LoadingManger.Instance.Open_Loading_animator();
            yield return smsHandler.SendSMS(_phoneNumber, _userName);
            if (smsHandler.IsSendSMSSuccessful())
            {
                Time = 60;
                StartCoroutine(TimeCountDown());
            }
            LoadingManger.Instance.Close_Loading_animator();
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
        /// 按下確認按鈕
        /// </summary>
        public void BindPhoneNumber()
        {
            if (phoneInputField.text.Length == 10)
            {
                Debug.Log(codeInputField.text);
                _phoneNumber = phoneInputField.text;
                StartCoroutine(BindPhone(_phoneNumber, _userName, codeInputField.text));
            }
            else
            {
                InstanceRemindPanel.Instance.OpenPanel("手機格式有誤，請您重新輸入");
            }
        }

        private void OnValidateSuccessful()
        {
            // Open the change password panel and close the forget password panel.
            //changePasswordUI.OpenPanel();
            codeInputField.text = ""; 
            phoneInputField.text = "";
            ClosePanel();
        }


        private IEnumerator BindPhone(string phone, string userName, string verifyCode)
        {
            bool isResponseReceived = false;
            string useriD = PlayerInfoManager.Instance.PlayFabId;
            Debug.Log(useriD);

            BindPhoneText sendText = new BindPhoneText
            {
                userName = userName,
                userID = useriD,
                verifyCode = verifyCode,
                phone = phone
            };

            string jsonString = JsonMapper.ToJson(sendText);
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

            using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/BindPhone?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==", ""))
            //using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/BindPhone?code=UnbORuNPDIiEfuACfXgyObXFv3prVCAL6CPV3HPjyB50AzFu2DITMw%3D%3D", ""))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
                webRequest.SetRequestHeader("Content-Type", "application/json");

                // 發送請求
                yield return webRequest.SendWebRequest();

                // 檢查是否有網絡錯誤或HTTP錯誤
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    Debug.LogError("Network or HTTP Error: " + webRequest.error);
                }
                else
                {
                    Debug.Log("Response: " + webRequest.downloadHandler.text);

                    if (webRequest.downloadHandler.text == "successed")
                    {
                        OnValidatAccountInfoSuccessful.Invoke();
                        InstanceRemindPanel.Instance.OpenPanel("綁定成功");

                        var request1 = new GetUserDataRequest
                        {
                            Keys = new List<string> { "PhoneNumber" }
                        };

                        PlayFabClientAPI.GetUserData(request1, result =>
                        {
                            if (!result.Data.TryGetValue("PhoneNumber", out UserDataRecord PhoneNumber_data))
                            {
                                var request2 = new UpdateUserDataRequest
                                {
                                    Data = new Dictionary<string, string>
                            {
                                { "PhoneNumber", phone }
                            }
                                };

                                PlayFab.PlayFabClientAPI.UpdateUserData(request2, result =>
                                {
                                    Debug.Log("UpdateUserData: PhoneNumber OK");
                                },
                                error =>
                                {
                                    Debug.LogError("UpdateUserData: PhoneNumber Error");
                                });
                            }
                        },
                        error =>
                        {
                            Debug.LogError("GetUserData: PhoneNumber Error");
                        });
                    }
                    else if (webRequest.downloadHandler.text == "verifyfailed")
                    {
                        InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
                    }
                    else
                    {
                        InstanceRemindPanel.Instance.OpenPanel("該手機號碼已綁定其他遊戲資料，請使用其他手機號碼進行綁定");
                        Debug.Log("該手機號碼已綁定其他遊戲資料");
                    }

                    isResponseReceived = true;
                }
            }

            // 等待直到收到回應
            yield return new WaitUntil(() => isResponseReceived);
        }



        //private IEnumerator BindPhone(string phone, string userName, string verifyCode)
        //{
        //    bool isResponseReceived = false;

        //    BindPhoneText sendText = new BindPhoneText();
        //    sendText.userName = userName;
        //    sendText.verifyCode = verifyCode;
        //    sendText.phone = phone;
        //    string jsonString = JsonMapper.ToJson(sendText);
        //    byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        //    using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm("https://forgetpassword.azurewebsites.net/api/BindPhone?code=o5LFrAEL_dszigdDte7OOvgzW0iN1La4xtIXMzvxtJu3AzFu6yuaLQ==",""))
        //    {
        //        //wait result
        //        //webRequest.SetRequestHeader("ContentType", "application/json;charset=utf-8");
        //        webRequest.uploadHandler = new UploadHandlerRaw(utf8Bytes);
        //        yield return webRequest.SendWebRequest();

        //        // TODO: Implement PlayFab password reset logic
        //        if (webRequest.isNetworkError || webRequest.isHttpError)
        //        {
        //            // TODO: Check the reason for password reset failure
        //            Debug.Log(webRequest.error);
        //        }
        //        else // Reset Account Success
        //        {
        //            Debug.Log(webRequest.downloadHandler.text);
        //            if(webRequest.downloadHandler.text== "successed")
        //            {
        //                OnValidatAccountInfoSuccessful.Invoke();
        //                InstanceRemindPanel.Instance.OpenPanel("綁定成功");
        //                var request1 = new GetUserDataRequest
        //                {
        //                    Keys = new List<string> { "PhoneNumber"} 
        //                };

        //                PlayFabClientAPI.GetUserData(request1, result =>
        //                {
        //                    if (!result.Data.TryGetValue("PhoneNumber", out UserDataRecord PhoneNumber_data))
        //                    {
        //                        var request2 = new UpdateUserDataRequest
        //                        {
        //                            Data = new Dictionary<string, string>
        //                            {
        //                                { "PhoneNumber", phone },
        //                            }
        //                        };
        //                        PlayFab.PlayFabClientAPI.UpdateUserData(request2, result =>
        //                        {
        //                            Debug.Log("UpdateUserData : PhoneNumber OK");
        //                        },
        //                        error =>
        //                        {
        //                            Debug.LogError("UpdateUserData : PhoneNumber Error");
        //                        }
        //                        );
        //                    }
        //                },
        //                error =>
        //                {
        //                    Debug.LogError("GetUserData : PhoneNumber Error");
        //                }
        //                );
        //            }
        //            else if(webRequest.downloadHandler.text == "verifyfailed")
        //            {
        //                InstanceRemindPanel.Instance.OpenPanel("驗證碼錯誤");
        //            }
        //            else
        //            {
        //                InstanceRemindPanel.Instance.OpenPanel("該手機號碼已綁定其他遊戲資料，請使用其他手機號碼進行綁定");
        //                Debug.Log("該手機號碼已綁定其他遊戲資料");

        //            }
        //            isResponseReceived = true;
        //        }
        //    }

        //    // Wait until a response is received
        //    yield return new WaitUntil(() => isResponseReceived);
        //}
    }
}

