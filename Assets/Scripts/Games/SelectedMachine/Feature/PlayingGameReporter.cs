using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace Games.SelectedMachine
{
    /// <summary>
    /// This class is responsible for periodically sending messages to the server, indicating that the player is playing a slot machine game.
    /// </summary>
    public class PlayingGameReporter : MonoBehaviour
    {
        /// <summary>
        /// The interval for updating time, in seconds.
        /// </summary>
        [SerializeField] private float updateInterval = 60;

        /// <summary>
        /// Timer used to calculate the next update time.
        /// </summary>
        [SerializeField] private float timer;

        /// <summary>
        /// Initialization setting when the game starts, setting the timer to the update interval.
        /// </summary>
        private void Start()
        {
            timer = updateInterval;
        }

        /// <summary>
        /// Update the timer each frame. When the timer reaches zero, report the player's last play time to the MachineLobbyMediator.
        /// </summary>
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer < 0)
            {
                timer = updateInterval;
                SetLastPlayMachineTime();
            }
        }
        
        /// <summary>
        /// Sets the last play time for the selected machine using a CloudScript function.
        /// </summary>
        [ContextMenu("SetLastPlayMachineTime")]
        public void SetLastPlayMachineTime()
        {
            // Construct the CloudScript request to set the last play time for the machine
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "SetLastPlayMachineTime",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"machineId" , MachineLobbyMediator.Instance.SelectedMachineId},
                },
            };

            // Execute the CloudScript function to set the last play time
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest, 
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function, See PlayFab Limits Page for details.");
                        return;
                    }
                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");

                    if (result.ExecutionTimeMilliseconds > 5500)
                    {
                        StartCoroutine(DelayedInvoke());
                        Debug.Log("WaitForSeconds(20f) Reacquire Result");
                    }

                    var isSuccess =  Convert.ToBoolean(result.FunctionResult.ToString());
                    if (!isSuccess)
                    {
                        Debug.LogError($" {result.FunctionName}  Fail");
                    }
                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Opps Something went wrong: \n{error.GenerateErrorReport()}");
                });
        }

        private IEnumerator DelayedInvoke()
        {
            yield return new WaitForSeconds(20f); 
            GameCach.Instance.GainMoreResult(0, (int)MachineLobbyMediator.Instance.SelectedGameId, 2);
        }
    }
}