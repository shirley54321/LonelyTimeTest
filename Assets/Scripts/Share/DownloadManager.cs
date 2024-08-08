using Games.Data;
using GameSelectedMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadManager : MonoBehaviour
{
    #region Instance (Singleton)
    private static DownloadManager instance;

    public static DownloadManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DownloadManager>();

                if (instance == null)
                {
                    Debug.LogError($"The GameObject of type {typeof(DownloadManager)} is not present in the scene, " +
                                   $"yet its method is being called. Please add {typeof(DownloadManager)} to the scene.");
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);

            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
    }


    #endregion

    public List<GameId> waitDownload = new List<GameId>();//单
    public string nowDownload;//瞷更笴栏
    public float fill=0f;//更秈


    /// <summary>
    /// 更笴栏
    /// </summary>
    /// <param name="gameId"></param>
    public void DownloadGame(GameId gameId)
    {
        if (nowDownload == string.Empty)//⊿Τタ更笴栏
        {
            if(waitDownload.Count ==0)//⊿Τ单更笴栏
            {
                nowDownload = gameId.ToString();
            }
            else
            {
                nowDownload = waitDownload[0].ToString();
            }
            StartCoroutine(DownloadScreen());
        }
        else
        {
            waitDownload.Add(gameId);//单更
        }
    }

    /// <summary>
    /// bundle更
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadScreen()
    {
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(nowDownload);

        while (!downloadHandle.IsDone)
        {
            float progress = downloadHandle.PercentComplete;
            fill = progress;
            //Debug.Log($"Downloading scene: {progress * 100}%");
            yield return null;
        }
        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            var InitAddressablesAsync = Addressables.InitializeAsync();
            yield return InitAddressablesAsync;
            UIGameSelector.CallDownloadSuccess(nowDownload);
        }
        else
        {
            UIGameSelector.CallDownloadFaild(nowDownload);
        }
        Addressables.Release(downloadHandle);

        //狦Τ单琩材琌蛤瞷更笴栏琌碞埃竒更挡
        if(waitDownload.Count!=0)
        {
            if(waitDownload[0].ToString() == nowDownload)
            {
                waitDownload.RemoveAt(0);
            }
        }
        nowDownload = string.Empty;//睲埃瞷更Ω更

        if (waitDownload.Count > 0)//狦临Τ更
        {
            DownloadGame(waitDownload[0]);
        }
    }

}
