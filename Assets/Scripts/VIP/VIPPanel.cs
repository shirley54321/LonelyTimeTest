using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using Player;
using Loading;
using TMPro;
using LitJson;
using System;
public class MonthlyData
{
    public List<MonthlyEntry> entries;
}

public class MonthlyEntry
{
    public int total;
    public int year;
    public int month;
}


public class VIPPanel : BasePanel
{
    public Image NowVIP,NextVIP;
    public TMP_Text  Back_EXP_Topup,Back_EXP_Bet;
    public TMP_Text  VipDeadLine;
    public int tmp_Top_Up, tmp_Recharge;
    public string playerID;
    public string tmp_VIP_DEADLINE;
    [SerializeField] private VipIconData vipIconData;
    int[] TopUp = { 1000, 5000, 10000, 30000, 50000, 100000 };
    int[] Bet = { 5000000, 25000000, 50000000, 150000000, 250000000, 500000000 };

    private SlicedFilledImage EXP_Topup_Fill_value,EXP_Bet_Fill_value;
    public GameObject EXP_Topup_Fill,EXP_Bet_Fill;


    bool VipPanelIsOpen = false;
    float VipPanelOpTimer = 0;
    private void Update()
    {
        if (VipPanelIsOpen)
        {
            VipPanelOpTimer += Time.deltaTime;


            if(VipPanelOpTimer > 5)
            {
                SetInfo();
                VipPanelOpTimer = 0;
            }

        }
    }

    public void SetInfo()
     {
  
   if (VipPanelIsOpen == false) LoadingManger.Instance.Open_Loading_animator();

     VipPanelIsOpen = true;
     VipPanelOpTimer = 0;
     Debug.Log("SetInfo()");
    if (EXP_Topup_Fill != null)
    {
        EXP_Topup_Fill_value=EXP_Topup_Fill.GetComponent<SlicedFilledImage>();
        if (EXP_Topup_Fill_value == null)
        {
            Debug.Log("EXP_Topup_Fill_value");
        }
    }

    if (EXP_Bet_Fill != null)
    {
        EXP_Bet_Fill_value=EXP_Bet_Fill.GetComponent<SlicedFilledImage>();
        if (EXP_Bet_Fill_value == null)
        {
            Debug.Log("EXP_Bet_Fill_value");
        }
    }

    var playerinfo = PlayerInfoManager.Instance.PlayerInfo;
    playerID = PlayerInfoManager.Instance.PlayFabId;
    tmp_Top_Up = 0;
    tmp_Recharge = 0;
    tmp_VIP_DEADLINE = "";
    
    var request1 = new GetUserDataRequest
    {
        Keys = new List<string> { "Top_Up_VIP", "Recharge_VIP", "VIP_DEADLINE" }
    };

    PlayFabClientAPI.GetUserData(request1, result =>
    {
        Debug.Log("從伺服器獲取VIP資訊!!!");
        if (result.Data.TryGetValue("Top_Up_VIP", out UserDataRecord Top_Up_VIP_Data))
        {
            if (int.TryParse(Top_Up_VIP_Data.Value, out int Top_Up_VIP_Value))
            {
                tmp_Top_Up = Top_Up_VIP_Value;
            }
            else
            {
                Debug.LogError("Failed to parse Top_Up_VIP value as integer");
            }
        }
        else
        {
            Debug.LogError("Top_Up_VIP data not found");
        }

        if (result.Data.TryGetValue("Recharge_VIP", out UserDataRecord Recharge_VIP_Data))
        {
            if (int.TryParse(Recharge_VIP_Data.Value, out int Recharge_VIP_Value))
            {
                tmp_Recharge = Recharge_VIP_Value;
            }
            else
            {
                Debug.LogError("Failed to parse Recharge_VIP value as integer");
            }
        }
        else
        {
            Debug.LogError("Recharge_VIP data not found");
        }

        if (result.Data.TryGetValue("VIP_DEADLINE", out UserDataRecord vipDeadlineData))
        {
            tmp_VIP_DEADLINE = vipDeadlineData.Value;
        }
        else
        {
            Debug.LogError("VIP_DEADLINE data not found");
        }

        StartCoroutine(GetMonthlyData(() =>
        {
            Debug.Log("DATA_Setting");

            if (playerinfo.vip.level >= 4)
            {
                //Debug.LogError("DDDDDDDDDDDDDDDDDDDDDDDDDD");
                int year = 0, month = 0;
                int Deadline_day = 0;
                string[] parts = tmp_VIP_DEADLINE.Split('_');

                if (parts.Length == 2)
                {
                    string yearString = parts[0];
                    string monthString = parts[1];
                    bool yearParsed = int.TryParse(yearString, out year);
                    bool monthParsed = int.TryParse(monthString, out month);

                    if (!yearParsed || !monthParsed)
                    {
                        Debug.LogError($"Failed to parse year or month.year={year} month={month}");
                    }
                }
                else
                {
                    Debug.LogError("輸入VIP有效日期有問題");
                }

                if (year != 0 && month != 0)
                {
                    Deadline_day = CalculateDaysInMonth(year, month);
                }

                NowVIP.sprite = playerinfo.vipIcon;
                if (playerinfo.vip.level <= 8)
                {
                    NextVIP.gameObject.SetActive(true);
                    NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level + 1);
                }
                else
                {
                    NextVIP.gameObject.SetActive(false);
                }

                if (playerinfo.vip.level < 9)
                {
                    EXP_Topup_Fill_value.fillAmount = (float)tmp_Recharge / (float)TopUp[playerinfo.vip.level - 3];
                    EXP_Bet_Fill_value.fillAmount = (float)tmp_Top_Up / (float)Bet[playerinfo.vip.level - 3];
                    Back_EXP_Topup.text = $"{tmp_Recharge}/{TopUp[playerinfo.vip.level - 3]}";
                    Back_EXP_Bet.text = $"{tmp_Top_Up}/{Bet[playerinfo.vip.level - 3]}";
                }
                else
                {
                    if (tmp_Recharge > TopUp[5])
                    {
                        tmp_Recharge = TopUp[5];
                    }
                    if (tmp_Top_Up > Bet[5])
                    {
                        tmp_Top_Up = Bet[5];
                    }
                    EXP_Topup_Fill_value.fillAmount = (float)tmp_Recharge / (float)TopUp[5];
                    EXP_Bet_Fill_value.fillAmount = (float)tmp_Top_Up / (float)Bet[5];
                    Back_EXP_Topup.text = $"{tmp_Recharge}/{TopUp[5]}";
                    Back_EXP_Bet.text = $"{tmp_Top_Up}/{Bet[5]}";
                }

                VipDeadLine.text = $"有效日期：{month}/{Deadline_day}";
            }
            else
            {
                NowVIP.sprite = playerinfo.vipIcon;
                NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level + 1);
                EXP_Topup_Fill_value.fillAmount = (float)tmp_Recharge / (float)TopUp[0];
                EXP_Bet_Fill_value.fillAmount = (float)tmp_Top_Up / (float)Bet[0];
                Back_EXP_Topup.text = $"{tmp_Recharge}/{TopUp[0]}";
                Back_EXP_Bet.text = $"{tmp_Top_Up}/{Bet[0]}";
                VipDeadLine.text = "永久";
            }

            LoadingManger.Instance.Close_Loading_animator();
        }));
    },
    error =>
    {
        Debug.LogError("GetUserData : Top_Up && Recharge Error");
        LoadingManger.Instance.Close_Loading_animator();
    });
}
//tmp_Top_Up
    private void ParseMonthlyTopUp(string json,int BaseMonth,int BaseYear)
    {
        List<MonthlyEntry> monthlyEntries = JsonMapper.ToObject<List<MonthlyEntry>>(json);
        if(BaseMonth==1){
            foreach (var entry in monthlyEntries)
            {
                if(entry.year==(BaseYear-1)&& entry.month==12){
                    tmp_Top_Up=tmp_Top_Up+entry.total;
                }
            }
        }
        else{
            foreach (var entry in monthlyEntries)
            {
                if(entry.year==BaseYear&& (entry.month==BaseMonth-1)){
                    tmp_Top_Up=tmp_Top_Up+entry.total;
                }
            }
        }
    }
