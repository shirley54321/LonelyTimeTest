using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Loading;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Globalization;
using System.Text;
using static OpenOtherPlayerPanel;
public class OpenOtherPlayerPanel : MonoBehaviour
{
    private void OnEnable()
    {
        LobbyEventHandler.OpenOtherPlayerInfoPage += OpenOtherPlayerInfoPage;
    }

    private void OnDisable()
    {
        LobbyEventHandler.OpenOtherPlayerInfoPage -= OpenOtherPlayerInfoPage;
    }

    private void OpenOtherPlayerInfoPage(Transform obj)
    {
        PanelOBJ = obj;
    }
    public Transform PanelOBJ;
    public void OpenPlayerPanel()
    {

        UIManager.Instance.OpenPanel(UIConst.OtherPlayerPage);


        string id = GetComponentInParent<FriendObj>().id;



        //StartCoroutine(SetPanelValue());
        StartCoroutine(GetPlayerCombinedInfoAsync(id));
    }

    public class OtherPlayerInfoPageData
    {
        public string id;
        public string Name;
        //public string HeadIconURL;
        public string DashBoard;
        public string Activity;
        public string VIP;
        public string LEVEL;
        public string DC;
        public string DB;
        Sprite HeadIcon;
    }
    OtherPlayerInfoPageData otherPlayerInfoPageData = new OtherPlayerInfoPageData();
    IEnumerator SetPanelValue()
    {
        yield return null;

        //PanelOBJ.GetComponent<OtherPlayerInfoPage>().player = GetComponentInParent<FriendObj>().player;
        //PanelOBJ.GetComponent<OtherPlayerInfoPage>().SetValue();
        // PanelOBJ.GetComponent<OtherPlayerInfoPage>().SetValue(otherPlayerInfoPageData);
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().HeadIconOBJ.GetComponent<Image>().sprite = GetComponentInParent<FriendObj>().HeadIcon.sprite;

    }

    public GameObject OtherPlayerPanel;
    IEnumerator GetPlayerCombinedInfoAsync(string playFabId)
    {
        LoadingManger.Instance.Open_Loading_animator();
      

        yield return new WaitForSeconds(0.1f);
        OtherPlayerPanel = GameObject.Find("OtherPlayerInfoPage(Clone)");
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().Name = GetComponentInParent<FriendObj>().Name;
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().playFabId = GetComponentInParent<FriendObj>().id;
        StartCoroutine(SetPanelValue());





        var request = new GetPlayerCombinedInfoRequest
        {
            PlayFabId = playFabId,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetUserAccountInfo = true,
                GetUserInventory = true,
                GetUserVirtualCurrency = true,
                GetUserData = true,
                GetUserReadOnlyData = true,
                GetPlayerStatistics = true
            }
        };

        GetPlayerVirtualCurrency(playFabId);

        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().MessageBoard = "";


