using System.Collections.Generic;
using System.Linq;
using Games.Data;
using LitJson;
using Player;
using PlayFab;
using PlayFab.CloudScriptModels;
using Share.Azure;
using UnityEngine;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles player's machine reservation.
    /// </summary>
    public class MachineReservedHandler : MonoBehaviour
    {

        private PlayerReserve _playerReserve;
        /// <summary>
        /// Gets the player's machine reservation.
        /// </summary>
        public PlayerReserve PlayerReserve => _playerReserve;


        public void SetPlayerReserve(PlayerReserve playerReserve)
        {
            _playerReserve = playerReserve;
        }

        #region For Enter Machine

        /// <summary>
        /// 檢查玩家是否有保留中的機台
        /// </summary>
        /// <returns>True if there are reserved machines; false otherwise.</returns>
        public bool HasReservedMachines()
        {
            return PlayerReserve.reserveMachines.Count > 0;
        }

        /// <summary>
        /// 現在選擇的機台，是否為玩家保留中的機台
        /// </summary>
        /// <returns>True if the machine is reserved; false otherwise.</returns>
        public bool IsInReservedMachines()
        {
            foreach (var machineInfo in _playerReserve.reserveMachines)
            {
                if (machineInfo.MachineId == MachineLobbyMediator.Instance.SelectedMachineId)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 玩家在這個遊戲中，是否有保留的機台.
        /// </summary>
        /// <param name="reserveMachine">The reserved machine info.</param>
        /// <returns>True if there is a reserved machine; false otherwise.</returns>
        public bool HasReservedMachineInCurrentGame(out MachineInfo reserveMachine)
        {
            foreach (var machineInfo in PlayerReserve.reserveMachines)
            {
                if (machineInfo.GameId == MachineLobbyMediator.Instance.SelectedGameId)
                {
                    reserveMachine = machineInfo;
                    return true;
                }
            }

            reserveMachine = null;
            return false;
        }
        
        /// <summary>
        /// Cancels the current game reservation for the selected machine.
        /// </summary>
        public void CancelCurrentGameReserve()
        {
            Debug.Log("CancelCurrentGameReserve");
            // Construct the CloudScript request to leave the machine
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "CancelCurrentGameReserve",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"gameId", (int)MachineLobbyMediator.Instance.SelectedGameId},
                    {"machineId", GetReservedMachineIdInCurrentGame()}
                },
            };

            // Execute the CloudScript function to leave the machine
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log(
                            "This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    var functionResult = JsonMapper.ToObject<FunctionResult>(result.FunctionResult.ToString());
                    if (!functionResult.success)
                    {
                        Debug.LogError($" {result.FunctionName}  Fail, {functionResult.errorMessage}");
                    }

                    RemoveLocalReservedMachine();
                    
                },
                (PlayFabError error) =>
                {
                    Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }
        
        #endregion
        
        
        
        /// <summary>
        /// Gets the ID of the reserved machine in the current game.
        /// </summary>
        /// <returns>The ID of the reserved machine or -1 if none.</returns>
        private int GetReservedMachineIdInCurrentGame()
        {
            if (HasReservedMachineInCurrentGame(out Data.MachineInfo reserveMachine))
            {
                return reserveMachine.MachineId;
            }
            else
            {
                return -1;
            }
        }
        
        /// <summary>
        /// Removes the locally reserved machine.
        /// </summary>
        private void RemoveLocalReservedMachine()
        {
            Data.MachineInfo needRemoveMachine = null;
            foreach (var machineInfo in PlayerReserve.reserveMachines.Where(
                machineInfo => machineInfo.GameId == MachineLobbyMediator.Instance.SelectedGameId))
            {
                needRemoveMachine = machineInfo;
            }

            PlayerReserve.reserveMachines.Remove(needRemoveMachine);
        }

        
        /// <summary>
        /// Checks if a machine is in an idle state or reserved by the current player.
        /// </summary>
        /// <param name="machineInfo">The machine info to check.</param>
        /// <returns>True if the machine is in an eligible state; false otherwise.</returns>
        public bool IsEnabledPlayedMachine(MachineInfo machineInfo)
        {
            return machineInfo.ReserveMachineInfo.State == MachineState.Idle ||
                   IsPlayingOrReservedByCurrentPlayer(machineInfo);
        }

        /// <summary>
        /// Checks if a machine is being played or reserved by the current player.
        /// </summary>
        /// <param name="machineInfo">The machine info to check.</param>
        /// <returns>True if the machine is being played or reserved by the current player; false otherwise.</returns>
        private bool IsPlayingOrReservedByCurrentPlayer(MachineInfo machineInfo)
        {
            return machineInfo.ReserveMachineInfo.PlayFabId == PlayerInfoManager.Instance.PlayFabId;
        }
    }
}
