using Microsoft.AspNetCore.SignalR.Client;
using Player;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUiRedPoint : MonoBehaviour
{
    private static ChangeUiRedPoint _instance;

    public event EventHandler<string> UIRedPoint;

    private HubConnection connection;


    async void Start()
    {
        Initialize();

        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        AzureUpateNotOnline();
    }
    async void Initialize()
    {
        connection = new HubConnectionBuilder()
             .WithUrl("https://chatroomservice.azurewebsites.net/api")
             .WithAutomaticReconnect()
             .Build();

        // When receive the SignalRMessage of the target "OnMessageReceived"
        // the message would be a List of ChatMessage
        connection.On<List<GiftNotOnilne>>("OnlineCheckReceived", message =>
        {
            ChangeUiRedPoint.ChangeGiftRedPoint(true);

            Debug.Log("Received message from server:"); 
        });

        await connection.StartAsync();
    }

    //private void Awake()
    //{

    //}
    public static void AzureUpateNotOnline()//獲取有無未讀消息
    {
        //Debug.Log("=====");
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
                {"message", "Giftmessage"},
                {"displayName", "GiftmyUserName"},
                { "friendID","0"},
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},
                {"userConnectionId",_instance.connection.ConnectionId },//3FE9DB6C107ED4AF
                {"type",  "Gift"},
                {"typeChange",  "Gift"}

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

                    Debug.Log($"Theeeeeee {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");



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

        _instance.AzureUpateNotUID();

    }
    private async void AzureUpateNotUID()//更新自身UID
    {
        // 等待 myUserName 不为空
        while (OnlineList.myUserName == "")
        {
            await System.Threading.Tasks.Task.Delay(100); // 每100毫秒检查一次
        }
        ExecuteFunctionRequest cloudFunctionUID = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GetNickUidPlayFabID",
            FunctionParameter = new Dictionary<string, object>()
            {
                {"PlayFabID", ""},
                 {"PlayFabUID", OnlineList.myUserName.ToString()},
                {"type", "1"},

            },
            GeneratePlayStreamEvent = false
        };
        PlayFabCloudScriptAPI.ExecuteFunction(cloudFunctionUID,
                (ExecuteFunctionResult result) =>
                {
                   

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");

                });
    }
    public static void ChangeChatRedPoint(bool changeChatRedPoint)
    {
        string message = changeChatRedPoint ? "chat1" : "chat0";
        UiRedPoint.OnUIRedPoint(message);
        //_instance.UIRedPoint?.Invoke(_instance, message);
    }
    public static void ChangeBagRedPoint(bool changeBag1RedPoint)
    {
        string message = changeBag1RedPoint ? "Bag1" : "Bag0";
        UiRedPoint.OnUIRedPoint(message);
        //_instance.UIRedPoint?.Invoke(_instance, message);
    }
    public static void ChangeGiftRedPoint(bool changeGiftRedPoint)
    {
        string message = changeGiftRedPoint ? "Gift1" : "Gift0";
                UiRedPoint.OnUIRedPoint(message);
        //_instance.UIRedPoint?.Invoke(_instance, message);
    } public static void ChangeOptionRedPoint(bool changeOptionRedPoint)
    {
        string message = changeOptionRedPoint ? "Option1" : "Option0";
                UiRedPoint.OnUIRedPoint(message);
        //_instance.UIRedPoint?.Invoke(_instance, message);
    }



    #region botton
    public void IsChatRedPointbotton(bool ChatRedPointbottonBool)
    {
        ChangeChatRedPoint(ChatRedPointbottonBool);
    } 
    public void IsBagRedPointbotton(bool Bag1RedPointbottonBool)
    {
        ChangeBagRedPoint(Bag1RedPointbottonBool);
    } public static void IsGiftRedPointbotton(bool GiftRedPointbottonBool)
    {
        if(GiftRedPointbottonBool==false)
        {
            _instance.CancelAzureNotOnline();
        }
        ChangeGiftRedPoint(GiftRedPointbottonBool);
    } public void IsOptionRedPointbotton(bool OptionRedPointbottonBool)
    {
        ChangeOptionRedPoint(OptionRedPointbottonBool);
    }
    #endregion


    private void CancelAzureNotOnline()//取消當前在縣狀態
    {
        Debug.Log("=====");
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
                {"message", "Giftmessage"},
                {"displayName", "GiftmyUserName"},
                { "friendID","1"},//0為查詢，1為刪除在現狀態
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},
                {"userConnectionId",_instance.connection.ConnectionId },//3FE9DB6C107ED4AF
                {"type",  "Gift"},
                {"typeChange",  "Gift"}

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
                    var enterSuccess = result.FunctionResult.ToString();
                    Debug.Log(enterSuccess);
                    
                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            UIRedPoint?.Invoke(this, "chat1");
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            UIRedPoint?.Invoke(this, "Bag1");
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            UIRedPoint?.Invoke(this, "Gift1");
        } if(Input.GetKeyDown(KeyCode.O))
        {
            UIRedPoint?.Invoke(this, "Option1");
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            UIRedPoint?.Invoke(this, "chat0");
            UIRedPoint?.Invoke(this, "Bag0");
        }
        if (Input.GetKeyDown(KeyCode.T))
        {

        }
    }

}
