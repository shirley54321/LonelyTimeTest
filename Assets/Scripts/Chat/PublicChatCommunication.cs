using Microsoft.AspNetCore.SignalR.Client;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Share.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor.VersionControl;

public class PublicChatCommunication : MonoBehaviour
{
    #region Tools

    [TextArea(3, 20)] public string testStr;
    [SerializeField] private SensitiveWordFilter sensitiveWordFilter;
    [SerializeField] private StickerBtnPress stickerBtnPress;
    [SerializeField] private UrlImageGetter urlImageGetter;
    [SerializeField] private TMP_InputField communicationInputField;
    [SerializeField] private GameObject scrollContainer;
    [SerializeField] private GameObject peopleMsg;
    [SerializeField] private GameObject myMsg;

    [SerializeField] private Button stickerButton;
    [SerializeField] private Button sendButton;

    #endregion

    HubConnection connection;
    // Start is called before the first frame update
    async void Start()
    {
        Initialize();
        //StartCoroutine(detect());

        Debug.Log($"connection: {connection.State}");
        StartCoroutine(GetPlayerProfile(PlayerInfoManager.Instance.PlayFabId));
    }

    private void Update()
    {
        if (getMsg)
        {
            StartCoroutine(AddPeopleToContainer(chatMessage1));
        }
    }
    async void Initialize()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("https://chatroomservice.azurewebsites.net/api")
            .WithAutomaticReconnect()
            .Build();

        // When receive the SignalRMessage of the target "OnMessageReceived"
        // the message would be a List of ChatMessage
        connection.On<List<ChatMessage>>("OnMessageReceived", message =>
        {
            ChangeUiRedPoint.ChangeChatRedPoint(true);

            Debug.Log("Received message from server:");
            int i = 0;
            Debug.Log("msg " + i + " :" + message[0].userID + " " + message[0].message + " " + message[0].displayName + " " + message[0].avatarUrl);
            chatMessage1 = message[0];
            print("getMsg: " + getMsg);
            getMsg = true;
            //foreach (ChatMessage c in message)
            //{
            //    string a;
            //    sensitiveWordFilter.StringCheckAndReplace(c.message, out a);
            //    Debug.Log("msg " + i + " :" + c.userID + " " + c.message + " " + a);
            //    i++;
            //}
        });

