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

    public List<GameId> waitDownload = new List<GameId>();//���ݧǦC
    public string nowDownload;//�{�b�U�����C��
    public float fill=0f;//�U���i��


    /// <summary>
    /// �U���C��
    /// </summary>
    /// <param name="gameId"></param>
    public void DownloadGame(GameId gameId)
    {
        if (nowDownload == string.Empty)//�S�����b�U�����C��
        {
            if(waitDownload.Count ==0)//�S�����ݤU�����C��
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
            waitDownload.Add(gameId);//�[�J���ݤU��
        }
    }

    /// <summary>
    /// bundle�]�U��
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

        //�p�G�����ݧǦC�A�d�ݲĤ@�ӬO�_��{�b�U�����C���W�r�ۦP�A�O�N�R���A�N��w�g�U������
        if(waitDownload.Count!=0)
        {
            if(waitDownload[0].ToString() == nowDownload)
            {
                waitDownload.RemoveAt(0);
            }
        }
        nowDownload = string.Empty;//�M���{�b�U�����W�r�A�H�Q�U���U��

        if (waitDownload.Count > 0)//�p�G�٦��U���ǦC
        {
            DownloadGame(waitDownload[0]);
        }
    }

}
