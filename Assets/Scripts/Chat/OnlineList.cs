using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLobby;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Player;
using PlayFab.ClientModels;

using PlayFab;
using System;
using Tool;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Net.Http;
using TMPro;
using UnityEngine.UI;
using Share.Tool;
using Loading;
using System.Threading;
using Unity.Services.Core.Environments;
using PlayFab.CloudScriptModels;


public class OnlineList : MonoBehaviour
{
    private Lobby hostLobby;
    private Lobby nowOnlineLobby;
    private float heartbeatTimer;
    public GameObject PlayListPanel;
    public static OnlineList Instance { get; private set; }

    [SerializeField] private GameObject scrollContainer;
    [SerializeField] private GameObject playerListContain;

    [SerializeField] private UrlImageGetter urlImageGetter;
    [SerializeField] private TMP_InputField searchText;



    private void OnEnable()
    {
        LobbyEventHandler.RefreshFriendList += OnRefreshFriendList;
    }

    private void OnDisable()
    {
        LobbyEventHandler.RefreshFriendList -= OnRefreshFriendList;


    }

    private void OnRefreshFriendList()
    {
        refreshFriendList();
        urlImageGetter.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // 如果需要保持该对象在场景切换时不被销毁
        }
       
    }

    public async void Start()
    {
        myAvatarUrl = PlayerInfoManager.Instance.PlayerInfo.avatar.Url;
    }
    public void FixedUpdate()
    {
        // 在FixedUpdate中调用异步方法
        //QueryLobbiesAsync();
    }

    private async void QueryLobbiesAsync()
    {
        try
        {
            // 查询所有大厅
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            if (queryResponse.Results.Count==0) { 
             // 打印查询结果的数量
            Debug.Log("lobby zero: " + queryResponse.Results.Count);
            }
           
        }
        catch (Exception e)
        {
            //Debug.LogError("Error querying lobbies: " + e);
        }
    }

    public static void leaveOnlineListStaic()
    {
        Instance.leaveOnlineList();
    }

    public async void enterOnlineList()
    {
        try
        {
            await UnityServices.InitializeAsync();
            // 检查玩家是否已登录
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // 清除之前的会话令牌
                AuthenticationService.Instance.ClearSessionToken();
            }

            // 注册 SignedIn 事件
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
            };

            // 匿名登录
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Anonymous sign-in successful.");
            }
            else
            {
                Debug.Log("Player is already signed in: " + AuthenticationService.Instance.PlayerId);
            }           
        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }

        StartCoroutine(GetPlayerProfile(PlayerInfoManager.Instance.PlayFabId)); //displayName, avatar url
        GetPlayerData(PlayerInfoManager.Instance.PlayFabId); //活躍度, vip, 等級, 金幣

        JoinLobby();
    }
    public async void leaveOnlineList()
    {
        try
        {
            await UnityServices.InitializeAsync();
            print("id: " + AuthenticationService.Instance.PlayerId);
            AuthenticationService.Instance.SignedOut += () =>
            {
                Debug.Log("Signed Out");
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            hostLobby = queryResponse.Results[0];
            print(hostLobby.Id);
            print(AuthenticationService.Instance.PlayerId);
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Leave Lobby Success");

            //Debug.Log("List remain Lobbies");
            //ListLobbies();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Leave Lobby Error" + e);
        }
    }

    [Header("是好友名單嗎")]
   public bool isFriendList = false;
    public async void refreshFriendList()
    {

        isFriendList = true;
        

        Debug.Log("refreshFriendList()");
        destroy();
        GetFriendsList();
       



    }


    public void ClickBackToPlayerList()
    {
        if (isFriendList) refreshFriendList();
        else refreshOnlineList();

    }


    private  void GetFriendsList()
    {

        Debug.Log("GetFriendsList()");
        var request = new GetFriendsListRequest
        {

            ProfileConstraints = new PlayerProfileViewConstraints
            {
      
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };

        PlayFabClientAPI.GetFriendsList(request, OnGetFriendsListSuccess, OnGetFriendsListError);




    }
    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {


        if (playerListContain == null) return;
        Debug.Log("Friends List Retrieved:");
        if (result.Friends != null)
        {
            foreach (var friend in result.Friends)
            {
                Debug.Log($"Friend PlayFabId: {friend.FriendPlayFabId}");
                Debug.Log($"Display Name: {friend.Profile?.DisplayName}");
                Debug.Log($"Avatar URL: {friend.Profile?.AvatarUrl}");
                // 打印更多的好友信息
                playerListContain.GetComponent<FriendObj>().id = friend.FriendPlayFabId;
                playerListContain.GetComponent<FriendObj>().Name = friend.Profile?.DisplayName;
                playerListContain.GetComponent<FriendObj>().HeadIconURL = friend.Profile?.AvatarUrl;
                var objList = playerListContain.GetComponentsInChildren<TMP_Text>();
                var objList2 = playerListContain.GetComponentsInChildren<Image>();

                objList[0].text = playerListContain.GetComponent<FriendObj>().Name;
                // objList[1].text = "活躍度：" + player.Data["activity"].Value;

                //   urlImageGetter.GettingSprite(playerListContain.GetComponent<FriendObj>().HeadIconURL, "avatar");

                //  objList2[2].sprite = urlImageGetter.sprite;
             
                GameObject friendItem = Instantiate(playerListContain, scrollContainer.transform);
                friendItem.GetComponent<FriendObj>().SetHeadIcon( playerListContain.GetComponent<FriendObj>().HeadIconURL);

            }
        }
        else
        {
            Debug.Log("No friends found.");
        }
    }



    private static void OnGetFriendsListError(PlayFabError error)
    {
        Debug.Log($"Error retrieving friends list: {error.ErrorMessage}");
    }

    public async void refreshOnlineList()
    {
        isFriendList = false;
        try
        {
            //var options = new InitializationOptions()
            //   .SetEnvironmentName("test"); // 替换为你的环境名称
            // 初始化 Unity Services
            await UnityServices.InitializeAsync();
            // 检查玩家是否已登录
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                // 清除之前的会话令牌
                AuthenticationService.Instance.ClearSessionToken();
            }

            // 注册 SignedIn 事件
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
            };

            // 匿名登录
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Anonymous sign-in successful.");
            }
            else
            {
                Debug.Log("Player is already signed in: " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Authentication error: " + ex.ToString());
            return;
        }

        // 获取玩家信息
        Debug.Log("获取玩家信息");
        StartCoroutine(GetPlayerProfile(PlayerInfoManager.Instance.PlayFabId)); // displayName, avatar url
      
        GetPlayerData(PlayerInfoManager.Instance.PlayFabId); // 活躍度, vip, 等級, 金幣

        try
        {
            // 清空之前的玩家列表
            destroy();  // delete all players previous
            Debug.Log("Previous players destroyed.");

            // 查询大厅列表
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            Debug.Log("Lobbies queried successfully.");

            if (queryResponse.Results.Count > 0)
            {
                nowOnlineLobby = queryResponse.Results[0];
                Debug.Log("First lobby selected: " + nowOnlineLobby.Name);

                // 安排玩家并显示
                StartCoroutine(ArrangePlayersAndDisplay(nowOnlineLobby));
                Debug.Log("Players arranged and displayed.");



                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log("Lobby Name: " + lobby.Name);
                    Debug.Log("Current players in the lobby: " + lobby.Players.Count); // 获取当前大厅的玩家数量
                    
                }

                // 启动搜索文本检测
                StartCoroutine(searchTextDetect());
            }
            else
            {
                Debug.Log("First lobby selected: " + queryResponse.Results.Count);

                //if (queryResponse.Results.Count == 0)
                //{
                //    CreateLobby();
                //}

                Debug.LogWarning("No lobbies found.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error querying lobbies or arranging players: " + ex.ToString());

            // 尝试重新加入大厅
            JoinLobby();
        }
    }


    private void Update()
    {
        HandleLobbyHeartbeat();
    }
    IEnumerator searchTextDetect()
    {
        if (isFriendList == false)
        {
            string previousSearchText = ""; //initial blank
           // while (true)
           // {
           //     if (previousSearchText != searchText.text)
           //     {
           //         previousSearchText = searchText.text; //update
           //         print("searching: " + searchText.text);
           //         yield return DisplaySubList(previousSearchText);
           //     }
           //     yield return new WaitForSecondsRealtime(2); //等兩秒重新detect
           // }
        }

        else
        {

            while (true)
            {
                if (searchText.text != "")
                {
                    
                }

                else
                {
                    Debug.Log("不搜尋空字串");
                }
                yield return new WaitForSecondsRealtime(5); //等兩秒重新detect
            }


        }

        yield return null;
    }

    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    public void BtnPress()
    {
        Debug.Log("Call ListLobbies()");
        ListLobbies();
    }
    public void BtnPress2()
    {
        Debug.Log("Call JoinLobby()");
        JoinLobby();
    }
    public void BtnPress3()
    {
        Debug.Log("Call LeaveLobby()");
        LeaveLobby();
    }
    public void BtnPress4()
    {
        Debug.Log("Call ");
        ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
        {
            
            FunctionName = "OnlineList",
            FunctionParameter = new Dictionary<string, object>()
            {
                    {"time"        , "2024-05-31 15:26:30"} ,
                    {"senderID"    , PlayerInfoManager.Instance.PlayFabId} ,
                    {"senderName"  , "456"} ,
                    {"DCCount"     , 0} ,
                    {"props"       , new List<string>()} ,
                    { "inFreqList" , true},
                    {"type",1 },//0為新增
            },
            GeneratePlayStreamEvent = true,
        };
        PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction,
                (ExecuteFunctionResult result) =>
                {
                   

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });
    }
    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "OnlineList";
            int maxPlayer = 100;
            myAvatarUrl = PlayerInfoManager.Instance.PlayerInfo.avatar.Url;
            // 等待 myUserName 不为空
            while (myUserName == "")
            {
                await System.Threading.Tasks.Task.Delay(100); // 每100毫秒检查一次
            }
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions()
            {
                IsPrivate = false,
                Player = new Unity.Services.Lobbies.Models.Player()
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "playerName",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, myUserName)},
                        { "myAvatarUrl",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, myAvatarUrl)},
                        { "activity",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, activity.ToString())},
                        { "vip",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, vip.ToString())},
                        { "level",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, level.ToString())},
                        { "DC",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DC.ToString())},
                        { "PlayFabID",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playFabID)},
                    }
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

            hostLobby = lobby;

            Debug.Log("myUserName Lobby: " + myUserName);
            Debug.Log("Created Lobby: " + lobby.Name + " " + lobby.MaxPlayers);
        } catch (LobbyServiceException e)
        {
            Debug.Log("Create Lobby Error: " + e);
        }
    }

    private async void ListLobbies()//列出大厅（Lobbies）
    {
        try {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            if(queryResponse != null)
            {
                Debug.Log("Lobbies Found: " + queryResponse.Results.Count);
                foreach (Lobby lobby in queryResponse.Results)
                {
                    Debug.Log($"{lobby.Name} {lobby.MaxPlayers}");
                }
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log("Get Lobby List Error: " + e);
        }
    }

    public async void JoinLobby()
    {
        LoadingManger.Instance.Open_Loading_animator();
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count == 0)
            {
                CreateLobby();
                Debug.Log("CreateLobby");

            }
            else
            {
                int resultsCount = queryResponse.Results.Count - 1;
                if (queryResponse.Results[resultsCount].Players.Count > 95)//設95是怕一次性多五個人導致人數超過限制
                {
                    CreateLobby();
                }
                while (myUserName == "")
                {
                    await System.Threading.Tasks.Task.Delay(100); // 每100毫秒检查一次
                }
                Debug.Log("myAvatarUrl"+ myAvatarUrl);
                print("JoinLobby content: " + myUserName + " " + myAvatarUrl + " " + activity.ToString() + " " + vip.ToString() + " " + level.ToString() + " " + DC.ToString());
                JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
                {
                    Player = new Unity.Services.Lobbies.Models.Player()
                    {
                        Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "playerName",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, myUserName)},
                         { "PlayFabID",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playFabID)},
                        { "myAvatarUrl",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, myAvatarUrl)},
                        { "activity",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, activity.ToString())},
                        { "vip",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, vip.ToString())},
                        { "level",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, level.ToString())},
                        { "DC",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, DC.ToString())}
                    }
                    }
                };
                Debug.Log("Current players in the lobby: " + queryResponse.Results[resultsCount].Players.Count); // 获取当前大厅的玩家数量

               
                hostLobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[resultsCount].Id, joinLobbyByIdOptions);//加入玩家資訊

                Debug.Log("Join Lobby Success");
                Debug.Log("After join lobby, lobby player list: ");
                PrintPlayers(hostLobby);
            }
            LoadingManger.Instance.Close_Loading_animator(); //完成add to online list
        }
        catch (LobbyServiceException e)
        {
            Debug.Log("Join Lobby Error: " + e);
            //// 等待一段时间后再次尝试
            //await System.Threading.Tasks.Task.Delay(5000); // 例如等待 5 秒钟
            //JoinLobby(); // 重新尝试加入大厅
        }
        finally
        {
            LoadingManger.Instance.Close_Loading_animator(); // 关闭加载动画，操作完成
        }
    }

    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in Lobby " + lobby.Name + ": ");

       
        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            DisplayDictionaryItems(player.Data);
            Debug.Log(player.Id + " " + player.Data["playerName"].Value + player.Data["myAvatarUrl"].Value + " " + player.Data["activity"].Value + " " + player.Data["vip"].Value + " " + player.Data["level"].Value + " " + player.Data["DC"].Value);
        }
    }

    private void DisplayDictionaryItems(Dictionary<string, PlayerDataObject> dictionary)
    {
        // 遍历字典中的所有键值对
        foreach (KeyValuePair<string, PlayerDataObject> kvp in dictionary)
        {
            Debug.Log($"player.DataKey: {kvp.Key}, Value: {kvp.Value.Value}");
        }
    }
    IEnumerator testHaveLobby()
    {
        bool isResponseReceived = false;
        while(hostLobby == null)
            yield return new WaitForSecondsRealtime(1);

        JoinLobby();
        isResponseReceived = true;
        yield return new WaitUntil(() => isResponseReceived);
        yield return null;
    }
    private async void destroy()
    {


        if (scrollContainer != null)
        {
            Transform parentTransform = scrollContainer.transform;

            foreach (Transform child in parentTransform)
            {
                Destroy(child.gameObject);
            }
        }

    }

    IEnumerator  ArrangePlayersAndDisplay(Lobby lobby)
    {
        bool isResponseReceived = false;
        var objList = playerListContain.GetComponentsInChildren<TMP_Text>();
        var objList2 = playerListContain.GetComponentsInChildren<Image>();
        List<Unity.Services.Lobbies.Models.Player> players = lobby.Players;

        //foreach (var obj in objList)
        //{
        //    print(obj.ToString());
        //}
        //foreach (var obj in objList2)
        //{
        //    print(obj.ToString());
        //}

        ////add virtual player to test
        //players.Add(new Unity.Services.Lobbies.Models.Player()
        //{
        //    Data = new Dictionary<string, PlayerDataObject>
        //            {
        //                { "playerName",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "abc lo")},
        //                { "myAvatarUrl",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "https://playeravatar.blob.core.windows.net/avatar/avatar_2FE5ED78DAE18952.png")},
        //                { "activity",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "vip",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "level",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "DC",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "6666")}
        //            }
        //});
        //players.Add(new Unity.Services.Lobbies.Models.Player()
        //{
        //    Data = new Dictionary<string, PlayerDataObject>
        //            {
        //                { "playerName",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "abc looooooo")},
        //                { "myAvatarUrl",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "https://playeravatar.blob.core.windows.net/avatar/avatar_2FE5ED78DAE18952.png")},
        //                { "activity",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "vip",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "level",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")},
        //                { "DC",  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, "66")}
        //            }
        //});

        yield return quickSortPlayerList(players, 0, players.Count - 1);
        yield return bucketSortPlayerList(players, "activity", "vip");
        yield return bucketSortPlayerList(players, "vip", "level");
        yield return bucketSortPlayerList(players, "level", "DC");

        Debug.Log("Players in Lobby " + lobby.Name.ToString() + ": ");
        Debug.Log("myUserName " + myUserName);
        foreach (Unity.Services.Lobbies.Models.Player player in players)
        {
            DisplayDictionaryItems(player.Data);
           if (player.Data["playerName"].Value != myUserName)
          //  if(true)
            {
                Debug.Log("player.Data[playerName].Value :" + player.Data["playerName"].Value);
                objList[0].text = player.Data["playerName"].Value;
                objList[1].text = "活躍度：" + player.Data["activity"].Value;

                yield return urlImageGetter.GettingSprite(player.Data["myAvatarUrl"].Value, "avatar");
                objList2[2].sprite = urlImageGetter.sprite;
                //playerListContain.GetComponent<FriendObj>().id = player.Id;123
                playerListContain.GetComponent<FriendObj>().Name = player.Data["playerName"].Value;
                playerListContain.GetComponent<FriendObj>().HeadIconURL = player.Data["myAvatarUrl"].Value;
                playerListContain.GetComponent<FriendObj>().Activity = player.Data["activity"].Value;
                playerListContain.GetComponent<FriendObj>().VIP = player.Data["vip"].Value;
                playerListContain.GetComponent<FriendObj>().LEVEL = player.Data["level"].Value;
                //playerListContain.GetComponent<FriendObj>().id = AzureGetNickUidPlayFabID(playerListContain.GetComponent<FriendObj>().Name);
                lastPlayFabID = "";
               GameObject FriendOBJ_SPAWN =  Instantiate(playerListContain, scrollContainer.transform); //生成一個list的prefab
               AzureGetNickUidPlayFabID(playerListContain.GetComponent<FriendObj>().Name);
            }
            Debug.Log(player.Id + " player.Data " + player.Data["playerName"].Value 
                + "myAvatarUrl" + player.Data["myAvatarUrl"].Value + " " + player.Data["activity"].Value 
                + " " + player.Data["vip"].Value + " " + player.Data["level"].Value + " " + player.Data["DC"].Value);
          
        }
        isResponseReceived = true;
        yield return new WaitUntil(() => isResponseReceived);
        yield return null;
    }
    private void AzureGetNickUidPlayFabID(string PlayFabUID)
    {
        string tmpHOGA = "";
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
                    Debug.Log("enterSuccess:"+enterSuccess);
                  
                    // FreindOBJ.GetComponent<FriendObj>().id = enterSuccess;
                    StartCoroutine(FindNickNameHasSpawnInList_SetID(PlayFabUID, enterSuccess));
                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");

                });
        Debug.Log("tmpHOGA:" + tmpHOGA);
        
    }


    IEnumerator FindNickNameHasSpawnInList_SetID(string Name,string ID)
    {
        yield return null;

        if (scrollContainer != null)
        {
            Transform parentTransform = scrollContainer.transform;

            foreach (Transform child in parentTransform)
            {
              if(child.GetComponent<FriendObj>().Name == Name)  child.GetComponent<FriendObj>().id = ID;
            }
        }

        yield return null;
    }


    string lastPlayFabID = "";

    private void GetPlayerIdByDisplayName(string displayName)
    {
        Debug.Log("GetPlayerIdByDisplayName:" + displayName);
        AzureGetNickUidPlayFabID(displayName);
    }





    IEnumerator quickSortPlayerList(List<Unity.Services.Lobbies.Models.Player> players, int start, int end)
    {
        int pivot = start, left = start + 1, right = end;
        while(left < right)
        {
            while (Int32.Parse(players[right].Data["activity"].Value) < Int32.Parse(players[pivot].Data["activity"].Value))
                right--;
            while(Int32.Parse(players[left].Data["activity"].Value) > Int32.Parse(players[pivot].Data["activity"].Value) && left < right)
                left++;

            if (left > right)
                break;
            var temp = players[left];
            players[left] = players[right];
            players[right] = temp;
        }
        if (Int32.Parse(players[pivot].Data["activity"].Value) > Int32.Parse(players[right].Data["activity"].Value))//防止出現end == start + 1時不sort的問題
            right = pivot;
        //center point
        var temp2 = players[pivot];
        players[pivot] = players[right];
        players[right] = temp2;

        //continue quicksort
        if(right - 1 >= start && right - 1 - pivot >= 1) // list count >= 2
            yield return quickSortPlayerList(players, pivot, right - 1);
        if (right + 1 <= end && end - (right + 1) >= 1)
            yield return quickSortPlayerList(players, right + 1, end);



        yield return null;
    }

    IEnumerator bubbleSortPlayerList(List<Unity.Services.Lobbies.Models.Player> players)
    {
        bool isResponseReceived = false;

        //bubble sort players' activity
        for(int i = 0; i < players.Count - 1; i++)
        {
            for(int j = i; j < players.Count - 1; j++)
            {
                if (Int32.Parse(players[j].Data["activity"].Value) < Int32.Parse(players[j + 1].Data["activity"].Value))
                {
                    var temp = players[j];
                    players[j] = players[j + 1];
                    players[j + 1] = temp;
                }
            }
        }

        isResponseReceived = true;
        yield return new WaitUntil(() => isResponseReceived);

        yield return null;
    }

    IEnumerator bucketSortPlayerList(List<Unity.Services.Lobbies.Models.Player> players, string bucketName, string bucketContent)
    {
        bool isResponseReceived = false;
        int i = 0, j, k;

        //每個bucket做insertion sort
        while(i < players.Count -1) //到最後需要排序的起始點
        {
            int maxIndex = j = k = i;

            while(k < players.Count)
            {
                if (players[k].Data[bucketName].Value != players[j].Data[bucketName].Value) //確認處在同一個bucket
                    break;
                if (Int32.Parse(players[k].Data[bucketContent].Value) > Int32.Parse(players[maxIndex].Data[bucketContent].Value))
                    maxIndex = k;
                k++;
            }
            var temp = players[maxIndex];
            players[maxIndex] = players[j];
            players[j] = temp;

            i++;
        }

        isResponseReceived = true;
        yield return new WaitUntil(() => isResponseReceived);
    }
    IEnumerator DisplaySubList(string searchString)///**
    {
        LoadingManger.Instance.Open_Loading_animator(); //等待搜尋結果
        destroy();  //delete all players previous
        bool isResponseReceived = false;

        Lobby lobby = nowOnlineLobby;   //在 lobby 的改動也會同步到 nowOnlineLobby，不要輕易改動
        var objList = playerListContain.GetComponentsInChildren<TMP_Text>();
        var objList2 = playerListContain.GetComponentsInChildren<Image>();
        List<Unity.Services.Lobbies.Models.Player> players = lobby.Players;

        Debug.Log($"Players in Lobby {lobby.Name} (searchString = \" {searchString} \"): ");
        foreach (Unity.Services.Lobbies.Models.Player player in players)
        {
            if (player.Data["playerName"].Value.Contains(searchString) && player.Data["playerName"].Value != myUserName)
            {
                Debug.Log("DisplaySubList");
                Debug.Log(player.Id + " " + player.Data["playerName"].Value + player.Data["myAvatarUrl"].Value + " " + player.Data["activity"].Value + " " + player.Data["vip"].Value + " " + player.Data["level"].Value + " " + player.Data["DC"].Value);

                objList[0].text = player.Data["playerName"].Value;
                objList[1].text = "活躍度：" + player.Data["activity"].Value;

                yield return urlImageGetter.GettingSprite(player.Data["myAvatarUrl"].Value, "avatar");
                objList2[2].sprite = urlImageGetter.sprite;

                Instantiate(playerListContain, scrollContainer.transform); //生成一個list的prefab
            }
        }

        LoadingManger.Instance.Close_Loading_animator(); //搜尋完成
        isResponseReceived = true;
        yield return new WaitUntil(() => isResponseReceived);
        yield return null;
    }

    private async void LeaveLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
            hostLobby = queryResponse.Results[0];
            print(hostLobby.Id);
            print(AuthenticationService.Instance.PlayerId);
            await LobbyService.Instance.RemovePlayerAsync(hostLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Leave Lobby Success");

            //Debug.Log("List remain Lobbies");
            //ListLobbies();
        }
        catch(LobbyServiceException e)
        {
            Debug.Log("Leave Lobby Error" + e);
        }
    }

    public static string myUserName = "";
    string myAvatarUrl = PlayerInfoManager.Instance.PlayerInfo.avatar.Url;
    int activity;
    int vip;
    int level;
    int DC;
    string playFabID = PlayerInfoManager.Instance.PlayerInfo.playFabId;
    IEnumerator GetPlayerProfile(string playFabId)
    {
        Debug.Log("此玩家ID:" + playFabId);
        PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
        {
            ImageUrl = PlayerInfoManager.Instance.PlayerInfo.avatar.Url
        }, (result) =>
        {
            Debug.Log($"upload avatar url to PlayFab success");            
        }, (error) =>
        {            
            Debug.Log($"update avatar fail: {error}");
        });
        bool isResponseReceived = false;
        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        },
        result => {
            Debug.Log("The player's DisplayName profile data is: " + result.PlayerProfile.DisplayName);
            myUserName = result.PlayerProfile.DisplayName;
            myAvatarUrl = result.PlayerProfile.AvatarUrl.ToString();
            //Debug.Log("myAvatarUrl" + result.PlayerProfile.AvatarUrl.ToString());
            playFabID = result.PlayerProfile.PlayerId;
           isResponseReceived = true;
        },
        error => Debug.LogError("Get player Profile ERROR: " + error.GenerateErrorReport()));
        LoadingManger.Instance.Close_Loading_animator(); //完成add to online list
        yield return new WaitUntil(() => isResponseReceived);

        yield return null;
    }


    private void GetPlayerData(string playFabId)
    {
        StartCoroutine(getUserActivityAndLevel());
        StartCoroutine(getDC());
    }
    IEnumerator getUserActivityAndLevel()
    {
        bool isResponseReceived = false;
        List<string> keys = new List<string>();
        //keys.Add("Activity");
        keys.Add("UserLevel");
        print("enter get player data");

        var request = new PlayFab.ClientModels.GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys,
        };

        PlayFabClientAPI.GetUserReadOnlyData(request, result =>
        {
            Debug.Log("get user level sucsess:" + result.ToString());
            InventoryManager.Instance.UpdateDragonCoin();

           // activity = Int32.Parse(result.Data[keys[0]].Value);

            var playerinfo = PlayerInfoManager.Instance.PlayerInfo;
            vip = playerinfo.vip.level;

            string jsonString = result.Data[keys[0]].Value.ToString();
            jsonString = jsonString.TrimStart('[').TrimEnd(']');

            // 解析 JSON 字符串
            UserData userData = JsonUtility.FromJson<UserData>(jsonString);

            // 访问字段
            Debug.Log("Level: " + userData.Level);
            Debug.Log("Experience: " + userData.Experience);
            Debug.Log("UpgradeDate: " + userData.UpgradeDate);
            Debug.Log("NextLevelExperience: " + userData.NextLevelExperience);
            Debug.Log("eeeeeess:" + result.Data[keys[0]].Value.ToString());

            //level = Int32.Parse(result.Data[keys[0]].Value);
            userLevel.userData = userData;
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("get user level failed: " + error.ErrorMessage);
        });
        yield return new WaitUntil(() => isResponseReceived);

        yield return null;
    }

    IEnumerator getDC()
    {
        bool isResponseReceived = false;
        var request = new GetUserInventoryRequest();
        PlayFabClientAPI.GetUserInventory(request, result =>
        {
            Debug.Log($"result.VirtualCurrency {result.VirtualCurrency.Count}");
            foreach (var item in result.VirtualCurrency)
            {
                Debug.Log($"item.Key {item.Key}");
                if (item.Key == "DC")
                {
                    DC = item.Value;
                }
            }
            isResponseReceived = true;
        }, error =>
        {
            Debug.Log("Error retrieving user inventory: " + error.ErrorMessage);
        }
        );
        yield return new WaitUntil(() => isResponseReceived);
        yield return null;
    }

    public bool DoSearch = false;
    public string TestDisplayName = "HOGARARA";
    private void LateUpdate()
    {
        if(DoSearch == true)
        {
            DoSearch = false;
            GetPlayerIdByDisplayName(TestDisplayName);
        }
    }

}

[System.Serializable]
public class FunctionResultData
{
    public string PlayFabId;
    public string error;
}
[System.Serializable]
public class UserData
{
    public int Level;
    public float Experience=-1;
    public string UpgradeDate;
    public int NextLevelExperience;
}
