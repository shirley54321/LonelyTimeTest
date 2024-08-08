using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Threading.Tasks;
using PlayFab;
using UserAccount;
using PlayFab.ClientModels;
using Loading;
using TMPro;

namespace PlayerStatus{
    public class MultipleLogins : MonoBehaviour
    {
        private string DeviceCode;
        private string PlayerID;
        private int a=0;
        public GameObject Remind_Panel;
        public TextMeshProUGUI Remind_Text;
        private Coroutine CheckStatusCoroutine;
        private void Awake()
        {
            UserAccountManager.OnLoginSuccessful.AddListener(UPDV);
        }   
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }      
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Login" || scene.name == "Start")
            {
                if (CheckStatusCoroutine != null)
                {
                    StopCoroutine(CheckStatusCoroutine);
                    CheckStatusCoroutine = null;
                }
            }
            else
            {
                if (CheckStatusCoroutine == null)
                {
                    DeviceCode=SystemInfo.deviceUniqueIdentifier;
                    PlayerID=PlayFabSettings.staticPlayer.EntityId;
                    Debug.Log("PlayerID :"+PlayerID);
                    Debug.Log("DeviceCode :"+DeviceCode);     
                    CheckStatusCoroutine = StartCoroutine(ExecuteEvery5Seconds());
                }
            }           
        }       
        private async void UPDV(LoginResult result)
        {
            await updataDevice();
        }       
        public async Task updataDevice()
        {
            string baseUrlA = "https://playeronlinestatus.azurewebsites.net/api/updataDevice";
            string codeA = "WMLAjJZ5f2zVMH1tnbaGBhZD4NPGe-HikTxAdOlduJj8AzFuzmMO_Q==";
            string col1=PlayFabSettings.staticPlayer.EntityId;
            string col2=SystemInfo.deviceUniqueIdentifier;
            string fullUrl = baseUrlA + "?code=" + codeA + "&param1=" + col1+ "&param2=" + col2;

            UnityWebRequest request = UnityWebRequest.Get(fullUrl);
            await Task.Yield();
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            Debug.Log("設備資料更新完成");
        }

        private IEnumerator ExecuteEvery5Seconds()
        {
            while (true)
            {
                yield return new WaitForSeconds(5.0f);       
                CheckStatus();
            }
        }
        public async void CheckStatus(){
            string baseUrlA = "https://playeronlinestatus.azurewebsites.net/api/CheckDevice";
            string codeA = "WMLAjJZ5f2zVMH1tnbaGBhZD4NPGe-HikTxAdOlduJj8AzFuzmMO_Q==";
            string col1=PlayFabSettings.staticPlayer.EntityId;
            string col2=SystemInfo.deviceUniqueIdentifier;
            string fullUrl = baseUrlA + "?code=" + codeA + "&param1=" + col1+ "&param2=" + col2;


            UnityWebRequest request = UnityWebRequest.Get(fullUrl);
            await Task.Yield();
            request.SendWebRequest();
            while (!request.isDone)
            {
                await Task.Yield();
            }
            string responseText = request.downloadHandler.text;
            //Debug.Log("登入設備檢查完成 : "+responseText);
            if (responseText.Contains("NO"))
            {
                Debug.Log("設備不一致 : "+responseText);
                PlayFabClientAPI.ForgetAllCredentials();
                UserAccountManager.Instance.ClearQuickLoginId();
                UIManager.Instance.ClearDictionary();
                SceneManager.LoadScene("Login");
                Remind_Text.text="您於其他裝置登入，如有帳號問題請立即反應至客服";
                Remind_Panel.SetActive(true);
            }
            else if (responseText.Contains("Found"))
            {
                Debug.Log("設備未被登入進資料庫 : "+responseText);
                LoadingManger.instance.SetLoadingNormolScreen("Login");
                Remind_Text.text="帳號為外部資料";
                Remind_Panel.SetActive(true);
            }            
        }

    }

}
