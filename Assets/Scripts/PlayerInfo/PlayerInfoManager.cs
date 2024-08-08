using System;
using System.Collections;
using System.Collections.Generic;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UserAccount;
using UserAccount.AccountAndPhone;
using VIPSetting;

namespace Player
{
    /// <summary>
    /// Get PlayerInfo with PlayFabId
    /// </summary>
    public class PlayerInfoManager : MonoBehaviour
    {
        #region Instance(Singleton)
        private static PlayerInfoManager instance;

        public static PlayerInfoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerInfoManager>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject with {typeof(PlayerInfoManager)} does not exist in the scene, " +
                                       $"yet its method is being called.\n" +
                                       $"Please add {typeof(PlayerInfoManager)} to the scene.");
                    }
                    DontDestroyOnLoad(instance);
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }


        #endregion


        #region Variable
        [SerializeField] private UserAccountInfo _accountInfo;
        public UserAccountInfo AccountInfo => _accountInfo;
        public PlayerInfo PlayerInfo => _playerInfo;
        [SerializeField] private PlayerInfo _playerInfo;
        public string PlayFabId => _playerInfo.playFabId;

        [SerializeField] private PhoneInfo phoneInfo;
        public PhoneInfo PhoneInfo => phoneInfo;


        [SerializeField] private GetPlayerCombinedInfoResultPayload _combinedInfoResultPayload;
        #endregion


        [SerializeField] private PlayerInfoBuilder localPlayerBuilder;
        [SerializeField] private VipIconData _vipIconData;


        
        #region Event

        public static readonly UnityEvent<PlayerInfo> OnPlayerInfoChange = new UnityEvent<PlayerInfo>();

        public static readonly UnityEvent<int, Sprite> OnVipChanged = new UnityEvent<int, Sprite>();
        public static readonly UnityEvent<int, decimal> OnUserLevelChanged = new UnityEvent<int, decimal>();

        #endregion


        #region Subscribe Event
        void OnEnable()
        {
            UserAccountManager.OnLoginSuccessful.AddListener(BuildPlayerInfo);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            UserAccountManager.OnLoginSuccessful.RemoveListener(BuildPlayerInfo);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Call all UI to update local player info on scene loaded
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnPlayerInfoChange.Invoke(PlayerInfo);
        }

        #endregion


        #region Build Player Info

        private void BuildPlayerInfo(LoginResult result)
        {
            LoadingManger.Instance.Open_Loading_animator();
            if (result == null)
            {
                return;
            }
            localPlayerBuilder.OnInfoBuilded.AddListener(OnBuildLocalPlayerSuccess);
            localPlayerBuilder.OnGetCombinedResult.AddListener(SetCombineInfo);
            localPlayerBuilder.BuildPlayerInfo(result.PlayFabId);

            VipManager.Instance.Initialization();
        }

        private void OnBuildLocalPlayerSuccess(PlayerInfo playerInfo)
        {
            localPlayerBuilder.OnInfoBuilded.RemoveListener(OnBuildLocalPlayerSuccess);
            _playerInfo = playerInfo;
            Debug.Log($"Build Local Player Info Success {_playerInfo}");
            OnPlayerInfoChange.Invoke(_playerInfo);
            InventoryManager.OnDragonCoinChange.Invoke(_playerInfo.dragonCoin);
            InventoryManager.OnDragonBallChange.Invoke(_playerInfo.dragonBall);

            StartCoroutine(UpdateCoroutine());
        }
        
        public static void UpdateIngame()
        {
            Instance.StartCoroutine(Instance.UpdateCoroutine());
        }

        private IEnumerator UpdateCoroutine()
        {
            yield return UpdateVip();

            yield return UpdateUserLevel();

            yield return GetPhoneInfoCoroutine();

            LoadingManger.Instance.Close_Loading_animator();
        }

        private void SetCombineInfo(GetPlayerCombinedInfoResultPayload info)
        {
            localPlayerBuilder.OnGetCombinedResult.RemoveListener(SetCombineInfo);
            _combinedInfoResultPayload = info;
            _accountInfo = info?.AccountInfo;
        }

        /// <summary>
        /// 是否已經綁定門號
        /// </summary>
        /// <returns></returns>
        public bool IsAccountBound()
        {
            return !string.IsNullOrEmpty(AccountInfo.Username);
        }



        #endregion

        #region Get VIP Right
        public VipRight GetVipRight()
        {
            int level = _playerInfo.vip.level;
            return VipManager.Instance.GetVipRight(level);
        }
        #endregion


        #region Update Local Player Info

        /// <summary>
        /// Change When Vip Right may update
        /// </summary>
        [ContextMenu("Update VIP")]
        public IEnumerator UpdateVip()
        {
            Debug.Log("更新VIP資訊");
            bool isResponseReceived = false;

            ExecuteFunctionRequest cloudFunction = new ExecuteFunctionRequest()
            {
                FunctionName = "UpdateVIP",
                FunctionParameter = new { playFabId = PlayerInfoManager.Instance.PlayFabId },
                GeneratePlayStreamEvent = true
            };

            PlayFabCloudScriptAPI.ExecuteFunction(cloudFunction,
                (result) =>
                {
                    if (result != null && result.FunctionResult != null)
                    {
                        int vipLevel = Convert.ToInt32(result.FunctionResult.ToString());
                        PlayerInfo.vip.level = vipLevel;
                        PlayerInfo.vipIcon = _vipIconData.GetIcon(vipLevel);
                        OnVipChanged.Invoke(vipLevel, PlayerInfo.vipIcon);
                        OnPlayerInfoChange.Invoke(PlayerInfo);
                    }
                    else
                    {
                        Debug.LogError("CloudScript function did not return a valid result.");
                    }

                    isResponseReceived = true;
                },
                (error) =>
                {
                    Debug.LogError("CloudScript function failed with error: " + error.GenerateErrorReport());
                    isResponseReceived = true;
                });

            yield return new WaitUntil(() => isResponseReceived);
        }

        public IEnumerator UpdateUserLevel()
        {
            bool isResponseReceived = false;

            List<string> userReadOnlyDataKeys = new List<string> { "NewUserLevel", "TotalBet", "UserLevel" };
            GetUserDataRequest userLevelRequest = new GetUserDataRequest { Keys = userReadOnlyDataKeys };

            PlayFabClientAPI.GetUserData(userLevelRequest,
                (result) =>
                {

                    if (result != null && !result.Data.ContainsKey(userReadOnlyDataKeys[0]))
                    {
                        PlayFabClientAPI.GetUserReadOnlyData(userLevelRequest,
                             (result) =>
                             {
                                 string jsonString = result.Data[userReadOnlyDataKeys[0]].Value;
                                 jsonString = jsonString.TrimStart('[').TrimEnd(']');

                                 // 解析 JSON 字符串
                                 UserData userData = JsonUtility.FromJson<UserData>(jsonString);

                                 Debug.Log($"User Level: {userData.Experience}");
                                 PlayerInfo.userLevel.Level = userData.Level;
                                 PlayerInfo.userLevel.Experience = (int)userData.Experience;
                             },
                            (error) =>
                            {
                                Debug.LogError("Get User Level failed with error: " + error.GenerateErrorReport());
                                isResponseReceived = true;
                            });




                    }
                    else
                    {
                        string jsonString = result.Data[userReadOnlyDataKeys[0]].Value;
                        jsonString = jsonString.TrimStart('[').TrimEnd(']');

                        // 解析 JSON 字符串
                        UserData userData = JsonUtility.FromJson<UserData>(jsonString);

                        Debug.Log($"User Level: {userData.Experience}");
                        PlayerInfo.userLevel.Level = userData.Level;
                        PlayerInfo.userLevel.Experience = (int)userData.Experience;
                    }

                    //if (result != null && result.Data.TryGetValue(userReadOnlyDataKeys[1], out var totalBet))
                    //{
                    //    PlayerInfo.userLevel.Experience = decimal.Parse(totalBet.Value);
                    //    // Debug.Log($"Experience: {PlayerInfo.userLevel.Experience}");
                    //}
                    //else
                    //{
                    //    Debug.LogWarning($"Player Data Key - {userReadOnlyDataKeys[1]} not found.");
                    //}

                    OnUserLevelChanged.Invoke(PlayerInfo.userLevel.Level, PlayerInfo.userLevel.Experience);
                    isResponseReceived = true;
                },
                (error) =>
                {
                    Debug.LogError("Get User Level failed with error: " + error.GenerateErrorReport());
                    isResponseReceived = true;
                });

            yield return new WaitUntil(() => isResponseReceived);
        }

        private IEnumerator GetPhoneInfoCoroutine()
        {
            var phoneInfoGetter = new PhoneInfoGetter();
            yield return phoneInfoGetter.GetPhoneInfoCoroutine();
            phoneInfo = phoneInfoGetter.GetPhoneInfo();
        }

        #endregion


        #region Set Local Player Info

        public void SetLocalNickName(string newName)
        {
            PlayerInfo.userTitleInfo.DisplayName = newName;
            OnPlayerInfoChange?.Invoke(PlayerInfo);
        }

        public void SetLocalMessageBoards(List<MessageBoard> addMessageBoards)
        {
            PlayerInfo.messageBoards = addMessageBoards;
            OnPlayerInfoChange?.Invoke(PlayerInfo);
        }

        public void SetLocalChangeNickNameRecord(ChangeNameRecord changeNameRecord)
        {
            PlayerInfo.changeNameRecord = changeNameRecord;
            OnPlayerInfoChange?.Invoke(PlayerInfo);
        }

        public void SetLocalAvatar(Avatar avatar)
        {
            PlayerInfo.avatar = avatar;
            OnPlayerInfoChange?.Invoke(PlayerInfo);
        }

        public void SetLocalUserName(string userName)
        {
            _accountInfo.Username = userName;
            OnPlayerInfoChange?.Invoke(PlayerInfo);
        }

        public void SetLocalFacebookId(string facebookId)
        {
            if (_accountInfo.FacebookInfo == null)
            {
                _accountInfo.FacebookInfo = new UserFacebookInfo();
            }

            _accountInfo.FacebookInfo.FacebookId = facebookId;
        }

        public void SetLocalAppleId(string appleId)
        {
            if (_accountInfo.AppleAccountInfo == null)
            {
                _accountInfo.AppleAccountInfo = new UserAppleIdInfo();
            }

            _accountInfo.AppleAccountInfo.AppleSubjectId = appleId;
        }

        public void SetLocalGoogleId(string googleId)
        {
            if (_accountInfo.GoogleInfo == null)
            {
                _accountInfo.GoogleInfo = new UserGoogleInfo();
            }

            _accountInfo.GoogleInfo.GoogleId = googleId;
        }



        public void SetDragonCoin(int coin)
        {
            _playerInfo.dragonCoin = coin;
            InventoryManager.OnDragonCoinChange.Invoke(coin);
        }

        public void SetDragonBall(int ball)
        {
            _playerInfo.dragonBall = ball;
            InventoryManager.OnDragonBallChange.Invoke(ball);
        }

        public void SetLocalLevel(int playerLevel, decimal totalBet)
        {
            PlayerInfo.userLevel.Level = playerLevel;
            PlayerInfo.userLevel.Experience = totalBet;
            // OnPlayerInfoChange?.Invoke(PlayerInfo);
        }


        #endregion
    }
}