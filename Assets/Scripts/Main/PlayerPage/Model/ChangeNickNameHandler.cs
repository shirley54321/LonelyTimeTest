using System.Collections.Generic;
using Games.SelectedMachine;
using LitJson;
using Loading;
using Player;
using Player.AzureFunctionResult;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using Share.Azure;
using Share.Tool;
using UnityEngine;
using UnityEngine.Events;

namespace Main.PlayerPage.Model
{
    /// <summary>
    /// Handles the process of changing the player's nickname, including validation and PlayFab API calls.
    /// </summary>
    public class ChangeNickNameHandler : MonoBehaviour
    {
        [SerializeField] private StringFormatValidator stringFormatValidator;

        public readonly UnityEvent<ValidateResult> OnValidateNewNickName = new UnityEvent<ValidateResult>();
        public readonly UnityEvent OnChangeNickNameSuccess = new UnityEvent();
        public readonly UnityEvent<ChangeNickNameResultType> OnChangeNickNameFailed = new UnityEvent<ChangeNickNameResultType>();

        /// <summary>
        /// Initiates the process of changing the player's nickname.
        /// </summary>
        /// <param name="newName">The new nickname to set.</param>
        public void ChangeNickName(string newName)
        {
            var validateResult = ValidateChangeNickNameRule(newName);

            OnValidateNewNickName.Invoke(validateResult);

            if (validateResult == ValidateResult.Pass)
            {
                CallChangeNickNameAf(newName);
            }
        }

        private void CallChangeNickNameAf(string newName)
        {
            Debug.Log($"NickName : {newName} Pass Local String Validator");
                LoadingManger.Instance.Open_Loading_animator();
   
                // Construct the CloudScript request to leave the machine
                var executeCloudScriptRequest = new ExecuteFunctionRequest()
                {
                    FunctionName = "ChangeNickName",
                    FunctionParameter = new Dictionary<string, object>()
                    {
                        {"name", newName}
                    },
                };

                // Execute the CloudScript function 
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
                        var functionResult = JsonMapper.ToObject<ChangeNickNameResult>(result.FunctionResult.ToString());
                        if (functionResult.ResultType == ChangeNickNameResultType.Success)
                        {
                            PlayerInfoManager.Instance.SetLocalNickName($"{functionResult.DisplayName}");
                            Debug.Log($"Change Nick Name success {result}");
                            SetHaveChangeNickNameRecord();
                        }
                        else
                        {
                            OnChangeNickNameFailed.Invoke(functionResult.ResultType);
                        }
                        
                        LoadingManger.Instance.Close_Loading_animator();
                        
                    },
                    (PlayFabError error) =>
                    {
                        Debug.LogError($"Oops Something went wrong: \n{error.GenerateErrorReport()}");
                        OnChangeNickNameFailed.Invoke(ChangeNickNameResultType.OtherError);
                        LoadingManger.Instance.Close_Loading_animator();
                    });
        }
        
        

        /// <summary>
        /// Validates the new nickname against specified rules using StringFormatValidator.
        /// </summary>
        /// <param name="newName">The new nickname to validate.</param>
        /// <returns>The result of the validation process (Pass, Warning, or Error).</returns>
        public ValidateResult ValidateChangeNickNameRule(string newName)
        {
            stringFormatValidator.SetValidateFormat(2, 8, 1, 1);
            var validateResult = stringFormatValidator.ValidateInput(newName);

            return validateResult;
        }

        private void SetHaveChangeNickNameRecord()
        {
            var changeNameRecord = new ChangeNameRecord()
            {
                haveChangedNickName = true
            };


            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
                {
                    {nameof(ChangeNameRecord), JsonMapper.ToJson(changeNameRecord)}
                },
                Permission = UserDataPermission.Public
            }, (result) =>
            {
                Debug.Log($"Set {nameof(ChangeNameRecord)} Success");

                PlayerInfoManager.Instance.SetLocalChangeNickNameRecord(changeNameRecord);
                OnChangeNickNameSuccess?.Invoke();
            }, (error) =>
            {
                Debug.Log($"Set {nameof(ChangeNameRecord)}Fail {error}");
                OnChangeNickNameSuccess?.Invoke();
                // TODO: Add Server Error
            });
        }
    }
}
