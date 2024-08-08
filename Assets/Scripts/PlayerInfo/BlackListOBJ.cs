using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
public class BlackListOBJ : MonoBehaviour
{
    public string PlayFabID;
    public string Name;
    public string HeadIconURL;
    public Image HeadIcon;
    public GameObject NameText;





    public  void SetHeadIcon()
    {
        NameText.GetComponent<TMP_Text>().text = Name;
        StartCoroutine(LoadImageCoroutine(HeadIconURL));

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
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                HeadIcon.sprite = sprite;
            }
        }
    }
    BlacklistManager blacklistManager = new BlacklistManager();
    public void RemoveBlackListButtonClick()
    {
        blacklistManager.RemoveFromBlacklist(PlayFabID, success =>
        {
            if (success)
            {
                Debug.Log("Successfully remove from blacklist.");
                // 继续其他操作
            }
            else
            {
                Debug.LogError("Failed to remove from blacklist.");
            }
        }
        );
        Destroy(gameObject);
    }

}
