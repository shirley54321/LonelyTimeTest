using Newtonsoft.Json;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.CloudScriptModels;
using shopNameSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


public class userLevel : MonoBehaviour
{
    public static readonly UnityEvent<int> OnUserLevelChange = new UnityEvent<int>();
    public static readonly UnityEvent<int> OnUserExpChange = new UnityEvent<int>();

    //public GameObject LevelUP;
    public static bool test=false;
    public static userLevel Instance { get; private set; }
    //public TextMeshProUGUI levelText; // 如果使用的是 UI TextMeshPro 元件
    public static float Experience=-1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 如果需要保持该对象在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        if(userData!=null)
        {
            //levelText.text = userData.Experience.ToString() + " / 90000";
            Experience = userData.Experience;
        }
    }
    private class userExpHist
    {
        public int addExpCount { get; set; } //增加的數量
        public int afterAddExp { get; set; } //增加後的數量（超過90000會%90000，不然後期要做大數加法了）
        public string addDate { get; set; } //增加的時間日期
    }

    public static void addUserExp(float exp)
    {
        Instance.StartCoroutine(Instance.addUserExpImple(exp));
    }
    public void addUserLevel(int addLevel)
    {
        StartCoroutine(addUserLvlImple(addLevel));
    }
    public void refreshUserExp()  //這樣監聽userExp的function就可以接收到新值
    {
        StartCoroutine(getNowExp());
    }
    public static void refreshUserLevel()
    {
        Instance.StartCoroutine(Instance.getUserLevel());
    }
    public void refreshUserLevelBtn()
    {
        StartCoroutine(getUserLevel());
    }

    private void UserLevelChange(int addLevel)
    {
        OnUserLevelChange.Invoke(level + addLevel);
    }
    private void UserExpChange(int addExp)
    {
        OnUserExpChange.Invoke(expNow + addExp);
    }

    private int level = 0;
    private int expNow = 0;
    private string nowTimeStr = "";
    private string listOfUpdateExpStr = "";
    
    public static UserData userData;

    IEnumerator addUserExpImple(float exp)
    {
        //detect upgrade level or not and test should get DB or not
        //get now data from "UserLevel"
        yield return getTimeFromCloud();
        yield return getUserLevel();
        //yield return getNowExp();

        //store data to "PlayerLevel", "UserLevelExp", "DCHistory"
        //if ((expNow + exp) / 90000 != 0)
        //{
        //    yield return updateDragonBall((expNow + exp) / 90000);
        //    yield return updateUserLevel((expNow + exp) / 90000);
        //}
        //yield return updateUserExp(exp);
        yield return UpdateUserExperience(exp);
    }

    IEnumerator addUserLvlImple(int addLevel)
    {
        //get now data from "PlayerLevel", "UserLevelExp"
        yield return getTimeFromCloud();
        yield return getUserLevel();
        //yield return getNowExp();

        //store data to "PlayerLevel", "UserLevelExp", "DCHistory"
        yield return updateDragonBall(addLevel);
        yield return updateUserLevel(addLevel);
        yield return updateUserExp(addLevel * 90000); //90000升一級
    }

    IEnumerator UpdateUserExperience(float exp )
    {
        //Debug.Log("expNow" + (expNow + exp)+ "   Experience" + userData.Experience);
        bool isResponseReceived = false;
        OnUserExpChange.Invoke((expNow + (int)exp) % 90000);

        if ((expNow + exp) >= 90000)
        {
            userData.Level += 1;
            userData.Experience = ((expNow + exp) - 90000);
            expNow = 0;
            //LevelUP.SetActive(true);
            test =true;

            updateDragonBall(1);
           
            Invoke("CloseLevelUpUI", 5f);
           

        }
        else
        {
            userData.Experience += exp;
        }
        string jsonString = JsonUtility.ToJson(userData);

        Debug.Log("jsonString"+ jsonString);

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string> { { "NewUserLevel", jsonString } }
        },
        result =>
        {
            Debug.Log("Successfully updated user data");
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
            isResponseReceived = true;
        });
        yield return new WaitUntil(() => isResponseReceived);

    }
    IEnumerator getUserLevel()
    {
        bool isResponseReceived = false;
        List<string> keys = new List<string>();
        keys.Add("NewUserLevel");

        var request = new PlayFab.ClientModels.GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys,
        };

        PlayFabClientAPI.GetUserData(request, result =>
        {
            if (!result.Data.ContainsKey(keys[0]))
            {
                List<string> keys1 = new List<string>();
                keys1.Add("UserLevel");

                var request = new PlayFab.ClientModels.GetUserDataRequest()
                {
                    PlayFabId = PlayerInfoManager.Instance.PlayFabId,
                    Keys = keys1,
                };
                PlayFabClientAPI.GetUserReadOnlyData(request, result =>
                {
                    
                        Debug.Log("get user NEW level sucsess:" + result.ToString());
                        InventoryManager.Instance.UpdateDragonCoin();

                        string jsonString = result.Data[keys1[0]].Value.ToString();
                        jsonString = jsonString.TrimStart('[').TrimEnd(']');

                        // 解析 JSON 字符串
                        userData = JsonUtility.FromJson<UserData>(jsonString);

                        // 访问字段
                        Debug.Log("Level: " + userData.Level);
                        Debug.Log("Experience: " + userData.Experience);
                        Debug.Log("UpgradeDate: " + userData.UpgradeDate);
                        Debug.Log("NextLevelExperience: " + userData.NextLevelExperience);

                        level = userData.Level;
                        expNow = (int)userData.Experience;
                        OnUserLevelChange.Invoke(userData.Level);

                        isResponseReceived = true;
                },
                    error =>
                    {
                        Debug.Log("get user level failed: " + error.ErrorMessage);
                    });

                isResponseReceived = true;
            }
            else
            {
                Debug.Log("get user level sucsess:" + result.ToString());
                InventoryManager.Instance.UpdateDragonCoin();

                string jsonString = result.Data[keys[0]].Value.ToString();
                jsonString = jsonString.TrimStart('[').TrimEnd(']');

                // 解析 JSON 字符串
                userData = JsonUtility.FromJson<UserData>(jsonString);

                // 访问字段
                Debug.Log("Level: " + userData.Level);
                Debug.Log("Experience: " + userData.Experience);
                Debug.Log("UpgradeDate: " + userData.UpgradeDate);
                Debug.Log("NextLevelExperience: " + userData.NextLevelExperience);

                level = userData.Level;
                expNow = (int)userData.Experience;
                OnUserLevelChange.Invoke(userData.Level);

                isResponseReceived = true;
            }

                
        },
        error =>
        {
            Debug.Log("get user level failed: " + error.ErrorMessage);
        });
        yield return new WaitUntil(() => isResponseReceived);
    }

    IEnumerable initUserExp()
    {
        bool isResponseReceived = false;
        string initDataStr = "[]";
        string key = "UserLevelExp";
        List<string> keys = new List<string>();
        keys.Add(key);

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                    {key, initDataStr}
            }
        },
        result =>
        {
            Debug.Log("Successfully initiate user data");
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("Got error to initiate " + key + "data");
            Debug.Log(error.GenerateErrorReport());
            isResponseReceived = true;
        });
        yield return new WaitUntil(() => isResponseReceived);
    }
    IEnumerator getNowExp()
    {
        bool isResponseReceived = false;
        string key = "UserLevelExp";
        List<string> keys = new List<string>();
        keys.Add(key);

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result =>
        {
            Debug.Log("Got user data");
            if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
            {
                Debug.Log("initial " + keys[0]);
                initUserExp();
                expNow = 0;
                OnUserExpChange.Invoke(0);
                listOfUpdateExpStr = "[]";
                isResponseReceived = true;
            }
            else
            {
                string listOfUpgradeDataStr = result.Data[key].Value;  //note: type is string
                listOfUpdateExpStr = listOfUpgradeDataStr;
                if (listOfUpgradeDataStr == "[]") //之前沒有升級記錄
                {
                    expNow = 0;
                    OnUserExpChange.Invoke(0);

                    isResponseReceived = true;
                }
                else
                {
                    Debug.Log("UserLevelExp: " + listOfUpgradeDataStr);
                    listOfUpgradeDataStr = listOfUpgradeDataStr.Substring(1, listOfUpgradeDataStr.Length - 2);  //去掉頭尾的[]
                    listOfUpgradeDataStr = listOfUpgradeDataStr.Replace('"', '\''); //把"替換成'才可以用json的DeserializeObject

                    List<string> listOfUpgradeData = listOfUpgradeDataStr.Split('{').ToList();
                    listOfUpgradeData.RemoveAt(0); //第一個是空字串

                    listOfUpgradeData[0] = listOfUpgradeData[0].Insert(0, "{");
                    if (listOfUpgradeData[0][listOfUpgradeData[0].Length - 1] != '}')
                        listOfUpgradeData[0] = listOfUpgradeData[0].Substring(0, listOfUpgradeData[0].Length - 2); //去掉末尾的,符號

                    userExpHist p = new userExpHist();
                    p = JsonConvert.DeserializeObject<userExpHist>(listOfUpgradeData[0]);
                    expNow = p.afterAddExp;
                    OnUserExpChange.Invoke(p.afterAddExp);
                    isResponseReceived = true;
                }
            }
        }, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

        yield return new WaitUntil(() => isResponseReceived);
    }
    IEnumerator updateUserLevel(float addLevel)
    {
        bool isResponseReceived = false;
        OnUserLevelChange.Invoke(level + (int)(addLevel));

        var request = new ExecuteFunctionRequest
        {
            FunctionName = "upgradePlayerLevel",
            FunctionParameter = new
            {
                playfabid = PlayerInfoManager.Instance.PlayFabId,
                afterAddLevel = level + addLevel
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
        {
            Debug.Log("add user level sucsess:" + result.ToString());
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("add user level failed:" + error.ErrorMessage);
            isResponseReceived = true;
        });

        yield return new WaitUntil(() => isResponseReceived);
    }
    IEnumerator updateUserExp(int exp)
    {
        bool isResponseReceived = false;
        OnUserExpChange.Invoke((expNow + exp) % 90000);
        string key = "UserLevelExp";
        List<string> keys = new List<string>();
        keys.Add(key);


        userExpHist p = new userExpHist()
        {
            addExpCount = exp,
            afterAddExp = (expNow + exp) % 90000,
            addDate = nowTimeStr
        };
        string value = JsonConvert.SerializeObject(p);
        if (listOfUpdateExpStr != "[]") //曾經有過資料，加上", "做間隔
            value = "[" + value + ", " + listOfUpdateExpStr.Substring(1, listOfUpdateExpStr.Length - 1);
        else
            value = "[" + value + "]";

        Debug.Log("" + value);
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
                {key, value}
            }
        },
        result =>
        {
            Debug.Log("Successfully updated user data");
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("Got error updating data " + key);
            Debug.Log(error.GenerateErrorReport());
            isResponseReceived = true;
        });
        yield return new WaitUntil(() => isResponseReceived);
    }
    IEnumerator updateDragonBall(float count)
    {
        bool isResponseReceived = false;
        //add hist
        char[] splitChar = { '-', ' ', ':' };
        string[] nowTimeList = nowTimeStr.Split(splitChar);
        DateTime now = new DateTime(Convert.ToInt32(nowTimeList[0]), Convert.ToInt32(nowTimeList[1]), Convert.ToInt32(nowTimeList[2]), Convert.ToInt32(nowTimeList[3]), Convert.ToInt32(nowTimeList[4]), Convert.ToInt32(nowTimeList[5]));
        string id = PlayerInfoManager.Instance.PlayFabId;

        purchaseHist hist = new purchaseHist();
        hist.AddUserData(id, "玩家升級", "玩家升級獎勵", count, "龍珠", now);

        //add currency
        var request = new ExecuteFunctionRequest
        {
            FunctionName = "AddCoins",
            FunctionParameter = new
            {
                playfabid = id,
                virtualcurrency = "DB",
                amount = count
            }
        };

        PlayFabCloudScriptAPI.ExecuteFunction(request, result =>
        {
            Debug.Log("add coins sucsess:" + result.ToString());
            InventoryManager.Instance.UpdateDragonBall();
            isResponseReceived = true;
        },
        error =>
        {
            Debug.Log("add coins failed:" + error.ErrorMessage);
            isResponseReceived = true;
        });
        yield return new WaitUntil(() => isResponseReceived);
    }

    IEnumerator getTimeFromCloud()
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
        });

        yield return new WaitUntil(() => isResponseReceived);
    }

    void CloseLevelUpUI()
    {
        //LevelUP.SetActive(false);
        test=false;
    }
}