        await connection.StartAsync();
    }

    bool getMsg = false;
    ChatMessage chatMessage1 = null;
    string myUserName;
    IEnumerator detect()
    {
        while (!getMsg)
        {
            yield return new WaitForSecondsRealtime(1);
        }
        getMsg = false;
        print("chatMessage1.userID: " + chatMessage1.userID);
        print("my playfab id: " + PlayerInfoManager.Instance.PlayFabId);
        if (chatMessage1.userID != PlayerInfoManager.Instance.PlayFabId) //不是自己發的，出現在對面聊天框
        {
            StartCoroutine(AddPeopleToContainer(chatMessage1));
        }
        else
            continueDetect();
    }
    private void continueDetect()
    {
        StartCoroutine(detect());
    }

    DateTime lastSendTime = new DateTime(2020, 1, 1, 0, 0, 0);
    string LastSendMsg = "";

    private bool testLastSendMsg(string sendMsg)
    {
        if ((DateTime.Now - lastSendTime).Seconds <= 3 && sendMsg == LastSendMsg)
            return false;
        else
        {
            LastSendMsg = sendMsg;
            lastSendTime = DateTime.Now;
            return true;
        }
    }

    [ContextMenu("Test Send Public Chat Message")]
    public void TestSendPublicChatMessage()
    {
        if (string.IsNullOrEmpty(communicationInputField.text))
        {
            return;
        }

        if (communicationInputField.text.Length > 140)
        {
            InstanceRemindPanel.Instance.OpenPanel("訊息不可超過140字");
        }
        else if (testLastSendMsg(communicationInputField.text))
        {
            print("senting:" + communicationInputField.text);
            SendPublicChatMessage(communicationInputField.text);
            AddMyMsgToContainer(communicationInputField.text);
        }
        else
        {
            InstanceRemindPanel.Instance.OpenPanel("3秒內不得發送一樣訊息");
        }
        communicationInputField.text = "";
    }

    private void AddMyMsgToContainer(string myMsgText)
    {
        var objList = myMsg.GetComponentsInChildren<TMP_Text>(); //搜索包含這個的component，包含自己，參考https://blog.csdn.net/virus2014/article/details/52964159
        sensitiveWordFilter.StringCheckAndReplace(myMsgText, out myMsgText);
        List<string> strings = new List<string> { $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}", myMsgText };

        for (int i = 0; i < 2; i++)
        {
            objList[i].text = strings[i];
        }
        Instantiate(myMsg, scrollContainer.transform); //生成一個list的prefab
        StartCoroutine(ScrollToBottomCoroutine());
    }

    IEnumerator AddPeopleToContainer(ChatMessage chatMessage)
    {
        getMsg = false;
        print("getMsg2: " + getMsg);
        StartCoroutine(detect()); //繼續檢查get message
        string temp;
        sensitiveWordFilter.StringCheckAndReplace(chatMessage.message, out temp);
        yield return urlImageGetter.GettingSprite(chatMessage.avatarUrl, "avatar");
        List<string> strings1 = new List<string> { chatMessage.displayName, $"{chatMessage.sendTime.Hour:D2}:{chatMessage.sendTime.Minute:D2}" };
        List<string> strings2 = new List<string> { chatMessage.displayName, temp, $"{chatMessage.sendTime.Hour:D2}:{chatMessage.sendTime.Minute:D2}" };

        if (chatMessage.userID != PlayerInfoManager.Instance.PlayFabId)
        {
            var objList = peopleMsg.GetComponentsInChildren<TMP_Text>();
            print("print obj list: ");
            for (int i = 0; i < 3; i++)
            {
                print(objList[i]);
                objList[i].text = strings2[i];
            }

            var objList2 = peopleMsg.GetComponentsInChildren<Image>();
            objList2[0].sprite = urlImageGetter.sprite;
            Instantiate(peopleMsg, scrollContainer.transform); //生成一個list的prefab
            yield return new WaitForEndOfFrame(); // 等待下一幀以確保新消息已添加到容器中
            ScrollToBottom(); // 滾動到最新消息
        }
    }

    IEnumerator GetPlayerProfile(string playFabId)
    {
        bool isResponseReceived = false;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
            }
        },
        result => {
            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            myUserName = result.PlayerProfile.DisplayName;
            isResponseReceived = true;
        },
        error => Debug.LogError("Get player Profile ERROR: " + error.GenerateErrorReport()));

        yield return new WaitUntil(() => isResponseReceived);
    }


    public void SendPublicChatMessage(string message)
    {
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "SendPublicChatMessage",
            FunctionParameter = new Dictionary<string, object>()
            {
                {"message", message},
                {"displayName", myUserName},
                {"type","OnMessageReceived" },
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url}
            },
            GeneratePlayStreamEvent = false
        };
        PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");



                    // Deserialize the CloudScript response into MachineList object
                    //var enterSuccess = Convert.ToBoolean(result.FunctionResult.ToString());
                    var enterSuccess = result.FunctionResult.ToString();
                    Debug.Log(enterSuccess);
                    //if (enterSuccess)
                    //{
                    //}
                    //else
                    //{
                    //}

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });
        //if (message != null)
        //{
        //    string temp;
        //    sensitiveWordFilter.StringCheckAndReplace(message, out temp);
        //    urlImageGetter.GettingSprite(PlayerInfoManager.Instance.PlayerInfo.avatar.Url, "avatar");
        //    List<string> strings2 = new List<string> { myUserName, temp, $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}" };

        //    var objList = myMsg.GetComponentsInChildren<TMP_Text>();
        //    print("objList list: " + objList.Length);
        //    for (int i = 0; i < 2; i++)
        //    {
        //        print(objList[i]);
        //        objList[i].text = strings2[i];
        //    }

        //    var objList2 = myMsg.GetComponentsInChildren<Image>();
        //    objList2[0].sprite = urlImageGetter.sprite;
        //    Instantiate(myMsg, scrollContainer.transform); //生成一個list的prefab
        //}
    }

    class ChatMessage
    {
        public uint recno { get; set; }
        public string userID { get; set; }
        public string message { get; set; }
        public DateTimeOffset sendTime { get; set; }
        public string displayName { get; set; }
        public string avatarUrl { get; set; }

        override public string ToString()
        {
            return $"recno: {recno}\nuserID: {userID}\nmessage: {message}\nsendTime: {sendTime}\ndisplayName: {displayName}\navatarUrl: {avatarUrl}";
        }
    }

    // 滾動到最底部
    private void ScrollToBottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollContainer.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return new WaitForEndOfFrame(); // 等待下一幀以確保新消息已添加到容器中
        ScrollToBottom(); // 滾動到最新消息
    }

    // 點擊系統頻道
    public void OnSystemMessageButtonClick()
    {
        communicationInputField.interactable = false;
        communicationInputField.placeholder.GetComponent<TMP_Text>().text = "無法與系統訊息聊天";
        sendButton.interactable = false;
        stickerButton.interactable = false;
        communicationInputField.text = "";
    }

    // 點擊公共頻道
    public void OnPublicChannelButtonClick()
    {
        communicationInputField.interactable = true;
        communicationInputField.placeholder.GetComponent<TMP_Text>().text = "請輸入文字";
        sendButton.interactable = true;
        stickerButton.interactable = true;
    }

}