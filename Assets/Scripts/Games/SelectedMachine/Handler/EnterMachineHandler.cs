using System;
using System.Collections.Generic;
using System.Linq;
using Games.Data;
using Games.SelectedMachine.Enum;
using LitJson;
using Loading;
using Player;
using PlayFab;
using PlayFab.CloudScriptModels;
using Share.Azure;
using SlotTemplate;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles the process of entering a machine, including random selection and reservation.
    /// </summary>
    public class EnterMachineHandler : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when attempting to enter a machine fails.
        /// </summary>
        public static readonly UnityEvent<EnterMachineFailCode> TryEnterMachineFail = new UnityEvent<EnterMachineFailCode>();

        /// <summary>
        /// Event triggered when a reservation exists for the selected machine in the current game.
        /// </summary>
        public static readonly UnityEvent<Data.MachineInfo> OnHaveReserveInCurrentGame = new UnityEvent<Data.MachineInfo>();

        private Data.MachineInfo _enterMachine;

        /// <summary>
        /// Handles player's machine reservation.
        /// </summary>
        private MachineReservedHandler machineReservedHandler;

        private void Start()
        {
            machineReservedHandler = MachineLobbyMediator.Instance.MachineReservedHandler;
        }


        /// <summary>
        /// Randomly selects an eligible machine from the provided machine list and initiates the enter machine process.
        /// </summary>
        /// <param name="machineInfos">The list of machines to choose from.</param>
        public void RandomEnterMachine(List<MachineInfo> machineInfos)
        {
            // Get eligible machines that have an Idle state from the MachineList.
            var eligibleMachines = machineInfos
                .Where(machine => machine.ReserveMachineInfo.State == MachineState.Idle
                                  || machine.ReserveMachineInfo.PlayFabId == PlayerInfoManager.Instance.PlayFabId)
                .ToList();

            // Check if there are eligible machines.
            if (eligibleMachines.Count > 0)
            {
                // Generate a random index to select a random element.
                int randomIndex = Random.Range(0, eligibleMachines.Count);

                // Select the random element.
                var randomMachine = eligibleMachines[randomIndex];

                // Now, randomMachine contains the randomly selected eligible element.
                Debug.Log($"Randomly selected machine ID: {randomMachine.MachineId}, Number {randomMachine.MachineNumber}");

                MachineLobbyMediator.Instance.SetSelectedMachine(randomMachine);
                StartEnterMachineProcess(randomMachine);
            }
            else
            {
                Debug.Log("No machines meet the criteria.");
            }
        }

        #region Enter Machine Process

        /// <summary>
        /// Initiates the process of entering the specified machine.
        /// </summary>
        /// <param name="machineInfo">The machine to enter.</param>
        public void StartEnterMachineProcess(Data.MachineInfo machineInfo)
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            _enterMachine = machineInfo;

            // 檢查玩家是否有保留中的機台
            if (!machineReservedHandler.HasReservedMachines())
            {
                TryEnterMachine();
                Debug.Log("No Reserve Machine");
                return;
            }

            // 要進入的機台，是否為玩家保留中的機台
            if (machineReservedHandler.IsInReservedMachines())
            {
                TryEnterMachine();
                Debug.Log("Enter Reserve Machine");
                return;
            }

            // 玩家在這個遊戲中，是否有保留的機台
            if (machineReservedHandler.HasReservedMachineInCurrentGame(out var reserveMachine))
            {
                Debug.Log("Have Reserve Machine In CurrentGame");
                OnHaveReserveInCurrentGame.Invoke(reserveMachine);
                
                // Close Loading Screen
                LoadingManger.Instance.Close_Loading_animator();
            }
            else
            {
                TryEnterMachine();
            }
        }

        /// <summary>
        /// Attempts to enter the selected machine by calling a CloudScript function.
        /// </summary>
        private void TryEnterMachine()
        {
            // Construct the CloudScript request to retrieve machine list
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "TryEnterMachine",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"machineId" , _enterMachine.MachineId},
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

                    
                    
                    // Deserialize the CloudScript response into MachineList object
                    var enterSuccess = Convert.ToBoolean(result.FunctionResult.ToString());
                    if (enterSuccess)
                    {
                        // Close Loading Screen
                        LoadingManger.Instance.Close_Loading_animator();
                        LoadGameScene();
                    }
                    else
                    {
                        OnEnterMachineFail(EnterMachineFailCode.HaveBePlayingOrReserved);
                        Debug.Log($"Enter Machine Fail.\n" +
                                  $" {_enterMachine.MachineNumber}");
                    }

                }, (PlayFabError error) =>
                {
                    OnEnterMachineFail(EnterMachineFailCode.ServerError);
                    Debug.Log($"Enter Machine Fail.\n" +
                              $" {_enterMachine.MachineNumber}");
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }
        
        /// <summary>
        /// Handles the event when entering a machine fails.
        /// </summary>
        private void OnEnterMachineFail(EnterMachineFailCode failCode)
        {
            // Close Loading Screen
            LoadingManger.Instance.Close_Loading_animator();
            
            TryEnterMachineFail.Invoke(failCode);
            MachineLobbyMediator.Instance.FetchMachineList();
        }
        
        /// <summary>
        /// Enters the selected machine lobby, loading the associated game scene.
        /// </summary>
        public void LoadGameScene()
        {
            // TODO : 切換場景
            string sceneName = MachineLobbyMediator.Instance.GetGameMedia().sceneName;
            LoadingManger.instance.SetLoadingGameScreen(sceneName);
        }

        #endregion
    }
}
