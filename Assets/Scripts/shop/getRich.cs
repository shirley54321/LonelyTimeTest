using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;
using Player;
using System;
using System.Linq;
using PlayFab.CloudScriptModels;
using shopNameSpace;
using Unity.Services.Authentication;
using System.Threading;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Threading.Tasks;
using System.Net;
using Loading;


public class getRich : MonoBehaviour
{
    [SerializeField] private Text countdownText;
    [SerializeField] private Button GetRichButton;
    [SerializeField] private Image getRichImage;
    [SerializeField] private TextMeshProUGUI coinText;

    // Start is called before the first frame update
    void Start()
    {
        //一開始就設成false不然有時候會被卡bug領福利金
        GetRichButton.interactable = false;
        getRichImage.color = Color.gray;

        //initial getRichBtn上面文字
        retrieveCount((remainingCount) =>
        {
            if (remainingCount > 0)
            {
                refreshPlayfab(1);
            }
            else
            {
                countdownText.text = "(0/5)";
            }
        });
    }

    IEnumerator countdownTime(int time)
    {
        // 確保在倒數開始時按鈕被設置為不可點擊
        GetRichButton.interactable = false;
        getRichImage.color = Color.gray;

        while (time > 0)
        {
            countdownText.text = string.Format("{0:00}:{1:00}", time / 60, time % 60);
            time--;
            yield return new WaitForSecondsRealtime(1);
        }

        //do time = 0
        StartCoroutine(refresh(0));
    }

    private IEnumerator refresh(int sub)
    {
        yield return getTimeFromCloud(); // 取得雲端時間

        DateTime now = DateTime.Parse(nowTimeStr); // 解析雲端時間
        Debug.Log("Cloud time: " + now);

        int nowDC = PlayerInfoManager.Instance.PlayerInfo.dragonCoin; // 獲取當前玩家的龍幣數量
        List<string> keys = new List<string> { "getRich" }; // 定義要獲取的玩家數據鍵值

        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result =>
        {
            if (result.Data == null || !result.Data.ContainsKey(keys[0]))
            {
                Debug.Log("Initializing " + keys[0]);
                initPlayfab(() => StartCoroutine(refresh(0)));
            }
            else
            {
                string getRichData = result.Data[keys[0]].Value;
                Debug.Log("getRichData: " + getRichData);
                List<string> getRichDataList = getRichData.Split(',').ToList();

                DateTime lastReceiptTime = DateTime.Parse(getRichDataList[0]);
                int remainingCount = int.Parse(getRichDataList[1]);

                Debug.Log($"Last receipt time: {lastReceiptTime}, Remaining count: {remainingCount}");

                if (now.Date != lastReceiptTime.Date)
                {
                    remainingCount = 5;
                    lastReceiptTime = now.AddSeconds(-301); // 將上次領取時間設置為當前時間的300秒之前
                    Debug.Log("New day detected, resetting remaining count to 5.");
                }

                double secondsSinceLastReceipt = (now - lastReceiptTime).TotalSeconds;
                bool canReceiveNow = secondsSinceLastReceipt >= 300;

                Debug.Log($"Seconds since last receipt: {secondsSinceLastReceipt}, Can receive now: {canReceiveNow}");

                UpdateUI(remainingCount, nowDC < 10000 && canReceiveNow);

                if (remainingCount > 0 && !canReceiveNow)
                {
                    StartCoroutine(countdownTime((int)(300 - secondsSinceLastReceipt)));
                }
            }
        }, error =>
        {
            Debug.LogError("Got error retrieving user data:");
            Debug.LogError(error.GenerateErrorReport());
        });

