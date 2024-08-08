using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using PlayFab.CloudScriptModels;
using PlayFab;
using Share.Tool;
using System;
using Player;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using Unity.Burst;
using static ChatManager;
using Loading;
public class ChatManager : MonoBehaviour
{
    #region Tools

    [TextArea(3, 20)] public string testStr;
    [SerializeField] private SensitiveWordFilter sensitiveWordFilter;
    [SerializeField] private StickerBtnPress stickerBtnPress;
    [SerializeField] private ChatSprite chatSprite;
    [SerializeField] private UrlImageGetter urlImageGetter;
    [SerializeField] private GameObject peopleMsg;
    [SerializeField] private GameObject scrollContainer;
    [SerializeField] private GameObject myMsg;

    public ChangeUiRedPoint _changeUiRedPoint;
    public string friendPlayFabID;
    public string friendDisplayName;
    public Sprite AvatorWantTobe;
    string privateChat;


    #endregion

    HubConnection connection;
    string myUserName;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
  
        SendPublicChatMessage("");//更新connectionID

        Debug.Log($"connection: {connection.State}");
        StartCoroutine(GetPlayerProfile(PlayerInfoManager.Instance.PlayFabId)); //這個可以不用
    }

    private void Update()
    {
        if (getMsg && canDoSpawnMsgOBJ)
        {
            if(InAddPeopleToContainer == false) StartCoroutine(AddPeopleToContainer(chatMessage1));
        }
    }

    async void Initialize()
    {
        
        connection = new HubConnectionBuilder()
            .WithUrl("https://chatroomservice.azurewebsites.net/api"
            
            )
            .WithAutomaticReconnect()
            .Build();

        connection.On<List<ChatMessage>>("OnPrivateMessageReceived", message =>
        {
            ChangeUiRedPoint.ChangeChatRedPoint(true);
            Debug.Log("Received message from server:");
            chatMessage1 = message[0];
            getMsg = true;
            if (message[0].avatarUrl != "FreshMessage")
            {
                for (int i = message.Count - 1; i >= 0; i--)
                {
                    Debug.Log("收到信息 :" + message[i].message + ",ID:" + message[i].userID);
                    bool hasSameInChatList = false;



                    if (hasSameInChatList == false)
                    {
                        ChatDetail chatDetail = new ChatDetail();

                        chatDetail.message = message[i].message;
                        chatDetail.fromID = message[i].userID;

                        chatDetail.sendTime = message[i].sendTime;
                        chatDetail.avatarUrl = message[i].avatarUrl;
                        chatDetail.TimeText = message[i].sendTime + "";
                        chatDetail.targerID = message[i].friendID;
                        //  chatDetail.targerID =message[i].
                        if (message[i].userID == PlayerInfoManager.Instance.PlayFabId || message[i].userID == friendPlayFabID)
                        {
                            ChatList.Add(chatDetail);
                        }
                        //把信息加入ChatList內
                    }


                }
            }

            else
            {
                //DOREFRESH
                if(friendPlayFabID == message[0].friendID)
                {

                    RefreshPrivateMessage();
                    //可能會需要做成刷新排隊 避免連續刷新的問題

                }
            }

           
        });
        
        await connection.StartAsync();

    }



    public bool RefreshStart = false;

    public void RefreshPrivateMessage()
    {
        RefreshStart = true;
      //  SendPublicChatMessage("");
        Debug.Log("friendPlayFabID :" + friendPlayFabID + "搜尋私訊");
        ClearChatScroll();
        canDoSpawnMsgOBJ = false;
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "SendPublicChatMessage",
            FunctionParameter = new Dictionary<string, object>()//3FE9DB6C107ED4AF//EFEEFC8F72706E32
            {

 
                {"privateMessage", ""},//message//privateMessage
                {"displayName", myUserName}, //發信人
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},//自己的頭貼
                {"type" , "privateUpate"},//private
                {"userConnectionId",connection.ConnectionId },//3FE9DB6C107ED4AF
                //{"playFabID123",},
                {"friendID",friendPlayFabID }//收信人

            },
            GeneratePlayStreamEvent = false,

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

                    var enterSuccess = result.FunctionResult.ToString();
                    Debug.Log(enterSuccess);
                    canDoSpawnMsgOBJ = true;
                    LoadingManger.Instance.Close_Loading_animator();
                    RefreshStart = false;

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });


    }
    bool canDoSpawnMsgOBJ = false;

    [ContextMenu("Test Send Public Chat Message")]
    public void TestSendPublicChatMessage()
    {
        SendPublicChatMessage(testStr);
    }
    public void SendPublicChatMessage(string message)
    {
        Debug.Log("" + PlayFabSettings.staticPlayer.EntityId.ToString());
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "SendPublicChatMessage",
            FunctionParameter = new Dictionary<string, object>()//3FE9DB6C107ED4AF//EFEEFC8F72706E32
            {
                {"privateMessage", message},//message//privateMessage
                {"displayName", myUserName}, //發信人
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},//自己的頭貼
                {"type","private" },//private
                {"userConnectionId",connection.ConnectionId },//3FE9DB6C107ED4AF
                //{"playFabID123",},
                {"friendID",friendPlayFabID }//收信人

            },
            GeneratePlayStreamEvent = false,
            
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
                    RefreshPrivateMessage();
                    var enterSuccess = result.FunctionResult.ToString();
                    Debug.Log(enterSuccess);

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });

        if(message != null)
        {
            string temp;
            sensitiveWordFilter.StringCheckAndReplace(message, out temp);
            urlImageGetter.GettingSprite(PlayerInfoManager.Instance.PlayerInfo.avatar.Url, "avatar");
            List<string> strings1 = new List<string> { myUserName, $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}" };
            List<string> strings2 = new List<string> { myUserName, temp, $"{DateTime.Now.Hour:D2}:{DateTime.Now.Minute:D2}" };
            
            stickerBtnPress.Sticker(temp, strings1, urlImageGetter.sprite, true);

           
        }

       
    }

    




    bool getMsg = false;
    ChatMessage chatMessage1 = null;

    private void ChangeChatRedPoint()
    {
        ChangeUiRedPoint.ChangeChatRedPoint(true);

    }
   
    public void ClearChatScroll()
    {
        LoadingManger.Instance.Open_Loading_animator();
        ChatList.Clear();
        foreach (Transform child in scrollContainer.transform)
        {

            Destroy(child.gameObject);
        }

    }

    public List<ChatDetail> ChatList;
    bool InAddPeopleToContainer = false;
    IEnumerator AddPeopleToContainer(ChatMessage chatMessage)
    {
        InAddPeopleToContainer = true;
        canDoSpawnMsgOBJ = false;


        //先判斷是否有新的信息
        Debug.Log("進入AddPeopleToContainer ");

        int n = ChatList.Count;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (ChatList[j].sendTime > ChatList[j + 1].sendTime)
                {
                    // 交换位置
                    var temp = ChatList[j];
                    ChatList[j] = ChatList[j + 1];
                    ChatList[j + 1] = temp;
                    Debug.Log("泡沫排序");
                }
            }
        }

        getMsg = false;

        for (int a = 0; a < ChatList.Count; a++)
        {


            chatMessage.message = ChatList[a].message;
            chatMessage.userID = ChatList[a].fromID;
            chatMessage.sendTime = ChatList[a].sendTime;
            chatMessage.friendID = ChatList[a].targerID;

            bool hasSame = false;
            foreach (Transform child in scrollContainer.transform)
            {
                // 尝试获取子物件上挂载的ScriptA组件
                if (child.GetComponent<TextObject>().Msg == null || child.GetComponent<TextObject>().NameText == null || child.GetComponent<TextObject>().TimeText == null) continue;


                Debug.Log(" child.GetComponent<TextObject>().Msg.GetComponent<TMP_Text>().text :" + child.GetComponent<TextObject>().Msg.GetComponent<TMP_Text>().text);
                Debug.Log("chatMessage.message :" + chatMessage.message);
                Debug.Log("child.GetComponent<TextObject>().NameText.GetComponent<TMP_Text>().text :" + child.GetComponent<TextObject>().NameText.GetComponent<TMP_Text>().text);
                Debug.Log("chatMessage.displayName :" + chatMessage.displayName);
                if(  child.GetComponent<TextObject>().Msg.GetComponent<TMP_Text>().text == chatMessage.message && child.GetComponent<TextObject>().NameText.GetComponent<TMP_Text>().text == chatMessage.displayName )
                {


                    string minutetext_ZERO = "0";
                    if (chatMessage.sendTime.Minute < 10) minutetext_ZERO = "";
                    Debug.Log("(child.GetComponent<TextObject>().TimeText.GetComponent<TMP_Text>().text :" + child.GetComponent<TextObject>().TimeText.GetComponent<TMP_Text>().text);
                    if (child.GetComponent<TextObject>().TimeText.GetComponent<TMP_Text>().text == chatMessage.sendTime.Hour + ":" + minutetext_ZERO + chatMessage.sendTime.Minute)
                    {
                        Debug.Log("有相同信息");
                        hasSame = true;


                    }
                    // Debug.Log($"ScriptA found on GameObject: {child.gameObject.name}");
                    // 在这里执行你的逻辑

                }
                else
                {
                    Debug.Log($"ScriptA not found on GameObject: {child.gameObject.name}");
                }


            }

            if (hasSame == false)
            {
                Debug.Log("生成對話信息");
                //生成對話物件
                if (chatMessage.userID == friendPlayFabID && chatMessage.friendID == PlayerInfoManager.Instance.PlayFabId) // 來自好友的信息
                {


                    string minutetext_ZERO = "0";

                    if (chatMessage.sendTime.Minute < 10) minutetext_ZERO = "0";
                    else minutetext_ZERO = "";

                    Debug.Log("信息來對方:" + chatMessage.userID + ",信息要給自己:" + chatMessage.friendID+ ",信息內容:"+ chatMessage.message);
                    peopleMsg.GetComponent<TextObject>().Msg.GetComponent<TMP_Text>().text = chatMessage.message;
                    peopleMsg.GetComponent<TextObject>().TimeText.GetComponent<TMP_Text>().text = chatMessage.sendTime.Hour + ":" + minutetext_ZERO + chatMessage.sendTime.Minute;
                    peopleMsg.GetComponent<TextObject>().AvatorImage.sprite = AvatorWantTobe;
                    peopleMsg.GetComponent<TextObject>().NameText.GetComponent<TMP_Text>().text = friendDisplayName;
                    Instantiate(peopleMsg, scrollContainer.transform); //生成一個list的prefab
                  
                }

                else if (chatMessage.userID == PlayerInfoManager.Instance.PlayFabId && chatMessage.friendID == friendPlayFabID) ;
                {
                    string minutetext_ZERO = "0";
                    Debug.Log("信息來自我自己:"+ chatMessage.userID +",信息要給對方:"+ chatMessage.friendID +",信息內容:" + chatMessage.message);
                    if (chatMessage.sendTime.Minute < 10) minutetext_ZERO = "0";
                    else minutetext_ZERO = "";
                   
                    myMsg.GetComponent<TextObject>().Msg.GetComponent<TMP_Text>().text = chatMessage.message;
                    myMsg.GetComponent<TextObject>().TimeText.GetComponent<TMP_Text>().text = chatMessage.sendTime.Hour + ":" + minutetext_ZERO + chatMessage.sendTime.Minute;
                    Instantiate(myMsg, scrollContainer.transform);
                  
                }
            } //  if (hasSame == false)
        }

        Debug.Log("生成COROTINE結束了");
        ScrollDown();
        yield return null;
        InAddPeopleToContainer = false;
       
    }
   
    public void ScrollDown()
    {
        ScrollRect scrollRect = scrollContainer.GetComponentInParent<ScrollRect>();
        Canvas.ForceUpdateCanvases(); // 确保布局已更新
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public GameObject MSG;
    public void SentMessageButton()
    {
        //判斷是否黑單



       SendPublicChatMessage( MSG.GetComponent<TMP_Text>().text);
        MSG.GetComponentInParent<TMP_InputField>().text = "";

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
        result =>
        {
            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            myUserName = result.PlayerProfile.DisplayName;
            isResponseReceived = true;
        },
        error => Debug.LogError("Get player Profile ERROR: " + error.GenerateErrorReport()));

        yield return new WaitUntil(() => isResponseReceived);
    }

    public class ChatMessage
    {
        public uint recno { get; set; }
        public string userID { get; set; }
        public string message { get; set; }
        public DateTimeOffset sendTime { get; set; }
        public string displayName { get; set; }
        public string avatarUrl { get; set; }
        public string friendID { get; set; }



        override public string ToString()
        {
            return $"recno: {recno}\nuserID: {userID}\nmessage: {message}\nsendTime: {sendTime}\ndisplayName: {displayName}\navatarUrl: {avatarUrl}";
        }
    }

}


