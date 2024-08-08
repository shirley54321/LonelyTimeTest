using Games.Data;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Shared.RemindPanel;
namespace GameSelectedMenu
{
    /// <summary>
    /// UI for game selector in the game lobby
    /// </summary>
    public class UIGameSelector : MonoBehaviour
    {
        public GameObject RemindPanel;
        public static event Action<string> GameDownloadFaild;//遊戲下載失敗事件
        public static event Action<string> GameDownloadSuccess;//遊戲下載成功事件
        RemindPanel NonInteractPanel;
        public GameId GameId;
        public GameSelector gameSelector;
        private bool download;
        [SerializeField] private GameObject newIcon, hotIcon,downloadIcon,loadingIcon,waitForDownloadText;
        [SerializeField] private Toggle loveToggle;
        public Image fillImage;
        public Text Textprogress;
        private float fill = 0f;//下載進度
        void Start()
        {
            //Caching.ClearCache();
            //Addressables.ClearDependencyCacheAsync(GameId.ToString());
            StartCoroutine(CheckSceneDownloaded());
        }

        private void OnEnable()
        {
            GameDownloadFaild += DownloadFaild;//監聽遊戲下載失敗事件
            GameDownloadSuccess += DownloadSuccess;//監聽遊戲下載成功事件




        }

        private void OnDisable()
        {
            GameDownloadFaild -= DownloadFaild;//取消監聽遊戲下載失敗事件
            GameDownloadSuccess -= DownloadSuccess;//取消監聽遊戲下載成功事件
        }
        #region 遊戲下載事件
        /// <summary>
        /// 觸發事件的方法並傳入字串
        /// </summary>
        /// <param name="GameName"></param>
        public static void CallDownloadFaild(string GameName)
        {
            GameDownloadFaild?.Invoke(GameName);
        }

        /// <summary>
        /// 當遊戲下載失敗時，重新亮起下載按鈕
        /// </summary>
        /// <param name="FaildId"></param>
        public void DownloadFaild(string FaildId)
        {
            if (GameId.ToString() == FaildId)
            {
                loadingIcon.SetActive(false);
                downloadIcon.SetActive(true);
                Debug.Log(GameId + " Scene download failed: NO");
            }
        }

        /// <summary>
        /// 觸發事件的方法並傳入字串
        /// </summary>
        /// <param name="GameName"></param>
        public static void CallDownloadSuccess(string GameName)
        {
            GameDownloadSuccess?.Invoke(GameName);
        }

        /// <summary>
        /// 當遊戲下載成功時，關閉進度條
        /// </summary>
        /// <param name="FaildId"></param>
        public void DownloadSuccess(string SuccessId)
        {
            if (GameId.ToString() == SuccessId)
            {
                download = true;
                loadingIcon.SetActive(false);
                Debug.Log(GameId + " Scene download failed: YES");
            }
        }
        #endregion

        private void Update()
        {
            if(DownloadManager.Instance.nowDownload == GameId.ToString())//目前正在下載的遊戲為自己按鈕的遊戲
            {
                if(!loadingIcon.activeSelf)//沒有顯示進度條
                {
                    waitForDownloadText.SetActive(false);
                    loadingIcon.SetActive(true);
                }
                fill = DownloadManager.Instance.fill;
                fillImage.fillAmount = fill;
                Textprogress.text = Mathf.FloorToInt(fill * 100) + "%";
            }
            else if(fill !=0)//自己不是正在下載的遊戲且進度不為0 表示已經下載完了
            {
                download = true;
                loadingIcon.SetActive(false);
            }    
        }
        public IEnumerator CheckSceneDownloaded()
        {
            Debug.Log("startCheckSceneDownloaded");
            long updateLabelSize = 0;
            var async = Addressables.GetDownloadSizeAsync(GameId.ToString());
            yield return async;
            Debug.Log("GameId.ToString() : "+GameId.ToString()+" :"+async.Result);
            if (async.Status == AsyncOperationStatus.Succeeded)
                updateLabelSize = async.Result;
            Addressables.Release(async);
            if (updateLabelSize == 0)//沒有更新檔 且已下載
            {
                download=true;
                downloadIcon.SetActive(false);
            }
            else//沒下載 或有更新檔案
            {
                download = false;

                if (DownloadManager.Instance.waitDownload.Exists(x=>x == GameId))//存在於等待下載序列
                {
                    downloadIcon.SetActive(false);
                    waitForDownloadText.SetActive(true);
                }
                else
                {
                    downloadIcon.SetActive(true);
                    waitForDownloadText.SetActive(false);
                }
            }
        }

        #region Update UI

        public void UpdateUI(GameSelector data)
        {
            gameSelector = data;

            newIcon.SetActive(gameSelector.GameProfile.Mark == Mark.New);
            hotIcon.SetActive(gameSelector.GameProfile.Mark == Mark.Hot);
            
            loveToggle.SetIsOnWithoutNotify(gameSelector.FavoriteGame.IsFavorite);
        }

        #endregion

        #region For Button Methods

        public void SetIsFavorite()
        {
            bool isOn = loveToggle.isOn;
            gameSelector.FavoriteGame.IsFavorite = isOn;
            
            GameSelectedMenuManager.Instance.SetFavoriteGame(gameSelector.GameProfile.GameId, isOn);
        }

        public void EnterGame()
        {
            if (!download)
            {
                downloadIcon.SetActive(false);

                if(DownloadManager.Instance.nowDownload != string.Empty)//有正在下載的物件
                {
                    waitForDownloadText.SetActive(true);
                }
                DownloadManager.Instance.DownloadGame(GameId);
            }
            else
            {
                GameObject GM = GameObject.Find("DownloadManager");

                if (GM.GetComponent<DownloadManager>().nowDownload != "" || GM.GetComponent<DownloadManager>().waitDownload.Count>0)
                {
                    Debug.Log("有正在下載的遊戲");
                    InstanceRemindPanel.Instance.OpenPanel("有正在下載的遊戲");



                    return;
                }
                else Debug.Log("沒有正在下載的遊戲");
                GameSelectedMenuManager.Instance.EnterSelectedMachineLobby(gameSelector.GameID);
            }
        }

        //old code 等確認下載沒問題後刪掉
        //IEnumerator DownloadScene()
        //{
        //    DownloadManager.IncrementDownloadCount();
        //    downloadIcon.SetActive(false);
        //    loadingIcon.SetActive(true);
        //    AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(GameId.ToString());

        //    while (!downloadHandle.IsDone)
        //    {
        //        float progress = downloadHandle.PercentComplete;
        //        fillImage.fillAmount=progress;
        //        Textprogress.text= Mathf.FloorToInt(progress * 100)+"%";
        //        //Debug.Log($"Downloading scene: {progress * 100}%");
        //        yield return null;
        //    }
        //    if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        Debug.Log("Scene is downloaded: YES");
        //        var InitAddressablesAsync = Addressables.InitializeAsync();
        //        yield return InitAddressablesAsync;                
        //        download=true;
        //        loadingIcon.SetActive(false);
        //    }
        //    else
        //    {
        //        loadingIcon.SetActive(false);
        //        downloadIcon.SetActive(true);
        //        Debug.Log("Scene download failed: NO");
        //    }
        //    Addressables.Release(downloadHandle);
        //    DownloadManager.DecrementDownloadCount();
        //}
        #endregion
    }
}