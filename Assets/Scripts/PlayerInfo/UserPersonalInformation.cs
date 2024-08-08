using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SystemMessageManager;
using Shared.RemindPanel;
using System.Text.RegularExpressions;
using System;
using System.Net;
using Share.Tool;
using Player;

public class UserPersonalInformation : MonoBehaviour
{
    [SerializeField] InputField nameInputField;
    [SerializeField] InputField birthdayInputField;
    [SerializeField] InputField phoneInputField;
    [SerializeField] InputField personalIDInputField;
    [SerializeField] InputField addressInputField;
    [SerializeField] InputField emailInputField;
    [SerializeField] Button conformButton;
    /// <summary>
    /// 在playfab上的玩家資料格式
    /// </summary>
    class UserPersonalInformationData
    {
        public string name;
        public string birthday;
        public string phone;
        public string personalID;
        public string address;
        public string email;
        public UserPersonalInformationData(string nameText,string birthdayText,string phoneText,string personalIDText,string addressText,string emailText)
        {
            name = nameText;
            DateTime date;
            if (DateTime.TryParseExact(birthdayText, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                birthday = date.ToString("yyyy/MM/dd");
            }
            else if (DateTime.TryParseExact(birthdayText, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                birthday = birthdayText;
            }
            else
            {
                birthday = "invalid";
            }
            phone = phoneText;
            personalID = personalIDText;
            address = addressText;
            email = emailText;
        }
        public bool NameValid()
        {
            if(name.Length<2||name.Length>6)
                return false;
            return !SensitiveWordTool.Instance.StringCheckAndReplace(name, out string clearName);
        }
        public bool BirthdayValid()
        {
            if(birthday== "invalid")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool IDValid()
        {
            return Regex.IsMatch(personalID, @"^[A-Z]\d{9}$");
        }
        public bool AddressValid()
        {
            //列舉 https://www.ndc.gov.tw/nc_77_4402中的縣市
            string cityNamesRegex = "^(?:台北市|臺北市|新北市|基隆市|新竹市|桃園市|新竹縣|宜蘭縣" +
                                        "|台中市|臺中市|苗栗縣|彰化縣|南投縣|雲林縣" +
                                        "|台南市|臺南市|高雄市|嘉義市|嘉義縣|屏東縣|澎湖縣" +
                                        "|花蓮縣|臺東縣|金門縣|連江縣)";
            return Regex.IsMatch(address, cityNamesRegex);
        }
        public bool EmailValid()
        {
            return Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@([a-zA-Z]+\.)+[a-zA-Z]+$");
        }
        public bool PhoneVaild()
        {
            return Regex.IsMatch(phone, @"^[0-9]{10}$");
        }
    }
    private void OnEnable()
    {
        ShowUserPersonalInformation();
    }
    private void OnDisable()
    {
    }
    /// <summary>
    /// 按下按鈕時觸發，嘗試上傳資料
    /// </summary>
    public void OnConfirmButtonClick()
    {
        UpdateUserPersonalInformation();
    }
    /// <summary>
    /// 驗證資料格式，並在通過時更新至伺服器
    /// </summary>
    void UpdateUserPersonalInformation()
    {
        UserPersonalInformationData data = new UserPersonalInformationData(nameInputField.text, birthdayInputField.text, phoneInputField.text, personalIDInputField.text, addressInputField.text, emailInputField.text);
        if (data.name == "" || data.birthday == "" || data.phone == "" || data.personalID == "" || data.address == "" || data.email == "")
        {
            Debug.Log("有未填寫的資料");
            InstanceRemindPanel.Instance.OpenPanel("有未填寫的資料");
            return;
        }
        else if(!data.IDValid())
        {
            Debug.Log("身分證格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("身分證格式錯誤");
            return;
        }
        else if (!data.BirthdayValid())
        {
            Debug.Log("生日格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("生日格式錯誤");
            return;
        }
        else if(!data.EmailValid())
        {
            Debug.Log("電子郵件格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("電子郵件格式錯誤");
            return;
        }
        else if(!data.AddressValid())
        {
            Debug.Log("地址格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("地址格式錯誤");
            return;
        }
        else if (!data.NameValid())
        {
            Debug.Log("姓名格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("姓名格式錯誤");
            return;
        }
        else if (!data.PhoneVaild())
        {
            Debug.Log("電話格式錯誤");
            InstanceRemindPanel.Instance.OpenPanel("電話格式錯誤");
            return;
        }
        string updatedData = JsonUtility.ToJson(data);
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string> { { "UserPersonalInformation", updatedData } }
        },
        result =>
        {
            Debug.Log("Successfully updated user data");
            InstanceRemindPanel.Instance.OpenPanel("成功更新會員資料");
            ShowUserPersonalInformation();
        },
        error =>
        {
            Debug.Log("Got error to update " + "UserPersonalInformation" + "data");
            InstanceRemindPanel.Instance.OpenPanel("更新會員資料失敗，請稍後再試");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    /// <summary>
    /// 開啟面板時，從伺服器上抓玩家資料並更新顯示
    /// </summary>
    void ShowUserPersonalInformation()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = new List<string> { "UserPersonalInformation" }
        }, OnTitleDataSuccess, OnTitleDataFailure);
    }
    /// <summary>
    /// 抓到資料時，更新面板文字
    /// </summary>
    /// <param name="result"></param>
    private void OnTitleDataSuccess(GetUserDataResult result)
    {   // 成功獲取訊息
        if (result.Data == null || !result.Data.ContainsKey("UserPersonalInformation"))
        {   // 但訊息為空
            Debug.Log("沒有玩家資料");
            CheckButtonInteractable();
            return;
        }

        string systemMessagesJson = result.Data["UserPersonalInformation"].Value;
        UserPersonalInformationData systemMessages = JsonUtility.FromJson<UserPersonalInformationData>(systemMessagesJson);
        nameInputField.text=systemMessages.name;
        birthdayInputField.text=systemMessages.birthday;

        phoneInputField.text=systemMessages.phone.Substring(0, 4)+ new string('*', 6);

        personalIDInputField.text= systemMessages.personalID.Substring(0, 3) + new string('*', 7);
        addressInputField.text=systemMessages.address.Substring(0, 3)+new string('*', systemMessages.address.Length-3);
        emailInputField.text=systemMessages.email;
        CheckButtonInteractable();
    }
    /// <summary>
    /// 沒抓到資料，報LOG
    /// </summary>
    /// <param name="error"></param>
    private void OnTitleDataFailure(PlayFabError error)
    {   // 獲取訊息失敗
        Debug.LogError("與playfab的訊息取得發生問題: " + error.GenerateErrorReport());
    }
    /// <summary>
    /// 在InputField更新時檢查，若全部都為空，禁止按確認紐
    /// </summary>
    public void CheckButtonInteractable()
    {
        if (nameInputField.text == ""
                && birthdayInputField.text == ""
                && phoneInputField.text == ""
                && personalIDInputField.text == ""
                && addressInputField.text == ""
                && emailInputField.text == "")
        {
            conformButton.interactable = false;
        }
        else
        {
            conformButton.interactable = true;
        }
    }
    /// <summary>
    /// 在生日InputField更新時，若內容為八個數字，矯正格式
    /// </summary>
    public void AutoCorrectionBirthDay()
    {
        if (birthdayInputField.text.Length == 8 && Regex.IsMatch(birthdayInputField.text, @"^[0-9]{8}$")) 
        {
            birthdayInputField.text = birthdayInputField.text.Substring(0, 4) + "/" + birthdayInputField.text.Substring(4, 2) + "/" + birthdayInputField.text.Substring(6, 2);
        }

    }
}
