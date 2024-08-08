using LitJson;
using Newtonsoft.Json;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Player;
using UnityEngine.UI;
using TMPro;

public class BlackList : MonoBehaviour
{
    //[SerializeField] public Button addListBtn, deleteListBtn;

    public GameObject playBlacklistContent;
    public GameObject playBlacklistPrefab;

    public void Start()
    {
        //press btn to call a function with parameter的方法
        //addListBtn.onClick.AddListener(delegate { StartCoroutine(addToBlackList("111111", "123", "https://playeravatar.blob.core.windows.net/avatar/avatar_2FE5ED78DAE18952.png", 66)); });
        //deleteListBtn.onClick.AddListener(delegate { StartCoroutine(deleteFromBlackList("111111")); });
    }

    class BlackListClass
    {
        public string playfabID { get; set; }
        public string displayName { get; set; }
        public string avatarUrl { get; set; }
        public int activity { get; set; }
    }
    #region Botton
    public void addListBtn()
    {
        StartCoroutine(addToBlackList("111111", "123", "https://playeravatar.blob.core.windows.net/avatar/avatar_2FE5ED78DAE18952.png", 66));
    }
    public void deleteListBtn()
    {
        StartCoroutine(addToBlackList("111111", "123", "https://playeravatar.blob.core.windows.net/avatar/avatar_2FE5ED78DAE18952.png", 66));
    }
    public void DisplayBlackListBtn()
    {
        StartCoroutine(DisplayBlackList());
    }

    #endregion

    List<BlackListClass> blackListClasses = new List<BlackListClass>();

    private void InitialBlackList()
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>() {
            {"BlackList", ""}, //初始什麼都沒有
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => {
            Debug.Log("Got error setting user data Ancestor to Arthur");
            Debug.Log(error.GenerateErrorReport());
        });
    }

    IEnumerator LoadBlackList()
    {
        bool isResponseReceived = false;
        List<string> keys = new List<string>();
        keys.Add("BlackList");

        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = keys
        }, result => {
            Debug.Log("Got user data:");
            if (result.Data == null || !result.Data.ContainsKey("BlackList"))
            {
                Debug.Log("initial BlackList.");
                InitialBlackList();
                isResponseReceived = true;
            }
            else
            {
                Debug.Log("BlackList: " + result.Data["BlackList"].Value);
                blackListClasses = StringToListOfObject(result.Data["BlackList"].Value);
                isResponseReceived = true;
            }

        }, (error) => {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });

        yield return new WaitUntil(() => isResponseReceived);
    }

    IEnumerator storeBlackList()
    {
        bool isResponseReceived = false;

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest() {
            Data = new Dictionary<string, string>() {
                {"BlackList", ListOfObjectToString(blackListClasses)},
            }
        },
        result =>
        {
            Debug.Log("Successfully updated user data");
            isResponseReceived = true;
        },
        error => {
            Debug.Log("Got error setting user data BlackList");
            Debug.Log(error.GenerateErrorReport());
        });

        yield return new WaitUntil(() => isResponseReceived);
    }

    IEnumerator addToBlackList(string id, string name, string avatarurl, int acti)
    {
        yield return LoadBlackList();

        BlackListClass newBlackListPlayer = new BlackListClass {
            playfabID = id,
            displayName = name,
            avatarUrl = avatarurl,
            activity = acti
        };
        blackListClasses.Add(newBlackListPlayer);

        yield return storeBlackList();
    }
    IEnumerator deleteFromBlackList(string deleteID)
    {
        yield return LoadBlackList();

        for(int i = 0; i < blackListClasses.Count; i++)
            if (blackListClasses[i] != null && blackListClasses[i].playfabID == deleteID)
            {
                blackListClasses.RemoveAt(i);
                break;
            }

        yield return storeBlackList();
    }
    IEnumerator DisplayBlackList()
    {
        yield return LoadBlackList();
        UpdatePlayerData();       
    }

    private string ListOfObjectToString(List<BlackListClass> list)
    {
        List<string> strs = new List<string>();
        string result;

        //convert all object to string
        for (int i = 0; i < list.Count; i++)
        {
            strs.Add(JsonConvert.SerializeObject(list[i]));
        }

        //convert list of string to string
        result = String.Join(">->->->", strs.ToArray()); //單純用","的話有人名字包含這個字符很難處理
        return result;
    }
    private List<BlackListClass> StringToListOfObject(string str)
    {
        List<string> strs = new List<string>(str.Split(">->->->"));
        List<BlackListClass> list = new List<BlackListClass>();

        //convert all string to object
        for (int i = 0; i < strs.Count; i++)
        {
            print(strs[i]);
            if(strs[i] != null)
                list.Add(JsonConvert.DeserializeObject<BlackListClass>(strs[i]));
        }

        return list;
    }

    BlacklistManager blacklistManager = new BlacklistManager();
    void UpdatePlayerData()//ChatPagePanel
    {
            for (int i = 0; i < playBlacklistContent.transform.childCount; i++)
                {
                  Transform child = playBlacklistContent.transform.GetChild(i);
                Destroy(child.gameObject);
            }

            Debug.Log("更新黑名單資料");
        blacklistManager.GetBlacklist(blacklist =>
        {
            if (blacklist != null)
            {
                foreach (string id in blacklist)
                {
                    Debug.Log("Blacklisted ID: " + id);


                    GameObject newChild = Instantiate(playBlacklistPrefab, playBlacklistContent.transform);
                    newChild.GetComponent<BlackListOBJ>().PlayFabID = id;


                    //StartCoroutine(SetHeadIconAndName(id, newChild));
                    GetPlayerProfile(id, (displayName, avatarUrl) =>
                    {
                        if (!string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(avatarUrl))
                        {
                            Debug.Log($"DisplayName: {displayName}");
                            Debug.Log($"AvatarUrl: {avatarUrl}");
                            // 继续处理，如在UI中显示
                            StartCoroutine(SetHeadIconAndName(id, displayName, avatarUrl));
                        }
                        else
                        {
                            Debug.LogError("Failed to retrieve player profile.");
                        }
                    });
                }
            }
            else
            {
                Debug.LogError("Failed to retrieve blacklist.");
            }
        });





    }


    IEnumerator SetHeadIconAndName(string id, string name, string avatarUrl)
    {
        for (int i = 0; i < playBlacklistContent.transform.childCount; i++)
        {
            Transform child = playBlacklistContent.transform.GetChild(i);
            if(child.GetComponent<BlackListOBJ>().PlayFabID == id)
            {

                child.GetComponent<BlackListOBJ>().Name = name;
                child.GetComponent<BlackListOBJ>().HeadIconURL = avatarUrl;
                child.GetComponent<BlackListOBJ>().SetHeadIcon();
            }
        }

        yield return null;
    }


    public void GetPlayerProfile(string playFabId, Action<string, string> callback)
    {
        var request = new GetPlayerProfileRequest
        {
            PlayFabId = playFabId,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };

        PlayFabClientAPI.GetPlayerProfile(request, result =>
        {
            string displayName = result.PlayerProfile?.DisplayName;
            string avatarUrl = result.PlayerProfile?.AvatarUrl;

            Debug.Log($"Player DisplayName: {displayName}");
            Debug.Log($"Player AvatarUrl: {avatarUrl}");

            callback(displayName, avatarUrl);
        }, error =>
        {
            Debug.LogError($"Error retrieving player profile: {error.GenerateErrorReport()}");
            callback(null, null);
        });
    }
}
