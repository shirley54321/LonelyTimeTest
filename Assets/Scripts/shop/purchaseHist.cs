using Player;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using TMPro;
using Games.SelectedMachine.Star;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.ComponentModel;
using UnityEngine.SceneManagement;
using PlayFab.CloudScriptModels;

namespace shopNameSpace
{
    public class purchaseHist : MonoBehaviour
    {
        [SerializeField] private GameObject scrollContainer;
        [SerializeField] private GameObject list;
        void Start()
        {
            destroy();
            display();
        }

        private class purchaseData
        {
            public string source { get; set; }
            public string item { get; set; }
            public int count { get; set; }
            public string type { get; set; }
            public string date { get; set; }
        }

        private bool canDisplay(string str)
        {
            purchaseData p = new purchaseData();
            p = JsonConvert.DeserializeObject<purchaseData>(str);

            //covert string to datetime string list
            string date = p.date;
            char[] splitChar = { '-', ' ', ':' };
            string[] words = date.Split(splitChar);

            DateTime timeReceipt = new DateTime(Int32.Parse(words[0]), Int32.Parse(words[1]), Int32.Parse(words[2]), Int32.Parse(words[3]), Int32.Parse(words[4]), Int32.Parse(words[5]));
            DateTime timeNow = DateTime.Now;
            double subResult = timeNow.Subtract(timeReceipt).TotalDays; //減去後的天數

            if (subResult > 30) //超過三十天不顯示
            {
                return false;
            }
            return true;
        }

        private void setContainerSise(int height)
        {
            var rectTransform = scrollContainer.GetComponent<RectTransform>();
            if (height <= 500)
                ; //do nothing
            else if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            }
        }

