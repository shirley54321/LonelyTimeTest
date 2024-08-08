using System;
using System.Collections;
using System.Collections.Generic;
using Games.SelectedMachine.ReserveMachine;
using LitJson;
using Player;
using PlayFab;
using PlayFab.CloudScriptModels;
using Share.Azure;
using SlotTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using UserAccount;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles leaving the selected machine lobby, including checking reservations, spin count, and executing CloudScript functions.
    /// </summary>
    public class LeaveMachineHandler : MonoBehaviour
    {
        [SerializeField] private ReserveMachinePanel _reserveMachinePanel;
        [SerializeField] private CancelEarliestReserveThenReservePanel _cancelEarliestReserveThenReservePanel;

        [SerializeField] private LeaveMachinePanel leaveMachinePanel;

        public int spinCount = 1000;
        public int needSpinCount = 200;

        private MachineReservedHandler _machineReservedHandler;

        private void Start()
        {
            _machineReservedHandler = MachineLobbyMediator.Instance.MachineReservedHandler;
        }

        /// <summary>
        /// Initiates the process of leaving the machine lobby.
        /// </summary>
        [ContextMenu("TryLeaveMachineProcess")]
        public void TryLeaveMachineProcess()
        {
            StartCoroutine(LeaveMachineCoroutine());
            InventoryManager.Instance.UpdateDragonCoin();
            SlotTemplate.PlayerStatus.Instance.UpdateVirtualCoin();
            GameStatUpdate.Instance.ElementSet.Add($"[{string.Join(",", GameStatUpdate.Instance.Spinstatus)}]");
            GameStatUpdate.Instance.ElementSet.Add(MachineLobbyMediator.Instance.SelectedMachineId.ToString());
            GameStatUpdate.Instance.AzureUpdateGameState(1, GameStatUpdate.Instance.ElementSet);
        }

        private IEnumerator LeaveMachineCoroutine()
        {
            if (_machineReservedHandler.IsInReservedMachines())
            {
                LeaveMachine();
            }
            else
            {
                var vipRight = PlayerInfoManager.Instance.GetVipRight();
                var reserveMachineCount = vipRight.ReserveMachineCount;

                if (reserveMachineCount > 0) // Vip level can reserve machine
                {
                    // Wait for obtaining spin count from Azure Function
                    // TODO : Fetch Spin Count by Azure function
                    // yield return FetchSpinCount();
                    yield return null;

                    if (EnoughSpinsCount())
                    {
                        CheckReserveCountThenShowPanel();
                    }
                    else
                    {
                        leaveMachinePanel.ShowPanel(needSpinCount - spinCount);
                    }
                }
                else // Vip level can't reserve machine
                {
                    LeaveMachine();
                }
            }
        }

        /// <summary>
        /// Checks if the spin count is sufficient for reserving a machine.
        /// </summary>
        /// <returns>True if the spin count is enough; false otherwise.</returns>
        private bool EnoughSpinsCount()
        {
            return spinCount >= needSpinCount;
        }

        /// <summary>
        /// Fetches the spin count from an Azure Function.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        private IEnumerator FetchSpinCount()
        {
            bool isResponseReceived = false;

            // Construct the CloudScript request to retrieve spin count
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "GetSlotCount",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"machineId" , MachineLobbyMediator.Instance.SelectedMachineId},
                },
            };

            // Execute the CloudScript function to get spin count
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }
                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");

                    // Deserialize the CloudScript response into spin count
                    spinCount = Convert.ToInt32(result.FunctionResult.ToString());

                    isResponseReceived = true;

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }

        /// <summary>
        /// Checks the reserve count and shows the corresponding panel.
        /// </summary>
        private void CheckReserveCountThenShowPanel()
        {
            var vipRight = PlayerInfoManager.Instance.GetVipRight();
            var canReserveMachineCount = vipRight.ReserveMachineCount;

            var playerReserve = _machineReservedHandler.PlayerReserve;
            var haveReserveMachineCount = playerReserve.reserveMachines.Count;
            var stillCanReserveCount = canReserveMachineCount - haveReserveMachineCount;

            if (ReserveCountReachLimit())
            {
                _cancelEarliestReserveThenReservePanel.ShowPanel(playerReserve, haveReserveMachineCount, stillCanReserveCount);
            }
            else
            {
                _reserveMachinePanel.ShowPanel(stillCanReserveCount);
            }
        }

        /// <summary>
        /// Checks if the reserve count reaches the limit.
        /// </summary>
        /// <returns>True if the reserve count reaches the limit; false otherwise.</returns>
        private bool ReserveCountReachLimit()
        {
            var haveReserveCount = _machineReservedHandler.PlayerReserve.reserveMachines.Count;
            var canReserveCount = PlayerInfoManager.Instance.GetVipRight().ReserveMachineCount;

            return haveReserveCount >= canReserveCount;
        }

        #region For Remind Panel

        /// <summary>
        /// Initiates the process of cancelling the earliest reservation and then reserving a machine.
        /// </summary>
        [ContextMenu("CancelEarliestReserveThenReserve")]
        public void CancelEarliestReserveThenReserve()
        {
            Debug.Log("CancelEarliestReserveThenReserve");
            // Construct the CloudScript request to cancel the earliest reservation and then reserve
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "CancelEarliestReserveThenReserve",
                FunctionParameter = new Dictionary<string, object>()
                {
                    { "machineId", MachineLobbyMediator.Instance.SelectedMachineId },
                    {"vipLevel" , PlayerInfoManager.Instance.PlayerInfo.vip.level}
                },
            };

            // Execute the CloudScript function to cancel the earliest reservation and then reserve
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    var functionResult = JsonMapper.ToObject<FunctionResult>(result.FunctionResult.ToString());
                    if (!functionResult.success)
                    {
                        Debug.LogError($" {result.FunctionName}  Fail, {functionResult.errorMessage}");
                    }
                },
                (PlayFabError error) =>
                {
                    Debug.LogError($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }

        /// <summary>
        /// Initiates the process of reserving a machine.
        /// </summary>
        [ContextMenu("ReserveMachine")]
        public void ReserveMachine()
        {
            // Construct the CloudScript request to reserve a machine
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "ReserveMachine",
                FunctionParameter = new Dictionary<string, object>()
                {
                    { "machineId", MachineLobbyMediator.Instance.SelectedMachineId },
                    {"vipLevel" , PlayerInfoManager.Instance.PlayerInfo.vip.level}
                },
            };

            // Execute the CloudScript function to reserve a machine
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    var functionResult = JsonMapper.ToObject<FunctionResult>(result.FunctionResult.ToString());
                    if (!functionResult.success)
                    {
                        Debug.LogError($" {result.FunctionName}  Fail, {functionResult.errorMessage}");
                    }
                },
                (PlayFabError error) =>
                {
                    Debug.LogError($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }

        /// <summary>
        /// Leaves the selected machine lobby using a CloudScript function.
        /// </summary>
        public void LeaveMachine()
        {
            // Construct the CloudScript request to leave the machine
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "LeaveMachine",
                FunctionParameter = new Dictionary<string, object>()
                {
                    { "machineId", MachineLobbyMediator.Instance.SelectedMachineId },
                },
            };

            // Execute the CloudScript function to leave the machine
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }

                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");
                    var isSuccess = Convert.ToBoolean(result.FunctionResult.ToString());
                    if (!isSuccess)
                    {
                        Debug.LogError($" {result.FunctionName}  Fail");
                    }
                },
                (PlayFabError error) =>
                {
                    Debug.LogError($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });

            BackToSelectedMachineScene();
        }

        /// <summary>
        /// Returns to the selected machine lobby scene.
        /// </summary>
        public void BackToSelectedMachineScene()
        {
            UserAccountManager.Instance.InvokeLoginSuccess();
            SceneManager.LoadScene("MachineLobby");
        }

        #endregion
    }
}
