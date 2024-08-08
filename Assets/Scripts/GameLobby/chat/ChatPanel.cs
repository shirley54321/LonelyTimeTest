using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPanel : BasePanel
{
    public TextMeshProUGUI PlaceholderText;
    [SerializeField] GameObject systemMessagePrefab;
    [SerializeField] GameObject sysetmMessagePanelContent;
    [SerializeField] Button SendButton;
    [SerializeField] Button StickerButton;
    public GameObject FriendObject;
    private void Start()
    {
        TakeSystemInfo();
    }
    public void ClosePanel()
    {
        UIManager.Instance.ClosePanel(UIConst.ChatPanel);
    }
    
    public void ChangeSystemInputText(bool isOn)
    {
            if(isOn)
            {
                PlaceholderText.text = "無法與系統訊息對談";
                PlaceholderText.transform.parent.parent.GetComponent<TMP_InputField>().interactable = false;
                SendButton.interactable = false;
                StickerButton.interactable = false;
            }
    }
    public void ChangePublicInputText(bool isOn)
    {
        if (isOn)
        {
            PlaceholderText.text = "請輸入文字";
            PlaceholderText.transform.parent.parent.GetComponent<TMP_InputField>().interactable = true;
            SendButton.interactable = true;
            StickerButton.interactable = true;
        }
    }

    void TakeSystemInfo()
    {
        SystemMessageManager.SystemMessageList systemMessages = SystemMessageManager.Instance.systemMessages;
        if (systemMessages.messages == null) return;

        foreach(var message in systemMessages.messages)
        {
            var messagebox = Instantiate(systemMessagePrefab,sysetmMessagePanelContent.transform);
            messagebox.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = message.text;
            DateTime Time = Convert.ToDateTime(message.Time);
            string DisplayTime;
            if(Time.Hour>12)
            {
                DisplayTime = $"PM {Time.Hour-12}:{Time.Minute}";
            }
            else
            {
                DisplayTime = $"AM {Time.Hour - 12}:{Time.Minute}";
            }
            messagebox.transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = DisplayTime;
        }
    }
    private void OnApplicationQuit()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"SystemMessage", null},
        }
        },
     result => Debug.Log("Successfully updated user data"),
     error => {
         Debug.Log("Got error setting user data Ancestor to Arthur");
         Debug.Log(error.GenerateErrorReport());
     });
    }
}
