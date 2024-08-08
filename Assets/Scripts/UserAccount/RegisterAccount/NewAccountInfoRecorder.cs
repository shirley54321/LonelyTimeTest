using System.Collections.Generic;
using System.Threading.Tasks;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace UserAccount.RegisterAccount
{
    /// <summary>
    /// Records and manages new account information, such as login devices and registration records.
    /// </summary>
    public class NewAccountInfoRecorder
    {
        /// <summary>
        /// Adds a new login device record for the player to the PlayFab database.
        /// </summary>
        /// <returns>A task representing the asynchronous operation</returns>
        public static Task ADDLoginDevice()
        {
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType,
                },
                FunctionName = "LoginDevice",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"machineId", SystemInfo.deviceUniqueIdentifier},
                    {"playerId", PlayFabSettings.staticPlayer.EntityId}
                },
                GeneratePlayStreamEvent = false
            }, async (ExecuteFunctionResult result) =>
            {
                Debug.Log($"Account Record Status: {result.FunctionResult.ToString()}");
            }, (PlayFabError error) =>
            {
                Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
            });

            Debug.Log("ADDLoginDevice : OK");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Records the registration information of a new account to the PlayFab database.
        /// </summary>
        public static void RecordRegisterAccountToDB()
        {
            PlayFabCloudScriptAPI.ExecuteFunction(new ExecuteFunctionRequest()
            {
                Entity = new PlayFab.CloudScriptModels.EntityKey()
                {
                    Id = PlayFabSettings.staticPlayer.EntityId,
                    Type = PlayFabSettings.staticPlayer.EntityType,
                },
                FunctionName = "RecordRegisterRecord",
                FunctionParameter = new Dictionary<string, object>()
                {
                    {"machineId", SystemInfo.deviceUniqueIdentifier}
                },
                GeneratePlayStreamEvent = false
            }, async (ExecuteFunctionResult result) =>
            {
                Debug.Log($"Account Record Status: {result.FunctionResult.ToString()}");
            }, (PlayFabError error) =>
            {
                Debug.Log($"Opps Something went wrong: {error.GenerateErrorReport()}");
            });

            Debug.Log("RecordRegisterAccountToDB");
        }
    }
}
