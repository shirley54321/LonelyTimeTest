using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Share.Tool;
public class FriendObj : MonoBehaviour
{
    public Unity.Services.Lobbies.Models.Player player;


    public string id;
    public string Name;
    public string HeadIconURL;
    public string Activity;
    public string VIP;
    public string LEVEL;
    public string DC;
    public string DB;
    public Image HeadIcon;
    public Transform PanelOBJ;
    public GameObject ActivityText;



    private void OnEnable()
    {
        LobbyEventHandler.OpenOtherPlayerInfoPage += OpenOtherPlayerInfoPage;
    }

    private void OnDisable()
    {
        LobbyEventHandler.OpenOtherPlayerInfoPage -= OpenOtherPlayerInfoPage;
    }

    private void OpenOtherPlayerInfoPage(Transform obj)
    {
        PanelOBJ = obj;

    }

    // Start is called before the first frame update
    public void SetValue()
    {

        id = player.Id;
        Name = player.Data["playerName"].Value;
        HeadIconURL = player.Data["myAvatarUrl"].Value;
        Activity = player.Data["activity"].Value;
        VIP = player.Data["vip"].Value;
        LEVEL = player.Data["level"].Value;
        DC = player.Data["DC"].Value;
        DB=player.Data["DB"].Value;
    }

    UrlImageGetter urlImageGetter;
    public void SetHeadIcon(string url)
    {

        StartCoroutine(LoadImageCoroutine(url));
    }

    private IEnumerator LoadImageCoroutine(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error downloading image: {www.error}");
            }
            else
            {
                // 将下载的图片数据转换为Texture2D
                Texture2D texture = DownloadHandlerTexture.GetContent(www);

                // 创建Sprite并应用于UI Image组件
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    HeadIcon.sprite = sprite;
                }
            }
        }
    }

}
