using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Loading;
using PlayFab.ClientModels;
using PlayFab;
using Newtonsoft.Json.Linq;

using System.Globalization;
using System.Text;
public class FriendToggle : MonoBehaviour
{
    [SerializeField] Image avatar_Image;//好友頭像
    [SerializeField] GameObject onlineOrOffline_GameObject;//在/離線
    [SerializeField] List<Sprite> onlineAndOffline_Sprite;//在線跟離線的圖片 0:on 1:off
    [SerializeField] TextMeshProUGUI name_Text;//好友名字
    [SerializeField] Image toggleBackgroud_Imgae;//物件背景
    public Toggle toggle;
    private UnityEngine.Events.UnityAction<bool> toggleListener;

    public string id;
    public string Name;
    public string HeadIconURL;

    private void Start()
    {


        toggleBackgroud_Imgae.color = GetColor("#878787");
        avatar_Image.color = GetColor("#787878");
        name_Text.color = GetColor("#808080");
    }



    private void Update()
    {
        if (isSelect)//toggle被選擇
        {
            toggleBackgroud_Imgae.color = Color.white;
            avatar_Image.color = Color.white;
            name_Text.color = Color.white;


        }
        else
        {
            toggleBackgroud_Imgae.color = GetColor("#878787");
            avatar_Image.color = GetColor("#787878");
            name_Text.color = GetColor("#808080");
        }
    }
    /// <summary>
    /// 更換好友頭像(TODO)
    /// </summary>
    /// 
    public bool isSelect = false;
    public void ChangeAvator()
    {

    }




    /// <summary>
    /// 更換好友名字
    /// </summary>
    public void ChangeName(string name)
    {
        name_Text.text = name;
    }

    /// <summary>
    /// 更換在線或離線
    /// </summary>
    /// <param name="online">true = online false = offline</param>
    public void ChangeOnlineOrOffline(bool online)
    {
        if (online) //在線
        {
            onlineOrOffline_GameObject.transform.GetComponent<Image>().sprite = onlineAndOffline_Sprite[0];
            onlineOrOffline_GameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "線上";
        }
        else //離線
        {
            onlineOrOffline_GameObject.GetComponent<Image>().sprite = onlineAndOffline_Sprite[1];
            onlineOrOffline_GameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "離線";
        }
    }

    /// <summary>
    /// toggle沒被選擇就轉灰
    /// </summary>
    /// <param name="isOn"></param>
    void ToggleToGray(bool isOn)
    {
        if (isOn)//toggle被選擇
        {
            toggleBackgroud_Imgae.color = Color.white;
            avatar_Image.color = Color.white;
            name_Text.color = Color.white;

            ClickToggle();
        }
        else
        {
            toggleBackgroud_Imgae.color = GetColor("#878787");
            avatar_Image.color = GetColor("#787878");
            name_Text.color = GetColor("#808080");
        }
    }

    /// <summary>
    /// 用十六進位抓色票
    /// </summary>
    /// <param name="htmlString"></param>
    /// <returns></returns>
    public Color GetColor(string htmlString)
    {
        Color newColor;
        if (ColorUtility.TryParseHtmlString(htmlString, out newColor))
        {
            return newColor;
        }
        else
        {
            Debug.LogError("Invalid color code.");
            return Color.white; // 默認返回白色
        }
    }

    private void OnDestroy()
    {

    }


    public void SetHeadIconAndName(string url)
    {
        name_Text.text = Name;
        StartCoroutine(LoadImageCoroutine(url));
    }

    private IEnumerator LoadImageCoroutine(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading image: {www.error}");
            }
            else
            {
                // 将下载的图片数据转换为Texture2D
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // 创建Sprite并应用于UI Image组件
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    avatar_Image.sprite = sprite;
                }
            }
        }
    }



    public GameObject ChatPanel;
    public void ClickToggle()
    {


        foreach (Transform child in transform.parent)
        {
            child.GetComponent<FriendToggle>().isSelect = false;

        }



        isSelect = true;
        ChatPanel = GameObject.Find("messeng");

        Debug.Log("ClickToggle,SET_ID:" + id);
        ChatPanel.GetComponent<ChatManager>().friendPlayFabID = id;
        ChatPanel.GetComponent<ChatManager>().friendDisplayName = Name;
        ChatPanel.GetComponent<ChatManager>().AvatorWantTobe = avatar_Image.sprite;
        // ChatPanel.GetComponent<ChatManager>().ClearChatScroll();
        ChatPanel.GetComponent<ChatManager>().RefreshPrivateMessage();

    }

    public void ClickHeadIcon()
    {

        UIManager.Instance.OpenPanel(UIConst.OtherPlayerPage);


        string id = this.id;



        //StartCoroutine(SetPanelValue());
        StartCoroutine(GetPlayerCombinedInfoAsync(id));

    }

    GameObject OtherPlayerPanel;
    IEnumerator GetPlayerCombinedInfoAsync(string playFabId)
    {
        LoadingManger.Instance.Open_Loading_animator();


        yield return new WaitForSeconds(0.1f);
        OtherPlayerPanel = GameObject.Find("OtherPlayerInfoPage(Clone)");
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().Name = Name;
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().playFabId = id;
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().HeadIconURL = HeadIconURL;
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
        if (inventory != null) foreach (var item in inventory)
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
                OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().VIP = upgradeDataArray[upgradeDataArray.Length - 1].level + "";
            }

        }

        // 玩家统计
        var playerStatistics = result.InfoResultPayload.PlayerStatistics;
        foreach (var stat in playerStatistics)
        {
            Debug.Log($"Statistic: {stat.StatisticName}, Value: {stat.Value}");
            if (stat.StatisticName == "UserLevel") OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().USERLEVEL = stat.Value + "";
        }

        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().SetText();
        LoadingManger.Instance.Close_Loading_animator();
    }

    private void OnGetPlayerCombinedInfoError(PlayFabError error)
    {
        Debug.LogError($"Error retrieving player combined info: {error.GenerateErrorReport()}");
    }
    IEnumerator SetPanelValue()
    {
        yield return null;

        //PanelOBJ.GetComponent<OtherPlayerInfoPage>().player = GetComponentInParent<FriendObj>().player;
        //PanelOBJ.GetComponent<OtherPlayerInfoPage>().SetValue();
        // PanelOBJ.GetComponent<OtherPlayerInfoPage>().SetValue(otherPlayerInfoPageData);
        OtherPlayerPanel.GetComponent<OtherPlayerInfoPage>().HeadIconOBJ.GetComponent<Image>().sprite = GetComponentInParent<FriendToggle>().avatar_Image.sprite;


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
                    if (currency.Key == "DC") lastDC = currency.Value + "";

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
