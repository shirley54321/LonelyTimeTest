using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using Player;
using PlayFab.CloudScriptModels;
using System.Linq;
using TMPro;
using Loading;
using LitJson;

namespace shopNameSpace
{
    [Serializable]
    public class item_data
    {
        public string id;
        public int increaseCount;
    }

    public class shop : MonoBehaviour, IStoreListener
    {
        public item_data item1, item2, item3, item4, item5, item6;
        IStoreController m_StoreController;
        [SerializeField] private TextMeshProUGUI coins_TEXT;
        [SerializeField] private VipIconData vipIconData;
        [SerializeField] private Image NowVIP;
        [SerializeField] private Image NextVIP;

        //public Slider VIP_exp;
        public TMP_Text  Back_VIP_exp;
        private SlicedFilledImage VIP_exp_Fill_value;
        public GameObject VIP_exp_Fill;

        int[] Recharge = { 1000, 5000, 10000, 30000, 50000, 100000 };
        int tmp_Recharge=0;

        void Start()
        {
            if (VIP_exp_Fill != null)
            {
                VIP_exp_Fill_value=VIP_exp_Fill.GetComponent<SlicedFilledImage>();
                if (VIP_exp_Fill_value == null)
                {
                    Debug.Log("檢查Shop，找不到VIP_exp_Fill_value");
                }
            }

            DisPlayCoins(1);

            SetVIPUI();
        }

        private async void Awake()
        {
            InitializationOptions options = new InitializationOptions()
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
            await UnityServices.InitializeAsync(options);

            SetupBuilder();
        }