//tmp_Recharge
    private void ParseMonthlyRecharge(string json,int BaseMonth,int BaseYear)
    {
        List<MonthlyEntry> monthlyEntries = JsonMapper.ToObject<List<MonthlyEntry>>(json);
        if(BaseMonth==1){
            foreach (var entry in monthlyEntries)
            {
                if(entry.year==(BaseYear-1)&& entry.month==12){
                    tmp_Recharge=tmp_Recharge+entry.total;
                }
            }
        }
        else{
            foreach (var entry in monthlyEntries)
            {
                if(entry.year==BaseYear&& (entry.month==BaseMonth-1)){
                    tmp_Recharge=tmp_Recharge+entry.total;
                }
            }
        }
    }
    public void CloseVIPPanel()
    {
        UIManager.Instance.ClosePanel(UIConst.VIPPanel);
        VipPanelIsOpen = false;
    }
    public int CalculateDaysInMonth(int year,int month)
    {
        bool isLeapYear = (year % 4 == 0 && year % 100 != 0) || (year % 400 == 0);
        int[] daysInMonth = new int[]
        {
            31, // 1月
            isLeapYear ? 29 : 28, // 2月，29天，28天
            31, // 3月
            30, // 4月
            31, // 5月
            30, // 6月
            31, // 7月
            31, // 8月
            30, // 9月
            31, // 10月
            30, // 11月
            31  // 12月
        };
        if (month >= 1 && month <= 12)
        {
            return daysInMonth[month - 1];
        }
        else
        {
            Debug.LogError($"錯誤的月份 {month}");
            return 0;
        }
    }

    private IEnumerator<bool> GetMonthlyData(Action onComplete)
    {
        var request = new GetUserDataRequest
        {
            PlayFabId=playerID,
            Keys = new List<string> { "Monthly_Recharge", "Monthly_TopUp" }
        };
        bool isDataReceived = false;
        PlayFabClientAPI.GetUserReadOnlyData(request, result =>
        {
            DateTime currentTime = DateTime.Now;
            int BaseMonth=currentTime.Month;
            int BaseYear=currentTime.Year;
            //Debug.Log($"--------------{BaseYear}{BaseMonth}--------------");
            if (result.Data.ContainsKey("Monthly_Recharge"))
            {
                if (result.Data["Monthly_Recharge"].Value != "")
                {
                    ParseMonthlyRecharge(result.Data["Monthly_Recharge"].Value,BaseMonth,BaseYear);
                }
            }

            if (result.Data.ContainsKey("Monthly_TopUp"))
            {
                if (result.Data["Monthly_TopUp"].Value != "")
                {
                    ParseMonthlyTopUp(result.Data["Monthly_TopUp"].Value,BaseMonth,BaseYear);
                }
            }
            isDataReceived = true;
        },
        error =>
        {
            isDataReceived = true;
            Debug.LogError("呼叫失敗GetUserReadOnlyData: " + error.GenerateErrorReport());
        });

        while (!isDataReceived)
        {
            yield return false;
        }

        onComplete();
    }
}