        private void initUserData(string key)
        {
            string initDataStr = "[]";

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>() {
                    {key, initDataStr}
                }
            },
            result => Debug.Log("Successfully initiate user data"),
            error =>
            {
                Debug.Log("Got error to initiate " + key + "data");
                Debug.Log(error.GenerateErrorReport());
            });
        }

        private void GetUserData(string myPlayFabId, List<string> keys) //get history data, item by item
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = myPlayFabId,
                Keys = keys
            }, result =>
            {
                Debug.Log("Got user data:");
                if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
                {
                    Debug.Log("initial " + keys[0]);
                    initUserData(keys[0]);
                    System.Threading.Thread.Sleep(1000);
                    GetUserData(myPlayFabId, keys);
                }
                else
                {
                    string listOfPurchaseDataStr = result.Data[keys[0]].Value;  //note: type is string
                    if (listOfPurchaseDataStr == "[]")  //沒有購買記錄
                        print("no data");
                    else
                    {
                        Debug.Log("DCHistory: " + listOfPurchaseDataStr);
                        listOfPurchaseDataStr = listOfPurchaseDataStr.Substring(1, listOfPurchaseDataStr.Length - 2);  //去掉頭尾的[]

                        List<string> listOfPurchaseData = listOfPurchaseDataStr.Split('{').ToList();
                        listOfPurchaseData.RemoveAt(0); //第一個是空字串

                        for (int i = 0; i < listOfPurchaseData.Count; i++)
                        {
                            listOfPurchaseData[i] = listOfPurchaseData[i].Insert(0, "{");
                            if (listOfPurchaseData[i][listOfPurchaseData[i].Length - 1] != '}')
                                listOfPurchaseData[i] = listOfPurchaseData[i].Substring(0, listOfPurchaseData[i].Length - 2); //去掉最後一字‘,’符號
                        }
                        foreach (string key in listOfPurchaseData)
                        {
                            print(key);
                        }

                    }
                }
            }, (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });
        }

        public void AddUserData(string myPlayFabId, string Source, string Item, float Count, string Type, DateTime now) //加item進history 
                                                                                                        //input data format see K2B-07-存摺v1.0
        {
            purchaseData p = new purchaseData()
            {
                source = Source,
                item = Item,
                count = (int)Count,
                type = Type,
                date = string.Format("{0:20yy-MM-dd HH:mm:ss}", now) //文件上的格式規定 //"2023-10-25 22:00:00"
            };
            string value = JsonConvert.SerializeObject(p);
            string key = "DCHistory";
            List<string> keys = new List<string>();
            keys.Add(key);
            updateRecharge((int)Count);
            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = myPlayFabId,
                Keys = keys
            }, result =>
            {
                Debug.Log("Got user data");
                if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
                {
                    Debug.Log("initial " + keys[0]);
                    initUserData(keys[0]);
                    System.Threading.Thread.Sleep(100); //給它時間initial
                    AddUserData(myPlayFabId, Source, Item, Count, Type, now); //做剛剛沒做的add
                }
                else
                {
                    string listOfPurchaseDataStr = result.Data[key].Value;  //note: type is string
                    if (listOfPurchaseDataStr == "[]") //之前沒有儲值記錄
                    {
                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
                        {
                            Data = new Dictionary<string, string>() {
                            {key, "[" + value + "]"}
                        }
                        },
                        result =>
                            Debug.Log("Successfully updated user data"),
                        error =>
                        {
                            Debug.Log("Got error updating data " + key);
                            Debug.Log(error.GenerateErrorReport());
                        });
                    }
                    else
                    {
                        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
                        {
                            Data = new Dictionary<string, string>() {
                            {key, "[" + value + ", " + listOfPurchaseDataStr.Substring(1, listOfPurchaseDataStr.Length-1)}
                        }
                        },
                        result =>
                            Debug.Log("Successfully updated user data"),
                        error =>
                        {
                            Debug.Log("Got error updating data " + key);
                            Debug.Log(error.GenerateErrorReport());
                        });
                    }
                }
            }, (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });
        }
        private void updateRecharge(int Count){
            var request1 = new GetUserDataRequest
            {
                Keys = new List<string> { "Recharge_VIP","Recharge_Activity" } 
            };

            PlayFabClientAPI.GetUserData(request1, result =>
            {
                // 檢查 Recharge 有無數據
                if (result.Data.TryGetValue("Recharge_VIP", out UserDataRecord Recharge_VIP_data))
                {
                    if (int.TryParse(Recharge_VIP_data.Value, out int currentRecharge_VIP))
                    {
                        int newRecharge = currentRecharge_VIP + (Count/100);
                        var request2 = new UpdateUserDataRequest
                        {
                            Data = new Dictionary<string, string>
                            {
                                { "Recharge_VIP", newRecharge.ToString() }
                            }
                        };

                        PlayFabClientAPI.UpdateUserData(request2, updateResult =>
                        {
                            //Debug.Log("UpdateUserData : Recharge_VIP OK");
                        },
                        updateError =>
                        {
                            Debug.LogError("UpdateUserData : Recharge_VIP Error");
                        });
                    }
                    else
                    {
                        Debug.LogError("Failed to parse Recharge value to integer");
                    }

                }
                if (result.Data.TryGetValue("Recharge_Activity", out UserDataRecord Recharge_Activity_data))
                {
                    if (int.TryParse(Recharge_Activity_data.Value, out int currentRecharge_Activity))
                    {
                        int newRecharge = currentRecharge_Activity + (Count/100);
                        var request2 = new UpdateUserDataRequest
                        {
                            Data = new Dictionary<string, string>
                            {
                                { "Recharge_Activity", newRecharge.ToString() }
                            }
                        };

                        PlayFabClientAPI.UpdateUserData(request2, updateResult =>
                        {
                            //Debug.Log("UpdateUserData : Recharge_Activity OK");
                        },
                        updateError =>
                        {
                            Debug.LogError("UpdateUserData : Recharge_Activity Error");
                        });
                    }
                    else
                    {
                        Debug.LogError("Failed to parse Recharge value to integer");
                    }
                }
            },
            error =>
            {
                Debug.LogError("GetUserData : Recharge Error");
            });
        }
        private void init()
        {
            initUserData("DCHistory");
        }

        public void refresh(int a)
        {
            destroy();
            display();
        }

        public void OnEnable()
        {
            InventoryManager.OnDragonCoinChange.AddListener(refresh);
        }

        public void OnDisable()
        {
            InventoryManager.OnDragonCoinChange.RemoveListener(refresh);
        }

        private void destroy()
        {
            var lists = scrollContainer.GetComponentsInChildren<Transform>();
            for(int i = 1; i < lists.Length; i++)
            {
                Destroy(lists[i].gameObject);
            }
        }

        public void display()
        {
            int i;
            string id = PlayerInfoManager.Instance.PlayFabId;
            print("id: " + id);

            List<string> keys = new List<string>();
            keys.Add("DCHistory");

            PlayFabClientAPI.GetUserData(new GetUserDataRequest()
            {
                PlayFabId = id,
                Keys = keys
            }, result =>
            {
                Debug.Log("Got user data:");
                if (result.Data == null || !result.Data.ContainsKey(keys[0])) //如果之前沒有呼叫過會沒有資料
                {
                    Debug.Log("initial " + keys[0]);
                    initUserData(keys[0]);
                    System.Threading.Thread.Sleep(100);
                    GetUserData(id, keys);
                }
                else
                {
                    string listOfPurchaseDataStr = result.Data[keys[0]].Value;  //note: type is string
                    if (listOfPurchaseDataStr == "[]")  //沒有購買記錄
                    {
                        print("no data");
                        //history0.text = "no data";
                    }
                    else
                    {
                        Debug.Log("DCHistory: " + listOfPurchaseDataStr);
                        listOfPurchaseDataStr = listOfPurchaseDataStr.Substring(1, listOfPurchaseDataStr.Length - 2);  //去掉頭尾的[]
                        listOfPurchaseDataStr = listOfPurchaseDataStr.Replace('"', '\''); //把"替換成'才可以用json的DeserializeObject

                        List<string> listOfPurchaseData = listOfPurchaseDataStr.Split('{').ToList();
                        listOfPurchaseData.RemoveAt(0); //第一個是空字串

                        for (int i = 0; i < listOfPurchaseData.Count; i++)
                        {
                            listOfPurchaseData[i] = listOfPurchaseData[i].Insert(0, "{");
                            if (listOfPurchaseData[i][listOfPurchaseData[i].Length - 1] != '}')
                                listOfPurchaseData[i] = listOfPurchaseData[i].Substring(0, listOfPurchaseData[i].Length - 2); //去掉末尾的,符號
                        }

                        purchaseData p = new purchaseData();
                        int historyCount = listOfPurchaseData.Count;

                        for (i = 0; i < historyCount; i++)
                        {
                            if (canDisplay(listOfPurchaseData[i]))
                                ; //do nothing
                            else
                                break;
                        }
                        historyCount = i;
                        setContainerSise(i * 100 + 100);


                        var textComponents = list.GetComponentsInChildren(typeof(TMP_Text), true); //get list底下的TMP text的components
                        for (int i = 0; i < historyCount; i++)
                        {
                            p = JsonConvert.DeserializeObject<purchaseData>(listOfPurchaseData[i]);

                            //Type A
                            TMP_Text TypeA = textComponents[0].GetComponent<TMP_Text>(); //取得第一個component(Type A)的TMP text物件
                            TypeA.text = p.source;

                            //Type B
                            TMP_Text TypeB = textComponents[1].GetComponent<TMP_Text>(); //取得Type B的TMP text物件
                            TypeB.text = p.item;

                            //Coin
                            TMP_Text Coin = textComponents[2].GetComponent<TMP_Text>(); //取得Coin的TMP text物件
                            Coin.text = p.count.ToString() + p.type;

                            //Time
                            TMP_Text Time = textComponents[3].GetComponent<TMP_Text>(); //取得Time的TMP text物件
                            Time.text = p.date;

                            Instantiate(list, scrollContainer.transform); //生成一個list的prefab
                        }
                    }
                }
            }, (error) =>
            {
                Debug.Log("Got error retrieving user data:");
                Debug.Log(error.GenerateErrorReport());
            });

        }
    }
}