        void SetupBuilder()
        {
#if UNITY_ANDROID
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
#elif UNITY_IOS
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.AppleAppStore));
#else
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.NotSpecified));
#endif
            builder.AddProduct(item1.id, ProductType.Consumable);
            builder.AddProduct(item2.id, ProductType.Consumable);
            builder.AddProduct(item3.id, ProductType.Consumable);
            builder.AddProduct(item4.id, ProductType.Consumable);
            builder.AddProduct(item5.id, ProductType.Consumable);
            builder.AddProduct(item6.id, ProductType.Consumable);

            UnityPurchasing.Initialize(this, builder);
        }

        public void purchase_btn_pressed1()
        {
            //3300
            AddVirtualCurrency("DC",3300);
           // m_StoreController.InitiatePurchase(item1.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }

        public void purchase_btn_pressed2()
        {
            //17000
            AddVirtualCurrency("DC",17000);
           // m_StoreController.InitiatePurchase(item2.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }
        public void purchase_btn_pressed3()
        {
            //33000
            AddVirtualCurrency("DC", 33000);
           // m_StoreController.InitiatePurchase(item3.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }

        public void purchase_btn_pressed4()
        {
            //99000
            AddVirtualCurrency("DC", 99000);
           // m_StoreController.InitiatePurchase(item4.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }
        public void purchase_btn_pressed5()
        {
            //169000
            AddVirtualCurrency("DC", 169000);
           // m_StoreController.InitiatePurchase(item5.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }

        public void purchase_btn_pressed6()
        {
            //329000
            AddVirtualCurrency("DC", 329000);
           // m_StoreController.InitiatePurchase(item6.id);
            LoadingManger.Instance.Open_Loading_animator(); //等待購買完成轉圈
        }


        public int addRecord = 0;
        public void AddVirtualCurrency(string currencyCode, int amount)
        {
            var request = new AddUserVirtualCurrencyRequest
            {
                VirtualCurrency = currencyCode,
                Amount = amount
            };

            addRecord = amount;
            PlayFabClientAPI.AddUserVirtualCurrency(request, OnAddVirtualCurrencySuccess, OnError);
        }

        private void OnAddVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult result)
        {
            Debug.Log("Successfully added virtual currency.");
            Debug.Log("New balance: " + result.Balance);





            LoadingManger.Instance.Close_Loading_animator();
            userLevel.refreshUserLevel();

            StartCoroutine(   addHist(addRecord));


            
                //123
            SetVIPUI();

        }

        private void OnError(PlayFabError error)
        {
            Debug.LogError("Error  " + error.GenerateErrorReport());
        }



      




















        private void OnEnable()
        {
            InventoryManager.OnDragonCoinChange.AddListener(DisPlayCoins);
        }

        private void OnDisable()
        {
            InventoryManager.OnDragonCoinChange.RemoveListener(DisPlayCoins);
        }

        private void DisPlayCoins(int coins)
        {
            var request = new GetUserInventoryRequest();
            PlayFabClientAPI.GetUserInventory(request, result =>
            {
                Debug.Log($"result.VirtualCurrency {result.VirtualCurrency.Count}");
                foreach (var item in result.VirtualCurrency)
                {
                    Debug.Log($"item.Key {item.Key}");
                    if (item.Key == "DC")
                    {
                        //coins_TEXT.text = item.Value.ToString();
                        if(item.Value / 100 == 0)
                            coins_TEXT.text = string.Format("0.{0:00}",item.Value % 100);
                        else
                            coins_TEXT.text = string.Format("{0:#,#}.{1:00}", item.Value/100, item.Value%100);
                    }
                }
            }, error =>
            {
                Debug.Log("Error retrieving user inventory: " + error.ErrorMessage);
            }
            );
        }


        public void AddDragonCoin(int count)
        {
            string id = PlayerInfoManager.Instance.PlayFabId;

            var request = new ExecuteFunctionRequest
            {
                FunctionName = "AddCoins",
                FunctionParameter = new
                {
                    playfabid = id,
                    virtualcurrency = "DC",
                    amount = count * 100
                }
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
            {
                Debug.Log("add coins sucsess:" + result.ToString());
                InventoryManager.Instance.UpdateDragonCoin();
                DisPlayCoins(1); // 更新顯示
            },
            error =>
            {
                Debug.Log("add coins failed:" + error.ErrorMessage);
            });


            //add puchase histoty
            StartCoroutine(addHist(count));

            //var request1 = new GetUserInventoryRequest();
            //PlayFabClientAPI.GetUserInventory(request1, result =>
            //{
            //    Debug.Log($"result.VirtualCurrency {result.VirtualCurrency.Count}");
            //    foreach (var item in result.VirtualCurrency)
            //    {
            //        Debug.Log($"item.Key {item.Key}");
            //        if (item.Key == "DC")
            //        {
            //            print(item.Value);  ///print now coins
            //            store = item.Value + count;

            //            //var request = new PlayFab.ClientModels.AddUserVirtualCurrencyRequest
            //            //{
            //            //    Amount = count,
            //            //    VirtualCurrency = item.Key
            //            //};


            //            //PlayFab.PlayFabClientAPI.AddUserVirtualCurrency(request, result =>
            //            //{
            //            //    Debug.Log($"AddUserDataRequest == Success {result}");
            //            //},
            //            //    error =>
            //            //    {
            //            //        Debug.LogError($"AddUserDataRequest == Failed {error}");
            //            //    }
            //            //);
            //            //InventoryManager.Instance.UpdateDragonCoin();


            //            var request2 = new PlayFab.ClientModels.SubtractUserVirtualCurrencyRequest   ///coins爆掉用來扣的
            //            {
            //                Amount = 30000,
            //                VirtualCurrency = item.Key
            //            };


            //            PlayFab.PlayFabClientAPI.SubtractUserVirtualCurrency(request2, result =>
            //            {
            //                Debug.Log($"subUserDataRequest == Success {result}");
            //            },
            //                error =>
            //                {
            //                    Debug.LogError($"subUserDataRequest == Failed {error}");
            //                }
            //            );

            //            coins_TEXT.text = string.Format("{0:N}", item.Value / 100);

            //        }
            //    }


            //}, error =>
            //{
            //    Debug.Log("Error retrieving user inventory: " + error.ErrorMessage);
            //}
            //);
        }


        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            print("IAP initial success");
            m_StoreController = controller;

            //debug_TEXT.text = "debug: IAP initial success";
            Debug.Log("debug: IAP initial success");
            /// throw new NotImplementedException();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            print("IAP initial failed" + error);

            //debug_TEXT.text = "debug: IAP initial failed: " + error;
            Debug.Log("debug: IAP initial failed: " + error);
            ///throw new NotImplementedException();
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            print("IAP initial failed" + error);

            //debug_TEXT.text = "debug: IAP initial failed: " + error;
            Debug.Log("debug: IAP initial failed: " + error);
            ///throw new System.NotImplementedException();
        }


        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            print("Purchase failed" + failureReason);

            //debug_TEXT.text = "debug: Purchase failed: " + failureReason;
            Debug.Log("debug: Purchase failed: " + failureReason);

            LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
            InstanceRemindPanel.Instance.OpenPanel("購買未完成，\n您未被收取任何費用。");

            ///throw new System.NotImplementedException();
        }
        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;
            print("purchasing");

            if (product == null)
            {
                print("purchase not complete");

                //debug_TEXT.text = "debug: purchase not complete";
                Debug.Log("debug: purchase not complete");

                return PurchaseProcessingResult.Pending;
            }
            else
            {
                switch (product.definition.id)
                {
                    case "dragoncoin3300a":
                        AddDragonCoin(3300); 
                        UpdateLocalCoinDisplay(3300);
                        Debug.Log("debug: purchase 3300DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    case "dragoncoin17000":
                        AddDragonCoin(17000);
                        UpdateLocalCoinDisplay(17000);
                        Debug.Log("debug: purchase 17000DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    case "dragoncoin33000":
                        AddDragonCoin(33000);
                        UpdateLocalCoinDisplay(33000);
                        Debug.Log("debug: purchase 33000DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    case "dragoncoin99000":
                        AddDragonCoin(99000);
                        UpdateLocalCoinDisplay(99000);
                        Debug.Log("debug: purchase 99000DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    case "dragoncoin169000":
                        AddDragonCoin(169000);
                        UpdateLocalCoinDisplay(169000);
                        Debug.Log("debug: purchase 169000DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    case "dragoncoin329000":
                        AddDragonCoin(329000);
                        UpdateLocalCoinDisplay(329000);
                        Debug.Log("debug: purchase 329000DC complete");
                        LoadingManger.Instance.Close_Loading_animator(); //關閉購買轉圈
                        return PurchaseProcessingResult.Complete;

                    default:
                        Debug.Log("does not have this product");
                        break;
                }
            }
            return PurchaseProcessingResult.Pending;
        }

        void UpdateLocalCoinDisplay(int count)
        {
            int currentCoins = ParseCoinTextToInt(coins_TEXT.text);
            currentCoins += count;
            coins_TEXT.text = FormatCoins(currentCoins);
        }

        int ParseCoinTextToInt(string coinText)
        {
            string cleanedText = coinText.Replace(",", "").Replace(".", "");
            if (int.TryParse(cleanedText, out int result))
            {
                return result;
            }
            Debug.LogError("Failed to parse coin text to int");
            return 0;
        }

        string FormatCoins(int coins)
        {
            if (coins / 100 == 0)
                return string.Format("0.{0:00}", coins % 100);
            else
                return string.Format("{0:#,#}.{1:00}", coins / 100, coins % 100);
        }

        string nowTimeStr;
        IEnumerator addHist(int count)
        {
            yield return getTimeFromCloud(); //取得雲端時間

            char[] splitChar = { '-', ' ', ':' };
            string[] nowTimeList = nowTimeStr.Split(splitChar);
            DateTime now = new DateTime(Convert.ToInt32(nowTimeList[0]), Convert.ToInt32(nowTimeList[1]), Convert.ToInt32(nowTimeList[2]), Convert.ToInt32(nowTimeList[3]), Convert.ToInt32(nowTimeList[4]), Convert.ToInt32(nowTimeList[5]));
            string id = PlayerInfoManager.Instance.PlayFabId;


            purchaseHist hist = new purchaseHist();
#if UNITY_ANDROID
                    hist.AddUserData(id, "玩家儲值", "Google Play儲值", count, "龍幣", now);
#elif UNITY_IOS
                    hist.AddUserData(id, "玩家儲值", "IOS儲值", count, "龍幣", now);
#else
            hist.AddUserData(id, "玩家儲值", "Fake store儲值", count, "龍幣", now);
#endif
        }

        private IEnumerator getTimeFromCloud()
        {
            bool isResponseReceived = false;

            var request = new ExecuteFunctionRequest
            {
                FunctionName = "getNowDateTime",
                FunctionParameter = new
                {
                    playfabid = PlayerInfoManager.Instance.PlayFabId,
                }
            };

            PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
            {
                Debug.Log("get nowtime sucsess:" + result.ToString());

                List<string> getRichContainList = result.FunctionResult.ToString().Split(',').ToList();
                nowTimeStr = getRichContainList[0];
                isResponseReceived = true;
            },
            error =>
            {
                Debug.Log("get nowtime failed:" + error.ErrorMessage);
                isResponseReceived = true;
            });

            yield return new WaitUntil(() => isResponseReceived);
        }
        public void SetVIPUI_NOAPI()
        {
            var playerinfo = PlayerInfoManager.Instance.PlayerInfo;
            NowVIP.sprite = playerinfo.vipIcon;
            NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level + 1);
            //更新VIP　LEVEL




            if (tmp_Recharge >= 100000) playerinfo.vip.level = 9;
            else if (tmp_Recharge >= 50000) playerinfo.vip.level = 8;
            else if (tmp_Recharge >= 30000) playerinfo.vip.level = 7;
            else if (tmp_Recharge >= 10000) playerinfo.vip.level = 6;
            else if (tmp_Recharge >= 5000) playerinfo.vip.level = 5;
            else if (tmp_Recharge >= 1000) playerinfo.vip.level = 4;


            NowVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level);
            NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level + 1);
            LobbyEventHandler.CallRefreshVIP();

            if (playerinfo.vip.level <= 3)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[0];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[0]}";
            }
            else if (playerinfo.vip.level == 4)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[1];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[1]}";
            }
            else if (playerinfo.vip.level == 5)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[2];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[2]}";
            }
            else if (playerinfo.vip.level == 6)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[3];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[3]}";
            }
            else if (playerinfo.vip.level == 7)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[4];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[4]}";
            }
            else if (playerinfo.vip.level == 8 && playerinfo.vip.level == 9)
            {
                VIP_exp_Fill_value.fillAmount = (float)tmp_Recharge / (float)Recharge[5];
                Back_VIP_exp.text = $"{tmp_Recharge}/{Recharge[5]}";
            }

        }
        public void SetVIPUI()
        {
            var playerinfo = PlayerInfoManager.Instance.PlayerInfo;
            NowVIP.sprite = playerinfo.vipIcon;
            NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level+1);

            //TODO:顯示下一等級所需經驗條
            var request1 = new GetUserDataRequest
            {
                Keys = new List<string> { "Recharge_VIP" }
            };
            PlayFabClientAPI.GetUserData(request1, result =>
                {
                    if (result.Data.TryGetValue("Recharge_VIP", out UserDataRecord Recharge_VIP_Data))
                    {
                        if (int.TryParse(Recharge_VIP_Data.Value, out int Recharge_VIP_Value))
                        {
                            Debug.Log("Recharge_VIP_Value : "+Recharge_VIP_Data.Value);
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

                StartCoroutine(GetMonthlyData(() =>
                {


                    //更新VIP　LEVEL




                    if (tmp_Recharge >= 100000) playerinfo.vip.level = 9;
                    else if (tmp_Recharge >= 50000) playerinfo.vip.level = 8;
                    else if (tmp_Recharge >= 30000) playerinfo.vip.level = 7;
                    else if (tmp_Recharge >= 10000) playerinfo.vip.level = 6;
                    else if (tmp_Recharge >= 5000) playerinfo.vip.level = 5;
                    else if (tmp_Recharge >= 1000) playerinfo.vip.level = 4;


                    NowVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level);
                    NextVIP.sprite = vipIconData.GetIcon(playerinfo.vip.level + 1);
                    LobbyEventHandler.CallRefreshVIP();

                    Debug.Log("tmp_Recharge :" + tmp_Recharge);
                    if (playerinfo.vip.level<=3){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[0];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[0]}";
                    }
                    else if(playerinfo.vip.level==4){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[1];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[1]}";
                    }
                    else if(playerinfo.vip.level==5){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[2];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[2]}";
                    }
                    else if(playerinfo.vip.level==6){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[3];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[3]}";
                    }
                    else if(playerinfo.vip.level==7){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[4];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[4]}";
                    }
                    else if(playerinfo.vip.level==8 || playerinfo.vip.level==9){
                        VIP_exp_Fill_value.fillAmount=(float)tmp_Recharge/(float)Recharge[5];
                        Back_VIP_exp.text=$"{tmp_Recharge}/{Recharge[5]}";
                    }
                }));

                },
                error =>
                {
                    Debug.LogError("GetUserData : Recharge_VIP_Value Error shop.cs");
                });
        }

        private IEnumerator<bool> GetMonthlyData(Action onComplete)
        {
            var request = new GetUserDataRequest
            {
                PlayFabId=PlayerInfoManager.Instance.PlayFabId,
                Keys = new List<string> { "Monthly_Recharge"}
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
                    if(entry.year==BaseYear&& (entry.month==BaseMonth-1))
                    {
                        tmp_Recharge=tmp_Recharge+entry.total;
                    }
                }
            }
        }

    }
}

[System.Serializable]
public class DCHistoryRecord
{
    public string source;
    public string item;
    public int count;
    public string type;
    public string date;
}

[System.Serializable]
public class DCHistoryWrapper
{
    public List<DCHistoryRecord> records;
}