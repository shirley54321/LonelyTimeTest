using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hamburger_Panel_setting : MonoBehaviour
{
    public GameObject Music;
    public GameObject Sound;
    public GameObject Notifications;
    public GameObject Chat_Pop_up;
    public GameObject Grand_Prize_Sharing;
    public GameObject Grand_Prize_Push_Notification;
    public GameObject Level_Up_Push_Notification;
    public Button ChangePasswordButton;

    private void Start()
    {
        // 设置按钮的初始位置
        SetButtonPosition(Music, GameSettings.instance.Music);
        SetButtonPosition(Sound, GameSettings.instance.Sound);
        SetButtonPosition(Notifications, GameSettings.instance.Notifications);
        SetButtonPosition(Chat_Pop_up, GameSettings.instance.Chat_Pop_up);
        SetButtonPosition(Grand_Prize_Sharing, GameSettings.instance.Grand_Prize_Sharing);
        SetButtonPosition(Grand_Prize_Push_Notification, GameSettings.instance.Grand_Prize_Push_Notification);
        SetButtonPosition(Level_Up_Push_Notification, GameSettings.instance.Level_Up_Push_Notification);
        //設置是否能修改密碼
        SetChangePasswordButtonInteractable();
    }
    private void SetButtonPosition(GameObject button, bool settingValue)
    {
        if(settingValue)
        {
            button.transform.GetChild(2).gameObject.SetActive(true);
            button.transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            button.transform.GetChild(2).gameObject.SetActive(false);
            button.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    public void OnButtonClick(string button)
    {
        if (button == "Music"){
            setThePOSX(Music,"Music");
        }
        else if(button == "Sound"){
            setThePOSX(Sound,"Sound");
        }
        else if(button == "Notifications"){
            setThePOSX(Notifications,"Notifications");
        }
        else if(button == "Chat_Pop_up"){
            setThePOSX(Chat_Pop_up,"Chat_Pop_up");
        }
        else if(button == "Grand_Prize_Sharing"){
            setThePOSX(Grand_Prize_Sharing,"Grand_Prize_Sharing");
        }
        else if(button == "Grand_Prize_Push_Notification"){
            setThePOSX(Grand_Prize_Push_Notification,"Grand_Prize_Push_Notification");
        }
        else if(button == "Level_Up_Push_Notification"){
            setThePOSX(Level_Up_Push_Notification,"Level_Up_Push_Notification");
        }        
    }
    private void setThePOSX(GameObject Tmp,string settingName)
    {
        bool settingValue ;
        if(!Tmp.transform.GetChild(2).gameObject.activeSelf){
            settingValue=true;
            Tmp.transform.GetChild(2).gameObject.SetActive(true);
            Tmp.transform.GetChild(3).gameObject.SetActive(false);
        }
        else{
            settingValue=false;
            Tmp.transform.GetChild(2).gameObject.SetActive(false);
            Tmp.transform.GetChild(3).gameObject.SetActive(true);
        }
        Debug.Log(settingName + " " + settingValue);
        UpdateSettingAndSave(settingName, settingValue);
    }
    private void UpdateSettingAndSave(string settingName, bool settingValue)
    {
        // Debug.Log("settingName"+settingName);
        // Debug.Log("settingValue"+settingValue);
        switch (settingName)
        {
            case "Music":
                GameSettings.instance.Music = settingValue ;
                break;
            case "Sound":
                GameSettings.instance.Sound = settingValue ;
                break;
            case "Notifications":
                GameSettings.instance.Notifications = settingValue ;
                break;
            case "Chat_Pop_up":
                GameSettings.instance.Chat_Pop_up = settingValue ;
                break;
            case "Grand_Prize_Sharing":
                GameSettings.instance.Grand_Prize_Sharing = settingValue ;
                break;
            case "Grand_Prize_Push_Notification":
                GameSettings.instance.Grand_Prize_Push_Notification = settingValue ;
                break;
            case "Level_Up_Push_Notification":
                GameSettings.instance.Level_Up_Push_Notification = settingValue ;
                break;
        }
        GameSettings.instance.Save(settingName);
    }
    /// <summary>
    /// 在不使用playfab帳號(使用第三方登入或遊客登入)登入時，因為沒有密碼，關閉修改密碼的按鈕
    /// </summary>
    private void SetChangePasswordButtonInteractable()
    {
        var request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request,
            result =>
            {
                if (!string.IsNullOrEmpty(result.AccountInfo.Username))
                {
                    ChangePasswordButton.interactable = true;
                }
                else
                {
                    ChangePasswordButton.interactable = false;
                    Debug.Log("not playfab account");
                }
            },
            error =>
            {
                Debug.LogError("can't get account info ： "+error);
                ChangePasswordButton.interactable = false;
            });
    }
}
