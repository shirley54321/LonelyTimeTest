using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using Player;
using System;

public class SystemMessageManager : MonoBehaviour
{
    #region Instance(Singleton)
    private static SystemMessageManager instance;

    public static SystemMessageManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SystemMessageManager>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject with {typeof(SystemMessageManager)} does not exist in the scene, " +
                                   $"yet its method is being called.\n" +
                                   $"Please add {typeof(SystemMessageManager)} to the scene.");
                }
                DontDestroyOnLoad(instance);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    #endregion

    public SystemMessageList systemMessages = new SystemMessageList();
    void Start()
    {
        GetUserData();
    } // start
    void Update()
    {
    }
    /// <summary>
    /// 新增訊息到玩家的系統訊息列表
    /// </summary>
    /// <param name="message">新訊息</param>
    /// <param name="messageTime">訊息發送時間</param>
    public void AddUserData( string message, DateTime messageTime)
    {
        List<string> keys = new List<string> { "SystemMessage" };
        SystemMessage newMassage=new SystemMessage();
        newMassage.text = message;
        newMassage.Time = messageTime.ToString("yyyy-MM-dd HH:mm:ss");
        systemMessages.messages.Add(newMassage);
        string updatedData = JsonUtility.ToJson(systemMessages);

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string> { { keys[0], updatedData } }
        },
        result => 
        {
            Debug.Log("Successfully updated user data");

        },
        error =>
        {
            Debug.Log("Got error to update " + keys[0] + "data");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    private void GetUserData()
    {   // 從playfab中獲取訊息
        PlayFabClientAPI.GetUserData(new GetUserDataRequest() { 
            PlayFabId = PlayerInfoManager.Instance.PlayFabId,
            Keys = null
        }, OnTitleDataSuccess, OnTitleDataFailure);
    } // GetTitleData

    private void OnTitleDataSuccess(GetUserDataResult result)
    {   // 成功獲取訊息
        if (result.Data == null || !result.Data.ContainsKey("SystemMessage"))
        {   // 但訊息為空
            Debug.Log("沒有找到系統訊息");
            return;
        }

        string systemMessagesJson = result.Data["SystemMessage"].Value;
        systemMessages = JsonUtility.FromJson<SystemMessageList>(systemMessagesJson);

        foreach (var message in systemMessages.messages)
        {
            Debug.Log("系統消息: " + message.text+" "+message.Time);
        }
    }

    private void OnTitleDataFailure(PlayFabError error)
    {   // 獲取訊息失敗
        Debug.LogError("與playfab的訊息取得發生問題: " + error.GenerateErrorReport());
    }
    [System.Serializable]
    public class SystemMessage
    {
        public string text;
        public string Time;
    }

    [System.Serializable]
    public class SystemMessageList
    {
        public List<SystemMessage> messages;
    }
}


