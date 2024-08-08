using Microsoft.AspNetCore.SignalR.Client;
using Player;
using PlayFab.CloudScriptModels;
using PlayFab;
using Share.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using static ChatManager;

public class ChatMessageSprite : MonoBehaviour
{
    HubConnection connection;
    bool getMsg = false;
    ChatMessage chatMessage1 = null;
    string myUserName;


    [SerializeField] private SensitiveWordFilter sensitiveWordFilter;
    [SerializeField] private StickerBtnPress stickerBtnPress;
    [SerializeField] private UrlImageGetter urlImageGetter;

    [SerializeField] private GameObject scrollContainer;
    [SerializeField] private GameObject peopleMsg;
    [SerializeField] private GameObject myMsg;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        GetPlayerProfile(PlayerInfoManager.Instance.PlayFabId);
    }
    private void Update()
    {
        if(getMsg)
        {
            StartCoroutine(AddPeopleToContainer(chatMessage1));
        }
    }
    async void Initialize()
    {

        connection = new HubConnectionBuilder()
            .WithUrl("https://chatroomservice.azurewebsites.net/api"

            )
            .WithAutomaticReconnect()
            .Build();


        connection.On<List<ChatMessage>>("publicChatImage", message =>//OnPrivateMessageReceived//OnMessageReceived
        {
            Debug.Log("Received message from server:");
            chatMessage1 = message[0];
            getMsg = true;
            foreach (ChatMessage c in message)
            {
                //ChangeChatRedPoint();
                Debug.Log("123" + c.recno);
                //Debug.Log("123" + c.userID);
                Debug.Log("123" + c.message);
                Debug.Log("123" + c.displayName);
                Debug.Log("avatarUrl" + c.avatarUrl);

                //string messageAfterFilter = c.message;
                //bool filtered = sensitiveWordFilter.StringCheckAndReplace(c.message, out messageAfterFilter);
                //Debug.Log(c.ToString() + $"\nmessageAfterFilter: {messageAfterFilter}\n");

            }
        });

        await connection.StartAsync();

    }    
   
    private IEnumerator AddPeopleToContainer(ChatMessage chatMessage)
    {
        getMsg = false;        
        string temp;
        sensitiveWordFilter.StringCheckAndReplace(chatMessage.message, out temp);
        yield return urlImageGetter.GettingSprite(chatMessage.avatarUrl, "avatar");
        List<string> strings1 = new List<string> { chatMessage.displayName, $"{chatMessage.sendTime.Hour:D2}:{chatMessage.sendTime.Minute:D2}" };
        List<string> strings2 = new List<string> { chatMessage.displayName, temp, $"{chatMessage.sendTime.Hour:D2}:{chatMessage.sendTime.Minute:D2}" };

        if (chatMessage.userID!= PlayerInfoManager.Instance.PlayFabId)
        {
            stickerBtnPress.Sticker(temp, strings1, urlImageGetter.sprite,false);
        }
    }

    public void GetPlayerProfile(string playFabId)
    {
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
        },
        error => Debug.LogError("Get player Profile ERROR: " + error.GenerateErrorReport()));

    }


    public void SendPublicChatImage(string message)
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
                {"type","publicChatImage" },
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
            stickerBtnPress.Sticker(temp, strings1, urlImageGetter.sprite, true);
        }
    }
}

public class ChatMessage
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
public class GiftNotOnilne
{
    public bool GiftCheck { get; set; }
   
}