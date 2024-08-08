using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftCenter : MonoBehaviour
{
    [SerializeField] private GameObject scrollContainerCenter;
    [SerializeField] private GameObject scrollContainerLog;
    [SerializeField] private GameObject GiftCenterPrefab;
    string nowTimeStr;
    public List<giftHistClass> giftHistClasses = new List<giftHistClass>();
    GiftNotOnilne _Gift=new GiftNotOnilne();

    HubConnection connection;
    [SerializeField] TMP_InputField textMeshProUGUIName;
    [SerializeField] TMP_InputField textMeshProUGUIDC;

    void OnEnable()
    {
        RestGiftCenter(0);
        RestGiftCenter(1);
    }
    async void Start()
    {
        Initialize();       
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
            //_Gift = message[0];

            //Debug.Log("Received message from server:" + _Gift.GiftCheck.ToString()+ "message.Count" + message.Count);
            int i = 0;
           
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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    InitGiftCenter();
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    StartCoroutine(CreatGit("16602815EADA97B5", 456));
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    RestGiftCenter(0);
        //    RestGiftCenter(1);
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    StartCoroutine(displayGiftCenter(0));
        //}
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    //AzureUpateNotOnline();
        //}
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        //    {
        //        Entity = new PlayFab.CloudScriptModels.EntityKey()
        //        {
        //            Id = PlayFabSettings.staticPlayer.EntityId,
        //            Type = PlayFabSettings.staticPlayer.EntityType,
        //        },
        //        FunctionName = "SendPublicChatMessage",
        //        FunctionParameter = new Dictionary<string, object>()//3FE9DB6C107ED4AF//EFEEFC8F72706E32
        //    {
        //        {"privateMessage", "Giftmessage"},//message//privateMessage
        //        {"displayName", "GiftmyUserName"},
        //        {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},
        //        {"type","Gift" },//private
        //        {"userConnectionId",connection.ConnectionId },//3FE9DB6C107ED4AF
        //        //{"playFabID123",},
        //        {"friendID","GiftfriendPlayFabID" }

        //    },
        //        GeneratePlayStreamEvent = false,

        //    };
        //    PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction,
        //            (ExecuteFunctionResult result) =>
        //            {
        //                if (result.FunctionResultTooLarge ?? false)
        //                {
        //                    Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
        //                    return;
        //                }

        //                Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");

        //                var enterSuccess = result.FunctionResult.ToString();
        //                Debug.Log(enterSuccess);

        //            }, (PlayFabError error) =>
        //            {
        //                Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
        //            });
        //}

    }

    #region button

    public void spendGiftForSomeoneBTN()//發送禮物給某人
    {
        AzureGetNickUidPlayFabID(textMeshProUGUIName.text.ToString());
        
        //FindPlayerIdByNickname(textMeshProUGUI.text);
    }

    public void resetGiftCenterBTN(int type)//更新贈禮中心的資料
    {
        RestGiftCenter(type);//0代表中心
        //RestGiftCenter(1);//1代表紀錄
    }




    #endregion
    //void FindPlayerIdByNickname(string nickname)
    //{
    //    var request = new GetAccountInfoRequest
    //    {
    //        PlayFabId = nickname
    //    };

    //    PlayFabClientAPI.GetAccountInfo(request, result =>
    //    {

    //        string playFabId = result.AccountInfo.PlayFabId;
    //        string _TitleDisplayName = result.AccountInfo.Username;
    //        Debug.Log("PlayFab ID for player " + textMeshProUGUI.text + " is: " + playFabId+ _TitleDisplayName);
    //    }, error =>
    //    {
    //        Debug.LogError("Failed to get account info: 請輸入正確的名子" + nickname);

    //    });
    //}


    private IEnumerator CreatGit(string receiverID, int _DCCount)
    {
        yield return getTimeFromCloud();

        giftHistClass newGiftHistClass = new giftHistClass()
        {
            number = "11222",
            time = nowTimeStr,
            senderID = PlayerInfoManager.Instance.PlayFabId,
            senderName = OnlineList.myUserName,
            receiverID = receiverID,
            DCCount = _DCCount,
            props = new List<string>(),
            step = 1,
            inFreqList = true
        };

        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GiveGift",
            FunctionParameter = new Dictionary<string, object>()//3FE9DB6C107ED4AF//EFEEFC8F72706E32
                {
                    { "Amount", 1 }, // 添加你要传递的参数
                    { "ReceiverId", receiverID },

                    {"number"      , "11222"} ,
                    {"time"        , nowTimeStr} ,
                    {"senderID"    , PlayerInfoManager.Instance.PlayFabId} ,
                    {"senderName"  , OnlineList.myUserName} ,
                    {"receiverID"  , receiverID} ,
                    {"DCCount"     , _DCCount} ,
                    {"props"       , new List<string>()} ,
                    {"step"        , 1} ,
                    { "inFreqList" , true},
                    {"type",0 },//0為新增
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
               AzureUpateNotOnline(receiverID);

           }, (PlayFabError error) =>
           {
               Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
           });

        giftHistClasses.Add(newGiftHistClass);
        Debug.Log(nowTimeStr);
    }
    //#region botton
    //public void testButtonPress1()
    //{
    //    //AddToGiftCenter("123", "4856", "2FE5ED78DAE18952", 10, new List<string>(), true);
    //    AddToGiftCenter(PlayerInfoManager.Instance.PlayFabId, "4856", "16602815EADA97B5", 10, new List<string>(), true);
    //}
    //public void testButtonPress2()
    //{
    //    StartCoroutine(displayGiftCenter(0));
    //}
    //#endregion

    #region buttonfunction
    public void AddToGiftCenter(string senderID, string senderName, string receiverID, int DCCount, List<string> props, bool inFreqList)
    {
        StartCoroutine(addToGiftCenter(senderID, senderName, receiverID, DCCount, props, inFreqList));
    }
    private IEnumerator addToGiftCenter(string senderID, string senderName, string receiverID, int DCCount, List<string> props, bool inFreqList)
    {
        giftHistClass newGiftHistObject = new giftHistClass();

        newGiftHistObject.number = "111111"; //還需要改

        yield return getTimeFromCloud();
        newGiftHistObject.time = nowTimeStr;

        newGiftHistObject.senderID = senderID;

        newGiftHistObject.senderName = senderName;

        newGiftHistObject.receiverID = receiverID;

        newGiftHistObject.DCCount = DCCount;

        newGiftHistObject.props = props;

        newGiftHistObject.step = 1;

        newGiftHistObject.inFreqList = inFreqList;
        
        //yield return ReadGiftCenter();
        giftHistClasses.Insert(0, newGiftHistObject); //資料由新到舊

        yield return WriteGiftCenter();

    }

    #endregion
    private IEnumerator displayGiftCenter(int typpe) //順便檢查是否超過確認的期限（24小時）
    {
        yield return getTimeFromCloud();
        DateTime now = stringToDateTime(nowTimeStr);

        yield return ReadGiftCenter();
        List <giftHistClass> newGiftHistClass = new List<giftHistClass>(); //用來儲存檢查過的List

        if (typpe == 0)
        {
            foreach (Transform child in scrollContainerCenter.transform)
            {
                // 销毁中心子对象
                GameObject.Destroy(child.gameObject);
            }
        }

        if (typpe == 1)
        {
            foreach (Transform child in scrollContainerLog.transform)
            {
                // 销毁紀錄子对象
                GameObject.Destroy(child.gameObject);
            }
        }

        var TextObjList = GiftCenterPrefab.GetComponentsInChildren<TMP_Text>();
        print("print TextObjList list: ");//用於確認是否是對的列表名稱
        for (int i = 0; i < TextObjList.Length; i++) //TextObjList.Length = 6
        {
            print(TextObjList[i].text);
        }
        var ButtonObjList = GiftCenterPrefab.GetComponentsInChildren<Button>();
        print("print ButtonObjList list: ");//用於確認是否是對的列表名稱
        for (int i = 0; i < ButtonObjList.Length; i++) //ButtonObjList.Length = 4
        {
            print(ButtonObjList[i].name);
        }
        var ImageObjList = GiftCenterPrefab.GetComponentsInChildren<Image>();
        print("print ImageObjList list: ");//用於確認是否是對的列表名稱
        for (int i = 0; i < ImageObjList.Length; i++) //ImageObjList.Length = 5
        {
            print(ImageObjList[i].name);
        }

        bool isChange =false;

        for (int i = 0; i < giftHistClasses.Count; i++)
        {
            TextObjList[5].text = "贈禮完成";

            if (typpe == 0)//用於更新贈禮中心或是贈禮紀錄
            {
                //Debug.Log(""+giftHistClasses[i].time);
                try
                {
                    DateTime timeOfListObect = stringToDateTime(giftHistClasses[i].time);
                    if ((now - timeOfListObect).Days <= 1) //不到24小時
                    {
                        newGiftHistClass.Add(giftHistClasses[i]);

                        TextObjList[0].text = giftHistClasses[i].number;
                        TextObjList[1].text = giftHistClasses[i].time;
                        TextObjList[2].text = giftHistClasses[i].senderName;
                        TextObjList[3].text = giftHistClasses[i].DCCount.ToString();
                        if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].senderID)//本人為贈送方
                        {
                            if (giftHistClasses[i].step == 1)
                            {
                                TextObjList[4].enabled = true;           //顯示等待中的文字
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 2)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = true;    //送禮方確認
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[3].enabled = true;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                        }
                        else //if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].receiverID)//本人為接收方
                        {
                            if (giftHistClasses[i].step == 1)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;          //收禮方確認
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;//0為道具
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = true;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[1].enabled = true;
                                ButtonObjList[2].enabled = true;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[2].enabled = true;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 2)
                            {
                                TextObjList[4].enabled = true;           //顯示等待中的文字
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 3)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;            //送禮驗證
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = true;            //領取
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[4].enabled = true;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                        }
                    }
                    else if(giftHistClasses[i].step == 1)
                    {
                        giftHistClasses[i].step = 5;
                        isChange = true;
                    }    
                }
                catch
                {
                    DateTime dateTime = DateTime.Parse(giftHistClasses[i].time);
                    if ((now - dateTime).Days <= 1) //不到24小時
                    {
                        newGiftHistClass.Add(giftHistClasses[i]);

                        TextObjList[0].text = giftHistClasses[i].number;
                        TextObjList[1].text = giftHistClasses[i].time;
                        TextObjList[2].text = giftHistClasses[i].senderName;
                        TextObjList[3].text = giftHistClasses[i].DCCount.ToString();
                        if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].senderID)//本人為贈送方//I am the giver
                        {
                            if (giftHistClasses[i].step == 1)
                            {
                                TextObjList[4].enabled = true;           //顯示等待中的文字
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 2)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = true;    //送禮方確認
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[3].enabled = true;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                        }
                        else //if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].receiverID)//本人為接收方//scroll area adds a new object
                        {
                            if (giftHistClasses[i].step == 1)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;          //收禮方確認
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;//0為道具
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = true;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[1].enabled = true;
                                ButtonObjList[2].enabled = true;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[2].enabled = true;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 2)
                            {
                                TextObjList[4].enabled = true;           //顯示等待中的文字
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                            if (giftHistClasses[i].step == 3)
                            {
                                Debug.Log("SHUHSIUHDF : " + TextObjList.Length);
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = false;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;            //送禮驗證
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = true;            //領取
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = true;
                                ImageObjList[4].enabled = true;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerCenter.transform); //scroll area 新增一個object
                            }
                        }
                    }
                    else if(giftHistClasses[i].step == 1)
                    {
                        giftHistClasses[i].step = 5;
                        isChange = true;
                    }
                }
            }
            else
            {
                try
                {
                    
                     newGiftHistClass.Add(giftHistClasses[i]);

                     TextObjList[0].text = giftHistClasses[i].number;
                     TextObjList[1].text = giftHistClasses[i].time;
                     TextObjList[2].text = giftHistClasses[i].senderName;
                     TextObjList[3].text = giftHistClasses[i].DCCount.ToString();
                     if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].senderID)//本人為贈送方
                     {
                         if (giftHistClasses[i].step == 4)
                            {
                                TextObjList[4].enabled = false;           //顯示等待中的文字
                                TextObjList[5].enabled = true;

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerLog.transform); //scroll area 新增一個object//scroll area adds a new object
                        }
                         //step = 5 是因為時間過24小時，需要做相應的UI
                         if (giftHistClasses[i].step == 5)
                            {
                                TextObjList[4].enabled = false;           //顯示等待中的文字
                                TextObjList[5].enabled = true;
                                TextObjList[5].text = "贈禮失敗";

                                ButtonObjList[0].enabled = false;
                                ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerLog.transform); //scroll area 新增一個object//scroll area adds a new object
                        }

                     }
                    else //if (PlayerInfoManager.Instance.PlayFabId == giftHistClasses[i].receiverID)//本人為接收方//I am the recipient
                    {
                         if (giftHistClasses[i].step == 4)
                            {
                                TextObjList[4].enabled = false;
                                TextObjList[5].enabled = true;

                                ButtonObjList[0].enabled = false;          //收禮方確認//Recipient confirmation
                            ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerLog.transform); //scroll area 新增一個object//scroll area adds a new object
                        }
                        //step = 5 是因為時間過24小時，需要做相應的UI////step = 5 is because the time has passed 24 hours and corresponding UI needs to be done.
                        if (giftHistClasses[i].step == 5)
                            {
                            TextObjList[4].enabled = false;           //顯示等待中的文字
                            TextObjList[5].enabled = true;
                            TextObjList[5].text = "贈禮失敗";

                            ButtonObjList[0].enabled = false;          //收禮方確認//Recipient confirmation
                            ButtonObjList[0].GetComponentInChildren<Text>().enabled = false;//0為道具
                            ImageObjList[0].enabled = false;
                                ButtonObjList[1].enabled = false;
                                ButtonObjList[1].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[1].enabled = false;
                                ButtonObjList[2].enabled = false;
                                ButtonObjList[2].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[2].enabled = false;
                                ButtonObjList[3].enabled = false;
                                ButtonObjList[3].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[3].enabled = false;
                                ButtonObjList[4].enabled = false;
                                ButtonObjList[4].GetComponentInChildren<Text>().enabled = false;
                                ImageObjList[4].enabled = false;

                                GiftCenterScripts _GiftCenterScripts = GiftCenterPrefab.GetComponent<GiftCenterScripts>();
                                _GiftCenterScripts._GiftCenter = this;
                                _GiftCenterScripts.num = i;

                                Instantiate(GiftCenterPrefab, scrollContainerLog.transform); //scroll area 新增一個object//scroll area adds a new object
                        }
                        
                     }
                                        
                }
                catch(Exception e) 
                {
                   Debug.Log(e);                   
                }
            }
        }

        giftHistClasses = newGiftHistClass;

        if(isChange)
        {
            yield return WriteGiftCenter();
        }
    }

    private IEnumerator getTimeFromCloud()
    {
        bool isResponseReceived = false;

        var request = new ExecuteFunctionRequest
        {
            FunctionName = "getNowDateTime",
            FunctionParameter = new
            {
                playfabid = PlayerInfoManager.Instance.PlayFabId,
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
        {
            Debug.Log("get nowtime sucsess:" + result.ToString());

            List<string> getRichContainList = result.FunctionResult.ToString().Split(',').ToList();
            nowTimeStr = getRichContainList[0];
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("get nowtime failed:" + error.ErrorMessage);
            isResponseReceived = true;
        });

        yield return new WaitUntil(() => isResponseReceived);
    }

    private DateTime stringToDateTime(string time)
    {
        char[] splitChar = { '-', ' ', ':' };
        string[] nowTimeList = time.Split(splitChar);
        DateTime now = new DateTime(Convert.ToInt32(nowTimeList[0]), Convert.ToInt32(nowTimeList[1]), Convert.ToInt32(nowTimeList[2]), Convert.ToInt32(nowTimeList[3]), Convert.ToInt32(nowTimeList[4]), Convert.ToInt32(nowTimeList[5]));

        return now;
    }

    private string ListOfObjectToString(List<giftHistClass> list)
    {
        List<string> strs = new List<string>();
        string result;

        //convert all object to string
        for (int i = 0; i < list.Count; i++)
        {
            strs.Add(JsonConvert.SerializeObject(list[i]));
        }

        //convert list of string to string
        result = String.Join(">->->->", strs.ToArray()); //單純用","的話有人名字包含這個字符很難處理
        return result;
    }
    private List<giftHistClass> StringToListOfObject(string str)
    {
        Debug.Log(str);
        //List<GiftCenter.giftHistClass> giftList = JsonConvert.DeserializeObject<List<GiftCenter.giftHistClass>>(str);

        List<string> strs = new List<string>(str.Split(">->->->"));
        List<giftHistClass> list = new List<giftHistClass>();

        //var jsonArray = JArray.Parse(str);
        try
        {
            for (int i = 0; i < strs.Count; i++)
            {
                //print(strs[i]);
                if (strs[i] != null && strs[i] != "null")
                    list.Add(JsonConvert.DeserializeObject<GiftCenter.giftHistClass>(strs[i]));               
            }
        }
        catch {
            for (int i = 0; i < strs.Count; i++)
            {
                //print(strs[i]);
                if (strs[i] != null && strs[i] != "null")
                    list = JsonConvert.DeserializeObject<List<GiftCenter.giftHistClass>>(str);               
            }
        }      
        

        return list;
    }

    #region AzureFunction
    public void RestGiftCenter(int type)
    {
        if (type == 0)
        {
            foreach (Transform child in scrollContainerCenter.transform)
            {
                // 销毁中心子对象
                GameObject.Destroy(child.gameObject);
            }
        }

        if (type == 1)
        {
            foreach (Transform child in scrollContainerLog.transform)
            {
                // 销毁紀錄子对象
                GameObject.Destroy(child.gameObject);
            }
        }
        StartCoroutine(displayGiftCenter(type));//顯示禮物畫面

    }
    private void InitGiftCenter()//清空自己的禮物紀錄
    {

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"GiftCenter", ""}, //初始什麼都沒有
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data: ");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    private IEnumerator ReadGiftCenter()//读取礼物中心的数据
    {
        bool isResponseReceived = false;
        List<string> keys = new List<string>();
        keys.Add("GiftCenter");

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("GiftCenter"))
            {
                Debug.Log("initial GiftCenter.");
                InitGiftCenter();
                isResponseReceived = true;
            }
            else
            {
                Debug.Log("GiftCenter: " + result.Data["GiftCenter"].Value);
                giftHistClasses = StringToListOfObject(result.Data["GiftCenter"].Value);
                isResponseReceived = true;
            }

        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

        yield return new WaitUntil(() => isResponseReceived);

    }
    public IEnumerator WriteGiftCenter()
    {
        Debug.Log(""+giftHistClasses.Count);
        bool isResponseReceived = false;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {"GiftCenter", ListOfObjectToString(giftHistClasses)},
            }
        },
        result =>
        {
            Debug.Log("Successfully updated user data");
            isResponseReceived = true;
        },
        error => {
            Debug.Log("Got error setting user data GiftCenter");
            Debug.Log(error.GenerateErrorReport());
        });

        yield return new WaitUntil(() => isResponseReceived);
    }
    public void AzureUpate(string receiverId, string number,int step,int num)
    {

        if (giftHistClasses.Count > 0)
        {
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            Entity = new PlayFab.CloudScriptModels.EntityKey()
            {
                Id = PlayFabSettings.staticPlayer.EntityId,
                Type = PlayFabSettings.staticPlayer.EntityType,
            },
            FunctionName = "GiveGift",
            FunctionParameter = new Dictionary<string, object>()//3FE9DB6C107ED4AF//EFEEFC8F72706E32
                {
                    { "Amount", 0 }, // 添加你要传递的参数
                    { "ReceiverId", giftHistClasses[num].receiverID },


                    {"number"      ,giftHistClasses[num].number} ,
                    {"time"        , giftHistClasses[num].time} ,
                    {"senderID"    , PlayerInfoManager.Instance.PlayFabId} ,
                    {"senderName"  , giftHistClasses[num].senderName} ,
                    {"receiverID"  , receiverId} ,
                    {"DCCount"     , giftHistClasses[num].DCCount} ,
                    {"props"       , giftHistClasses[num].props} ,
                    {"step"        , step} ,
                    { "inFreqList" , true},
                    {"type",1 },//0為新增
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
               AzureUpateNotOnline(receiverId);

           }, (PlayFabError error) =>
           {
               Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
           });
        }
      
    }


    public void AzureUpateNotOnline(string receiverId)
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
                {"message", "Giftmessage"},
                {"displayName", "GiftmyUserName"},
                { "friendID",receiverId},
                {"avatarUrl", PlayerInfoManager.Instance.PlayerInfo.avatar.Url},
                {"userConnectionId",connection.ConnectionId },//3FE9DB6C107ED4AF
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
    }

    private void AzureGetNickUidPlayFabID(string PlayFabUID)
    {

        Debug.Log("" + PlayFabUID);
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
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
                 {"PlayFabUID", PlayFabUID},
                 {"type", "0"},//1為上線時更新名稱

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

                    var enterSuccess = result.FunctionResult.ToString();
                    Debug.Log(enterSuccess);

                    int resultcount;
                    if (int.TryParse(textMeshProUGUIDC.text.ToString(), out resultcount))
                    {
                        if (resultcount >= 2000)
                        {
                            StartCoroutine(CreatGit(enterSuccess.ToString(), int.Parse(textMeshProUGUIDC.text.ToString())));

                        }
                        else
                        {
                            //做低於金額的UI
                        }
                    }
                    else
                    {
                        //做輸入錯誤資訊的UI(裡面必須是數字)
                    }

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");

                });


    }
    #endregion

    public class giftHistClass
    {
        public string number { get; set; } //編號
        public string time { get; set; } //發送時間
        public string senderID { get; set; } //發送者playfabID
        public string senderName { get; set; } //發送者user name
        public string receiverID { get; set; } //接收者playfabID
        public int DCCount { get; set; } //傳送的龍幣數額
        public List<string> props { get; set; } //道具，最多6種
        public int step {  get; set; } //當前正在執行的步驟, 1:等待接收方確認，2:等待贈禮方確認，3：等待接收方領取
        public bool inFreqList {  get; set; } //0:不在常用名單，1:在常用名單
    }
}
