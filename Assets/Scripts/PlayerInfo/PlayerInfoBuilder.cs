using System.Collections;
using System.Collections.Generic;
using LitJson;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Tool;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerInfoBuilder : MonoBehaviour
    {
        

        #region Tool
        [SerializeField] private bool printUserData;
        [SerializeField] private VipIconData vipIconData;
        [SerializeField] private AvatarManager avatarManager;
        
        #endregion

        #region Variable

        [SerializeField] private GetPlayerCombinedInfoRequestParams getInfoParams;
        [SerializeField] private GetPlayerCombinedInfoResultPayload infoResult;
        
        [SerializeField] private PlayerInfo buildingPlayerInfo;
        [SerializeField] private bool getAvatarFinish;

        #endregion

        #region Event

        public UnityEvent<PlayerInfo> OnInfoBuilded = new UnityEvent<PlayerInfo>();

        public UnityEvent<GetPlayerCombinedInfoResultPayload> OnGetCombinedResult =
            new UnityEvent<GetPlayerCombinedInfoResultPayload>();
        
        #endregion
        
        
        #region Build Player Info
        
        /// <summary>
        /// Get PlayerInfo with PlayFabId
        /// </summary>
        /// <param name="playFabId"></param>
        /// <returns></returns>
        public void BuildPlayerInfo(string playFabId)
        {
            buildingPlayerInfo = new PlayerInfo();
            
            PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest()
            {
                InfoRequestParameters = getInfoParams,
                PlayFabId = playFabId
            },result =>
            {
                OnGetCombinedResult.Invoke(result.InfoResultPayload);
                StartCoroutine(BuildPlayerInfoCoroutine(result.InfoResultPayload));

            }, error => {
                Debug.Log ($"Could not retrieve user data | {error.ErrorMessage}");
            });
        }

        public void BuildOtherPlayerInfo(string playFabId)
        {
            buildingPlayerInfo = new PlayerInfo();

            
            PlayFabClientAPI.GetPlayerCombinedInfo(new GetPlayerCombinedInfoRequest()
            {
                InfoRequestParameters = getInfoParams,
                PlayFabId = playFabId
            },result =>
            {
               GetOtherPlayerCurrency(playFabId, result.InfoResultPayload);

            }, error => {
                Debug.Log ($"Could not retrieve user data | {error.ErrorMessage}");
            });

        }

        private void GetOtherPlayerCurrency(string playFabId, GetPlayerCombinedInfoResultPayload combinedInfo)
        {
            // Construct the CloudScript request to retrieve machine list
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "GetPlayerVirtualCurrency",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"playFabId" , playFabId },
                },
            };
            // Execute the CloudScript function to get machine list
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest, 
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }
                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    Debug.Log($"Result: {result.FunctionResult.ToString()}");
            
                    // Deserialize the CloudScript response into MachineList object
                    var currency = JsonMapper.ToObject<Dictionary<string,int>>(result.FunctionResult.ToString());
                    combinedInfo.UserVirtualCurrency = currency;
                    StartCoroutine(BuildPlayerInfoCoroutine(combinedInfo));
            
                }, (PlayFabError error) =>
                {
                    Debug.Log($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }
        

        private IEnumerator BuildPlayerInfoCoroutine(GetPlayerCombinedInfoResultPayload info)
        {
            buildingPlayerInfo = BuildPlayerInfoWithoutAvatar(info);
            
            // Get Player Avatar
            avatarManager.OnGetAvatarWithUrl.AddListener(OnGainAvatar);
            getAvatarFinish = false;
            avatarManager.GetAvatar(info.AccountInfo.TitleInfo.AvatarUrl);
            yield return new WaitUntil(()=> getAvatarFinish);
            
            OnInfoBuilded.Invoke(buildingPlayerInfo);
            // Close Loading Screen
            
        }

        private void OnGainAvatar(Avatar avatar)
        {
            buildingPlayerInfo.avatar = avatar;
            avatarManager.OnGetAvatarWithUrl.RemoveListener(OnGainAvatar);
            getAvatarFinish = true;
        }

        

        

        public PlayerInfo BuildPlayerInfoWithoutAvatar(GetPlayerCombinedInfoResultPayload result)
        {
            infoResult = result;
            
            buildingPlayerInfo = new PlayerInfo
            {
                playFabId = infoResult.AccountInfo.PlayFabId,
                userTitleInfo = infoResult.AccountInfo.TitleInfo
            };

            // Player Data
            buildingPlayerInfo.messageBoards = PlayFabDataBuilder.BuildData<List<MessageBoard>>(
                $"{nameof(MessageBoard)}",  result.UserData, PlayFabDataType.PlayerData);
            buildingPlayerInfo.changeNameRecord = PlayFabDataBuilder.BuildData<ChangeNameRecord>(
                $"{nameof(ChangeNameRecord)}", result.UserData, PlayFabDataType.PlayerData);
            
            // Read Only Data
            buildingPlayerInfo.userProfile = PlayFabDataBuilder.BuildData<UserProfile>(
                $"{nameof(UserProfile)}", result.UserReadOnlyData, PlayFabDataType.ReadOnlyData);

            buildingPlayerInfo.activity = PlayFabDataBuilder.BuildDataInt(
                "Activity", result.UserReadOnlyData, PlayFabDataType.ReadOnlyData);
            buildingPlayerInfo.vip = PlayFabDataBuilder.BuildLastElementInList<VIP>(
                $"{nameof(VIP)}", result.UserReadOnlyData, PlayFabDataType.ReadOnlyData);
            buildingPlayerInfo.userLevel = PlayFabDataBuilder.BuildLastElementInList<UserLevel>(
                $"{nameof(UserLevel)}",  result.UserReadOnlyData, PlayFabDataType.ReadOnlyData);     
            // buildingPlayerInfo.activityEvent = PlayFabDataBuilder.BuildData<ActivityEvent>(
            //     $"{nameof(ActivityEvent)}",  result.UserReadOnlyData, PlayFabDataType.ReadOnlyData);

            if (printUserData)
            {
                foreach (var pair in result.UserReadOnlyData)
                {
                    Debug.Log($"ReadOnly: {pair.Key}, {pair.Value.Value}");
                }

                foreach (var pair in result.UserData)
                {
                    Debug.Log($"UserData: {pair.Key}, {pair.Value.Value}");
                }
            }
            
            // Coin
            var userVirtualCurrency = result.UserVirtualCurrency;
            if (userVirtualCurrency != null)
            {
                if (userVirtualCurrency.TryGetValue("DC", out var coin))
                {
                    buildingPlayerInfo.dragonCoin = coin;
                }

                if (userVirtualCurrency.TryGetValue("DB", out var ball))
                {
                    buildingPlayerInfo.dragonBall = ball;
                }
            }
                
            // Vip Sprite
            if (buildingPlayerInfo.vip != null)
            {
                buildingPlayerInfo.vipIcon = vipIconData.GetIcon(buildingPlayerInfo.vip.level);
            }
            
            return buildingPlayerInfo;
        }
        
        

        #endregion
    }
}