        testDCLower100();
    }



    /// <summary>
    /// 更新玩家數據
    /// </summary>
    /// <param name="lastReceiptTime">上次領取時間</param>
    /// <param name="remainingCount">剩餘可領取次數</param>
    private void UpdateUserData(DateTime lastReceiptTime, int remainingCount)
    {
        List<string> keys = new List<string> { "getRich" };
        string updatedData = lastReceiptTime.ToString("yyyy-MM-dd HH:mm:ss") + "," + remainingCount;
        Debug.Log("Updating user data: " + updatedData);

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string> { { keys[0], updatedData } }
        },
        result => Debug.Log("Successfully updated user data: " + updatedData),
        error =>
        {
            Debug.Log("Got error to update " + keys[0] + "data");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private void refreshPlayfab(int temp)
    {
        List<string> keys = new List<string> { "getRich" };

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result =>
        {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
            {
                Debug.Log("initial " + keys[0]);

                //init getRich data
                initPlayfab(() =>
                {
                    System.Threading.Thread.Sleep(1000); //等一秒
                    refreshPlayfab(1); //再試試看有沒有init成功
                });
            }
            else
            {
                Debug.Log("Already have data " + keys[0]);
                StartCoroutine(refresh(0));
            }
        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }


    /// <summary>
    /// 獲取並返回剩餘可領取次數
    /// </summary>
    /// <param name="callback"></param>
    private void retrieveCount(Action<int> callback)
    {
        int maxRetryCount = 3;
        int initialDelayInSeconds = 2;
        MakePlayFabRequestWithRetry(() =>
        {
            List<string> keys = new List<string> { "getRich" };

            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = PlayerInfoManager.Instance.PlayFabId,
                Keys = keys
            }, result =>
            {
                Debug.Log("Got user data:");
                if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
                {
                    Debug.Log("initial " + keys[0]);

                    // 初始化 getRich 數據
                    initPlayfab(() => callback(5)); // 初始化後設置剩餘次數為 5，並調用回調
                }
                else
                {
                    Debug.Log("Refreshing data " + keys[0]);

                    string getRichContainStr = result.Data[keys[0]].Value; // 獲取數據字符串
                    Debug.Log("getRich: " + getRichContainStr);
                    List<string> getRichContainList = getRichContainStr.Split(',').ToList();

                    DateTime lastReceiptTime = DateTime.Parse(getRichContainList[0]); // 上次領取時間
                    int remainingCount = int.Parse(getRichContainList[1]); // 剩餘可領取次數

                    // 檢查是否是新的一天，重置剩餘次數
                    if (DateTime.UtcNow.Date != lastReceiptTime.Date)
                    {
                        remainingCount = 5; // 重置為 5
                    }

                    callback(remainingCount); // 調用回調函數並傳遞剩餘次數
                }
            }, error =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
                callback(0); // 在出錯的情況下返回 0
            });
        }, maxRetryCount, initialDelayInSeconds);
    }

    private void MakePlayFabRequestWithRetry(Action requestAction, int retryCount, int delayInSeconds)
    {
        StartCoroutine(ExecuteWithRetry(requestAction, retryCount, delayInSeconds));
    }

    private IEnumerator ExecuteWithRetry(Action requestAction, int retryCount, int delayInSeconds)
    {
        int attempts = 0;
        while (attempts < retryCount)
        {
            bool isCompleted = false;
            requestAction.Invoke();
            attempts++;

            yield return new WaitForSeconds(delayInSeconds);

            if (isCompleted)
            {
                yield break;
            }
            else
            {
                Debug.LogWarning($"Retrying... Attempts left: {retryCount - attempts}");
                delayInSeconds *= 2; // Exponential backoff
            }
        }

        Debug.LogError("Reached maximum retry attempts.");
    }

    private void initPlayfab(Action callback)
    {
        List<string> keys = new List<string> { "getRich" };
        string initDataStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + ",5"; // 初始化為當前時間和5次可領取次數

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string> { { keys[0], initDataStr } }
        },
        result => {
            Debug.Log("Successfully initiated user data");
            callback?.Invoke(); // 初始化成功後調用回調函數
        },
        error =>
        {
            Debug.Log("Got error to initiate " + keys[0] + " data");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    private bool testDCLower100() //小於100龍幣（playfab存成10000）才可以領
    {
        int nowDC = PlayerInfoManager.Instance.PlayerInfo.dragonCoin;

        if (nowDC < 10000)
        {
            GetRichButton.interactable = true;
            getRichImage.color = Color.white;
            return true;
        }
        else
        {
            GetRichButton.interactable = false;
            getRichImage.color = Color.gray;
            return false;
        }
    }

    private int vipLevelToRichCount(int vipLevel)
    {
        switch (vipLevel)
        {
            case 1:
                return 10000;
            case 2:
                return 10000;
            case 3:
                return 20000;
            case 4:
                return 30000;
            case 5:
                return 50000;
            case 6:
                return 80000;
            case 7:
                return 100000;
            case 8:
                return 150000;
            case 9:
                return 300000;
            default:
                return 0;
        }
    }

    private void addCoins(int count)
    {
        //var request = new ExecuteFunctionRequest
        //{
        //    FunctionName = "AddCoins",
        //    FunctionParameter = new
        //    {
        //        playfabid = PlayerInfoManager.Instance.PlayFabId,
        //        virtualcurrency = "DC",
        //        amount = count * 100
        //    }
        //};
        var req = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "DC",
            Amount = count
        };
        PlayFabClientAPI.AddUserVirtualCurrency(req,
            success =>
            {
                Debug.Log("add coins success:");
                InventoryManager.Instance.UpdateDragonCoin();
                LoadingManger.Instance.Close_Loading_animator();
            },
            error => 
            {
                Debug.Log("add coins fail");
                LoadingManger.Instance.Close_Loading_animator();
            });

        //PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
        //{
        //    Debug.Log("add coins success:" + result.ToString());
        //    InventoryManager.Instance.UpdateDragonCoin(); // 更新 DragonCoin 並觸發事件
        //    LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
        //},
        //error =>
        //{
        //    Debug.Log("add coins failed:" + error.ErrorMessage);
        //});
    }

    private void OnEnable()
    {
        InventoryManager.OnDragonCoinChange.AddListener(UpdateDragonCoinAndUI);
    }

    private void OnDisable()
    {
        InventoryManager.OnDragonCoinChange.RemoveListener(UpdateDragonCoinAndUI);
    }

    private void UpdateDragonCoinAndUI(int dragonCoin)
    {
        // 檢查龍幣是否低於100
        if (dragonCoin < 10000)
        {
            GetRichButton.interactable = true;
            getRichImage.color = Color.white;
        }
        else
        {
            GetRichButton.interactable = false;
            getRichImage.color = Color.gray;
        }

        // 更新錢幣顯示
        if (dragonCoin / 100 == 0)
            coinText.text = string.Format("0.{0:00}", dragonCoin % 100);
        else
            coinText.text = string.Format("{0:#,#}.{1:00}", dragonCoin / 100, dragonCoin % 100);
    }


    string nowTimeStr;

    public void getRichBtnPress()
    {
        nowTimeStr = "";
        LoadingManger.Instance.Open_Loading_animator(); // 等待購買完成轉圈
        StartCoroutine(getRichCoroutine());
    }

    private IEnumerator getRichCoroutine()
    {
        yield return getTimeFromCloud(); // 取得雲端時間

        DateTime now = DateTime.Parse(nowTimeStr);
        List<string> keys = new List<string> { "getRich" };

        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result =>
        {
            if (result.Data == null || !result.Data.ContainsKey(keys[0]))
            {
                initPlayfab(() => StartCoroutine(getRichCoroutine())); // 初始化後再次調用該協程
            }
            else
            {
                string getRichData = result.Data[keys[0]].Value;
                List<string> getRichDataList = getRichData.Split(',').ToList();

                DateTime lastReceiptTime = DateTime.Parse(getRichDataList[0]);
                int remainingCount = int.Parse(getRichDataList[1]);

                double secondsSinceLastReceipt = (now - lastReceiptTime).TotalSeconds;

                if (testDCLower100() == false)
                {
                    InstanceRemindPanel.Instance.OpenPanel("當前龍幣小於100才能夠領取");
                    LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
                }
                else if ((secondsSinceLastReceipt >= 300 && remainingCount > 0) || remainingCount == 5)
                {
                    // 更新領取時間和剩餘次數
                    lastReceiptTime = now;
                    remainingCount--;
                    UpdateUserData(lastReceiptTime, remainingCount);

                    addCoins(vipLevelToRichCount(PlayerInfoManager.Instance.PlayerInfo.vip.level));

                    // Add bank history
                    purchaseHist hist = new purchaseHist();
                    hist.AddUserData(PlayerInfoManager.Instance.PlayFabId, "系統贈金", "福利金", vipLevelToRichCount(PlayerInfoManager.Instance.PlayerInfo.vip.level), "龍幣", now);

                    InstanceRemindPanel.Instance.OpenPanel("已獲得龍幣");

                    // 更新UI
                    UpdateUI(remainingCount, false);
                    if (remainingCount > 0)
                    {
                        StartCoroutine(countdownTime(300));
                    }
                    else
                    {
                        countdownText.text = "(0/5)";
                    }

                    LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
                }
                else if (secondsSinceLastReceipt < 300 && remainingCount > 0)
                {
                    InstanceRemindPanel.Instance.OpenPanel("還未到刷新時間");
                    StartCoroutine(countdownTime((int)(300 - secondsSinceLastReceipt)));
                    LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
                }
                else
                {
                    InstanceRemindPanel.Instance.OpenPanel("當日領取次數已用完");
                    LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
                }
            }
        }, error =>
        {
            Debug.LogError("Got error retrieving user data:");
            Debug.LogError(error.GenerateErrorReport());
            LoadingManger.Instance.Close_Loading_animator(); // 關閉購買轉圈
        });
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
            Debug.Log("get nowtime success: " + result.FunctionResult.ToString());

            List<string> getRichContainList = result.FunctionResult.ToString().Split(',').ToList();
            nowTimeStr = getRichContainList[0];
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("get nowtime failed: " + error.ErrorMessage);
            isResponseReceived = true;
        });

        yield return new WaitUntil(() => isResponseReceived);
        Debug.Log("nowTimeStr: " + nowTimeStr);
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    /// <param name="remainingCount"></param>
    /// <param name="canReceiveNow"></param>
    private void UpdateUI(int remainingCount, bool canReceiveNow)
    {
        countdownText.text = $"({remainingCount}/5)";
        if (remainingCount > 0 && canReceiveNow)
        {
            GetRichButton.interactable = true;
            getRichImage.color = Color.white;
        }
        else
        {
            GetRichButton.interactable = false;
            getRichImage.color = Color.gray;
        }
    }
}