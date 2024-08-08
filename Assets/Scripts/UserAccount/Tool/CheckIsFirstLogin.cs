using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.CloudScriptModels;
using System;
using LitJson;

public class CheckIsFirstLogin
{
    public class DeviceCodeData
    {
        public string DeviceCode;
        public DateTime LoginTime;
    }
    public bool IsFirstTime =false;

    private List<DeviceCodeData> DeviceData = new List<DeviceCodeData>();
    private string DeviceCode;
    public IEnumerator CheckIsFirstTime(string PlayFabId)
    {
        bool isFin = false;

        NowDeviceCode();

        var request = new GetUserDataRequest()
        {
            PlayFabId = PlayFabId,
            Keys = new List<string> {"DeviceCode"}
        };
        PlayFabClientAPI.GetUserReadOnlyData(request, result=> 
        {
            if(!result.Data.ContainsKey("DeviceCode"))
            {
                Debug.Log("No Data");
                IsFirstTime = true;
            }
            else
            {
                Debug.Log(result.Data["DeviceCode"].Value);
                DeviceData.AddRange(JsonMapper.ToObject<List<DeviceCodeData>>(result.Data["DeviceCode"].Value));
                foreach(var item in DeviceData)
                {
                    if(item.DeviceCode == DeviceCode)
                    {
                        IsFirstTime = false;
                    }
                }
            }
            isFin = true;
        },error=> 
        {
            Debug.Log(error.ErrorMessage);
        });
        yield return new WaitUntil(() =>isFin);
    }
    
    void NowDeviceCode()
    {
        var DeviceIdGetter = new UserAccount.DeviceIdGetter();
        DeviceIdGetter.GetDeviceID(out string andriodID, out string iosID, out string customID);

        if (!string.IsNullOrEmpty(andriodID))
        {
            DeviceCode = andriodID;
        }
        else if (!string.IsNullOrEmpty(iosID))
        {
            DeviceCode = iosID;
        }
        else
        {
            DeviceCode = customID;
        }
    }

    public IEnumerator SaveDeviceCode()
    {
        bool isFin = false;


        var request = new ExecuteFunctionRequest();
        request.FunctionName = "SaveDeviceCode";
        if(DeviceData.Count == 0)
        {
            request.FunctionParameter = new { Devicedata = "", Data = DeviceCode };
        }
        else
        {
            request.FunctionParameter = new { Devicedata = DeviceData, Data = DeviceCode };
        }

        PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
        {
            isFin = true;
            Debug.Log("Save success");
        }, error =>
        {
            Debug.Log(error.ErrorMessage);
        });
        yield return new WaitUntil(() => isFin);
    }

    
}
