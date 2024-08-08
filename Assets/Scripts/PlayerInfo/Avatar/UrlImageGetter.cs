using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Share.Tool
{
    /// <summary>
    /// Get Image with Url
    /// </summary>
    public class UrlImageGetter : MonoBehaviour
    {
        public Sprite sprite;
        
        public IEnumerator GettingSprite(string url, string spriteName) {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success) {
                Debug.Log($"Get sprite with {url} fail {www.error}");
            }
            else {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height) , Vector2.zero);
                sprite.name = spriteName;
            }
        }
    }
}