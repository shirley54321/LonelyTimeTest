using System.Collections;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

namespace UserAccount.AccountInitializer
{
    /// <summary>
    /// Class responsible for initializing user accounts, including the creation of a nickname.
    /// </summary>
    public class AccountInitializer
    {
        /// <summary>
        /// Coroutine to asynchronously create a nickname for a user account.
        /// </summary>
        /// <returns>IEnumerator used for coroutine functionality.</returns>
        public static IEnumerator CreateNickNameCoroutine()
        {
            bool isResponseReceived = false;

            // Construct the CloudScript request to retrieve spin count
            var executeCloudScriptRequest = new ExecuteFunctionRequest()
            {
                FunctionName = "CreateNickName",
            };

            // Execute the CloudScript function to get spin count
            PlayFabCloudScriptAPI.ExecuteFunction(executeCloudScriptRequest,
                (ExecuteFunctionResult result) =>
                {
                    if (result.FunctionResultTooLarge ?? false)
                    {
                        Debug.Log("This can happen if you exceed the limit that can be returned from an Azure Function. See PlayFab Limits Page for details.");
                        return;
                    }
                    Debug.Log($"The {result.FunctionName} function took {result.ExecutionTimeMilliseconds} to complete");

                    isResponseReceived = true;

                }, (PlayFabError error) =>
                {
                    Debug.LogError($"Oops! Something went wrong:\n{error.GenerateErrorReport()}");
                });

            // Wait until a response is received
            yield return new WaitUntil(() => isResponseReceived);
        }
    }
}