using System.Collections;
using System.Collections.Generic;
using Games.Data;
using LitJson;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UserAccount;

namespace Games.Tool
{
    public class GameInfoProvider : MonoBehaviour
    {
        [SerializeField] private List<GameProfile> gameProfileList;

        private bool gameProfileListHasFetched = false;
        private void Start()
        {
            StartCoroutine(FetchGameProfile());
        }
        
        public string GetGameName(GameId gameId)
        {
            if (!gameProfileListHasFetched)
            {
                Debug.LogError("gameProfileList hasn't fetched, but try to get data");
            }
            
            foreach (var gameProfile in gameProfileList)
            {
                if (gameProfile.GameId == gameId)
                {
                    return gameProfile.GameName;
                }
            }

            Debug.LogError($"Can't Find game name for {gameId}");
            return "";
        }

        public GameProfile GetGameProfile(GameId gameId)
        {
            if (!gameProfileListHasFetched)
            {
                Debug.LogError("gameProfileList hasn't fetched, but try to get data");
            }
            
            foreach (var gameProfile in gameProfileList)
            {
                if (gameProfile.GameId == gameId)
                {
                    return gameProfile;
                }
            }

            Debug.LogError($"Can't Find game name for {gameId}");
            return null;
        }

        public string GetHallName(Hall hall)
        {
            switch (hall)
            {
                case Hall.Beginner:
                    return "體驗廳";
                case Hall.General:
                    return "一般廳";
                case Hall.Honor:
                    return "尊榮聽";
                case Hall.Master:
                    return "高手聽";
            }

            Debug.LogError($"Can't Find hall name for {hall}");
            return "";
        }
        
        /// <summary>
        /// Fetch Game Profile if gameProfile has not fetch from PlayFab
        /// </summary>
        private IEnumerator FetchGameProfile()
        {
            yield return new WaitUntil(()=>UserAccountManager.Instance.HaveLogin());
            
            
            // Construct the CloudScript request to retrieve machine list
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "GetGameProfileList"
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
                    gameProfileList = JsonMapper.ToObject<List<GameProfile>>(result.FunctionResult.ToString());
                    gameProfileListHasFetched = true;
                }, (PlayFabError error) =>
                {
                    Debug.Log($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }
    }
}