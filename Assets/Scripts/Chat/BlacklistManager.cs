using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using System;
public class BlacklistManager : MonoBehaviour
{
    // 添加玩家到黑名单



    public void AddToBlacklist(string targetPlayFabId, System.Action<bool> callback)
    {
        Debug.Log("嘗試將targetPlayFabId加入黑名單");
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "addToBlacklist",
            FunctionParameter = new
            {
                playerId = PlayFabSettings.staticPlayer.PlayFabId,
                targetPlayFabId = targetPlayFabId
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            if (result.Error != null)
            {
                Debug.LogError("Error executing CloudScript: " + result.Error.Message);
                callback(false);
            }
            else
            {
                Debug.Log("CloudScript executed successfully: " + result.FunctionResult.ToString());
                callback(true);
            }
        }, error =>
        {
            Debug.LogError("Error calling CloudScript: " + error.GenerateErrorReport());
            callback(false);
        });
    }
    // 从黑名单中移除玩家

    public void RemoveFromBlacklist(string targetPlayFabId, System.Action<bool> callback)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "removeFromBlacklist",
            FunctionParameter = new
            {
                playerId = PlayFabSettings.staticPlayer.PlayFabId,
                targetPlayFabId = targetPlayFabId
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            if (result.Error != null)
            {
                Debug.LogError("Error executing CloudScript: " + result.Error.Message);
                callback(false);
            }
            else
            {
                Debug.Log("CloudScript executed successfully: " + result.FunctionResult.ToString());
                callback(true);
            }
        }, error =>
        {
            Debug.LogError("Error calling CloudScript: " + error.GenerateErrorReport());
            callback(false);
        });
    }

    private void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Blacklist updated successfully");
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError("Error updating blacklist: " + error.GenerateErrorReport());
    }

    public void ClickAddToBlacklist()
    {


        AddToBlacklist(GetComponent<OtherPlayerInfoPage>().playFabId, success =>
        {
            if (success)
            {
                Debug.Log("Successfully added to blacklist.");
                // 继续其他操作
            }
            else
            {
                Debug.LogError("Failed to add to blacklist.");
            }
        });

    }

    public void ClickRemoveBlacklist()
    {
        RemoveFromBlacklist(GetComponent<OtherPlayerInfoPage>().playFabId, success =>
        {
            if (success)
            {
                Debug.Log("Successfully Remove from blacklist.");
                // 继续其他操作
            }
            else
            {
                Debug.LogError("Failed to Remove from blacklist.");
            }
        });



    }


    public void CheckBlacklist(string targetPlayFabId, Action<bool> callback)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "checkBlacklist",
            FunctionParameter = new
            {
                playerId = PlayFabSettings.staticPlayer.PlayFabId,
                targetPlayFabId = targetPlayFabId
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            if (result.Error != null)
            {
                Debug.LogError("Error executing CloudScript: " + result.Error.Message);
                callback(false);
            }
            else
            {
                var functionResult = JsonUtility.FromJson<FunctionResult>(result.FunctionResult.ToString());
                callback(functionResult.isBlacklisted);
            }
        }, error =>
        {
            Debug.LogError("Error calling CloudScript: " + error.GenerateErrorReport());
            callback(false);
        });
    }

    public bool DoCheck = false;
    public bool DoRemove = false;
    private void Update()
    {
        if (DoCheck == true)
        {
            DoCheck = false;

            CheckBlacklist(GetComponent<OtherPlayerInfoPage>().playFabId, isInBlacklist =>
            {
                if (isInBlacklist)
                {
                    Debug.Log("Player is in the blacklist.");
                }
                else
                {
                    Debug.Log("Player is not in the blacklist.");
                }
            });

        }
    }

    public void GetBlacklist(Action<List<string>> callback)
    {
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getBlacklist",
            FunctionParameter = new
            {
                playerId = PlayFabSettings.staticPlayer.PlayFabId
            },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            if (result.Error != null)
            {
                Debug.LogError("Error executing CloudScript: " + result.Error.Message);
                callback(null);
            }
            else
            {
                Debug.Log("CloudScript executed successfully: " + result.FunctionResult.ToString());

                // 解析黑名单数据
                var functionResult = JsonUtility.FromJson<FunctionResult>(result.FunctionResult.ToString());
                callback(functionResult.blacklist);

            


            }
        }, error =>
        {
            Debug.LogError("Error calling CloudScript: " + error.GenerateErrorReport());
            callback(null);
        });





    }


    public void ClickGetBlackList()
    {
        GetBlacklist(blacklist =>
        {
            if (blacklist != null)
            {
                foreach (string id in blacklist)
                {
                    Debug.Log("Blacklisted ID: " + id);
                }
            }
            else
            {
                Debug.LogError("Failed to retrieve blacklist.");
            }
        });

    }

    [Serializable]
    public class FunctionResult
    {
        public List<string> blacklist;
        public bool isBlacklisted;
    }
}


