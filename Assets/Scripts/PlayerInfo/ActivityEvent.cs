using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player;
using UnityEngine.Networking;
namespace Activity{
    public class ActivityEvent : MonoBehaviour
    {
        private Coroutine totalPlaytimeCoroutine;
        private static ActivityEvent instance;
        public static ActivityEvent Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ActivityEvent>();

                    if (instance == null)
                    {
                        Debug.LogError($"The GameObject with {typeof(ActivityEvent)} does not exist in the scene, " +
                                        $"yet its method is being called.\n" +
                                        $"Please add {typeof(ActivityEvent)} to the scene.");
                    }
                }

                return instance;
            }
        }    
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Login" || scene.name == "Start")
            {
                if (totalPlaytimeCoroutine != null)
                {
                    StopCoroutine(totalPlaytimeCoroutine);
                    totalPlaytimeCoroutine = null;
                }
            }
            else if (scene.name == "GameLobby")
            {
                DoEvent("WeeklyLogin");
                if (totalPlaytimeCoroutine == null)
                {
                    totalPlaytimeCoroutine = StartCoroutine(TotalPlaytimeADD());
                }
            }
        }

        //key
        //GiftExchangeValue 收贈禮累計次數 呼叫一次+1
        //TotalPlaytime  累計遊玩時間呼叫一次+1
        //WeeklyLogin 當周登入 當日設為1 [0,0,0,0,0,0,0] 日~六 if 今天星期三 >>[0,0,0,1,0,0,0]
        //LimitedTimeEvent 限時活動 呼叫設為1
        //Reported 檢舉 呼叫設為1

        public void DoEvent(string key){
            var playerinfo = PlayerInfoManager.Instance.PlayerInfo;
            string playerID = PlayerInfoManager.Instance.PlayFabId;
            string baseUrlA = "https://playerinfo.azurewebsites.net/api/ActivityEvent?";
            string col1 = playerID; //"ABC00001";
            string col2 = key; //"703EC3D5066E9A3C";
            string fullUrl = baseUrlA +
                "&param1=" + col1 +
                "&param2=" + col2;
            UnityWebRequest www = UnityWebRequest.Get(fullUrl);
            www.SendWebRequest();    
        }

        private IEnumerator TotalPlaytimeADD()
        {
            while (true)
            {
                yield return new WaitForSeconds(60f);
                DoEvent("TotalPlaytime");
            }
        }    
    }
}

