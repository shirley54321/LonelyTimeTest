using System.Collections.Generic;
using System.Linq;
using Games.Data;
using LitJson;
using Loading;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using UnityEngine;
using UnityEngine.Events;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles the retrieval and processing of machine lists for a specific game and hall.
    /// </summary>
    public class FetchMachineListHandler : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when the request to fetch machine list is initiated.
        /// </summary>
        public static readonly UnityEvent OnFetchMachineList = new UnityEvent();
        
        /// <summary>
        /// Event triggered when the machine list is successfully retrieved.
        /// </summary>
        public static readonly UnityEvent<MachineList> OnRecievedMachineList = new UnityEvent<MachineList>();

        /// <summary>
        /// Represents the current machine list.
        /// </summary>
        public static MachineList _machineList;

        [System.Serializable]
        public class MachineData
        {
            public string MachineId;
            public int GameId;
            public int Hall;     
        }
        /// <summary>
        /// Initiates the process to retrieve the machine list for a specific game and hall.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="hall">The hall for which the machine list is requested.</param>
        public void FetchMachineList(GameId gameId, Hall hall)
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            OnFetchMachineList.Invoke();
            // Construct the CloudScript request to retrieve machine list
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "GetMachineList",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"gameId" , gameId},
                    {"hall", hall}
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

                    MachineData machineData = JsonUtility.FromJson<MachineData>(result.FunctionResult.ToString());

                    // Deserialize the CloudScript response into _machineList object
                    _machineList = JsonMapper.ToObject<MachineList>(result.FunctionResult.ToString());

                    ChangeHallHandler.OnChangeHallSuccess.Invoke();
 
                    GetWinnerPlayerName();
            
                }, (PlayFabError error) =>
                {
                    Debug.Log($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                    // Close Loading Screen
                    LoadingManger.Instance.Close_Loading_animator();
                    ServerEventHandler.Call_Server_Error_Event(error);
                });
        }
        
        /// <summary>
        /// Retrieves the display name of the winner player from PlayFab.
        /// </summary>
        private void GetWinnerPlayerName()
        {
            var winnerPlayFabId = _machineList.HistoryRecord.WinnerPlayFabId;
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                PlayFabId =  winnerPlayFabId,
            }, infoResult =>
            {
                _machineList.HistoryRecord.WinnerName = infoResult.PlayerProfile.DisplayName;
                // Invoke the event to notify listeners that machine list is updated
                
                MachineLobbyMediator.Instance.SetMachineList(_machineList);
                OnRecievedMachineList.Invoke(_machineList);
                
                // Close Loading Screen
                LoadingManger.Instance.Close_Loading_animator();
            }, error =>
            {
                Debug.Log($"Fail to get display name from {winnerPlayFabId}");
                
                MachineLobbyMediator.Instance.SetMachineList(_machineList);
                OnRecievedMachineList.Invoke(_machineList);
                
                // Close Loading Screen
                LoadingManger.Instance.Close_Loading_animator();
            });
        }
    }
}