        PlayFabClientAPI.GetPlayerCombinedInfo(request, OnGetPlayerCombinedInfoSuccess, OnGetPlayerCombinedInfoError);

        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().CheckIfFriend(playFabId);
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().CheckIfBlackList(playFabId);
    }

    private void OnGetPlayerCombinedInfoSuccess(GetPlayerCombinedInfoResult result)
    {
        Debug.Log("Successfully retrieved player combined info!");

        // 账户信息
        var accountInfo = result.InfoResultPayload.AccountInfo;
        Debug.Log($"Username: {accountInfo.Username}");


        // 库存
        var inventory = result.InfoResultPayload.UserInventory;
       if(inventory != null) foreach (var item in inventory)
        {
            Debug.Log($"Item: {item.DisplayName}, Quantity: {item.RemainingUses}");
        }

        // 虚拟货币


        // 用户数据
        var userData = result.InfoResultPayload.UserData;
        foreach (var data in userData)
        {
            Debug.Log($"UserData Key: {data.Key}, Value: {data.Value.Value}");
            if (data.Key == "MessageBoard")
            {
                string jsonString = data.Value.Value;
                MessageData[] messages = JsonHelper.FromJson<MessageData>(jsonString);
                foreach (var message in messages)
                {
                    string decodedMessage = DecodeUnicode(message.Message);
                    Debug.Log($"Message: {decodedMessage}, UpdateTime: {message.UpdateTime}");

                    OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().MessageBoard = decodedMessage;
                }
            }
        }

        // 只读数据
        var readOnlyData = result.InfoResultPayload.UserReadOnlyData;
        foreach (var entry in readOnlyData)
        {
            Debug.Log($"ReadOnlyData Key: {entry.Key}, Value: {entry.Value.Value}");
            Debug.Log($"Key: {entry.Key}, Value: {entry.Value.Value}");
            // 在这里，你可以根据需要将Player Data显示在UI上
            // 例如：在playerDataItem上添加新的Text组件来显示这些数据

            if (entry.Key == "UserProfile")
            {
                OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().id = JObject.Parse(entry.Value.Value)["UID"].ToString();
            }

           // else if (entry.Key == "PlayerLevel")
           // {
           //     OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().LEVEL = entry.Value.Value;
           // }



            else if (entry.Key == "VIP")
            {

         
                string jsonString = entry.Value.Value;
                UpgradeInfo[] upgradeDataArray = JsonHelper.FromJson<UpgradeInfo>(jsonString);

                // 提取最后一个 level 的值
                if (upgradeDataArray.Length > 0)
                {
                    int lastLevel = upgradeDataArray[upgradeDataArray.Length - 1].level;
                    Debug.Log($"Last  VIP level value: {lastLevel}");
                }
                else
                {
                    Debug.Log("No data found.");
                }
                OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().VIP = upgradeDataArray[upgradeDataArray.Length - 1].level +"";
            }

        }

        // 玩家统计
        var playerStatistics = result.InfoResultPayload.PlayerStatistics;
        foreach (var stat in playerStatistics)
        {
            Debug.Log($"Statistic: {stat.StatisticName}, Value: {stat.Value}");
          if(stat.StatisticName == "UserLevel")  OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().USERLEVEL = stat.Value + "";
        }

        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().SetText();
        LoadingManger.Instance.Close_Loading_animator();
    }

    private void OnGetPlayerCombinedInfoError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving player combined info: {error.GenerateErrorReport()}");
    }




    private void GetPlayerVirtualCurrency(string playFabId)
    {

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "getVirtualCurrency",
            FunctionParameter = new { playerId = playFabId },
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, OnExecuteCloudScriptSuccess, OnExecuteCloudScriptError);

    }
    public string lastDB = "0";
    public string lastDC = "0";
    private void OnExecuteCloudScriptSuccess(ExecuteCloudScriptResult result)
    {

        if (result.FunctionResult != null)
        {
            var virtualCurrencyData = (IDictionary<string, object>)result.FunctionResult;

            if (virtualCurrencyData.ContainsKey("VirtualCurrency"))
            {
                var virtualCurrency = (IDictionary<string, object>)virtualCurrencyData["VirtualCurrency"];

                foreach (var currency in virtualCurrency)
                {
                    Debug.Log($"Currency: {currency.Key}, Amount: {currency.Value}");
                    if (currency.Key == "DC") lastDC = currency.Value+"";

                    if (currency.Key == "DB") lastDB = currency.Value + "";
                }
            }
            else
            {
                Debug.Log("No virtual currency data found.");
            }
        }
        else
        {
            Debug.Log("No function result found.");
        }

        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().DC = lastDC;
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().DB = lastDB;
    }

    private void OnExecuteCloudScriptError(PlayFabError error)
    {
        Debug.LogError($"Error executing CloudScript: {error.GenerateErrorReport()}");
    }



    //======
    public static string DecodeUnicode(string input)
    {
        StringBuilder result = new StringBuilder();
        int i = 0;
        while (i < input.Length)
        {
            if (input[i] == '\\' && input[i + 1] == 'u' && i + 5 < input.Length)
            {
                string unicodeSequence = input.Substring(i + 2, 4);
                if (int.TryParse(unicodeSequence, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                {
                    result.Append((char)code);
                    i += 6;
                    continue;
                }
            }
            result.Append(input[i]);
            i++;
        }
        return result.ToString();
    }



}
[Serializable]
public class MessageData
{
    public string Message;
    public string UpdateTime;
}
[Serializable]
public class UpgradeInfo
{
    public int level;
    public int type;
    public string upgradeDate;
}
[Serializable]
public class PlayerProgress
{
    public int Level;
    public float Experience;
    public string UpgradeDate;
    public int NextLevelExperience;
}
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}