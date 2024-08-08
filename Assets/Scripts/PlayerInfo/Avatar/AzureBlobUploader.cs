
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Loading;
using UnityEngine.Events;

namespace Player
{
    /// <summary>
    /// Upload Player Avatar to Azure Blob Storage
    /// </summary>
    public class AzureBlobUploader : MonoBehaviour
    {
        #region Token and Url

        [SerializeField] private string StorageAccountName;
        [SerializeField] private string ContainerName;
        [SerializeField] private string uploadAvatarUrl = 
            "https://playerinfo.azurewebsites.net/api/UploadAvatar?code=O2xNBnnDib03xlqCvanUwVqUC0iKFWlMzQEeIMZguDZVAzFupex-RA==";
        
        #endregion
        
        #region Image Size

        [SerializeField] private int desiredWidth = 300;
        [SerializeField] private int desiredHeight = 300;
        

        #endregion
        
        #region Event

        public UnityEvent<string> OnAvatarUploadSuccess;
        public UnityEvent<string> OnAvatarUploadFail;

        #endregion
        
        #region Upload Image to Blob Storage

        public void UploadImageToBlobStorage(Texture2D image)
        {
            // TODO LoadingAndNetworkErrorManager.Instance.StartLoading();

            Texture2D resizedTexture = ResizeTexture(image);
            
            byte[] imageData = resizedTexture.EncodeToPNG();
            StartCoroutine(UploadImage(imageData));
        }

        private Texture2D ResizeTexture(Texture2D rawImage)
        {
            RenderTexture rt = new RenderTexture(desiredWidth, desiredWidth, 24);
            Graphics.Blit(rawImage, rt);
            RenderTexture.active = rt;
            Texture2D resizedTexture = new Texture2D(desiredWidth, desiredHeight);
            resizedTexture.ReadPixels(new Rect(0, 0, desiredWidth, desiredHeight), 0, 0);
            resizedTexture.Apply();

            return resizedTexture;
        }
        
        private IEnumerator UploadImage(byte[] imageData)
        {
            string playFabId = PlayerInfoManager.Instance.PlayFabId;

            WWWForm form = new WWWForm();

            form.AddField("playFabId", playFabId); // Add PlayFab ID parameter
            form.AddBinaryData("image", imageData, GetBlobName(), "image/png"); // Add image data and name parameter
            
            // Create UnityWebRequest and use POST method to upload image data
            UnityWebRequest request = UnityWebRequest.Post(uploadAvatarUrl, form); 

            using (request)
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("Image upload to Azure Blob Storage successful!");
                    OnAvatarUploadSuccess.Invoke(GetAvatarUrl());
                }
                else
                {
                    Debug.LogError("Image upload to Azure Blob Storage failed: " + request.error);
                    OnAvatarUploadFail.Invoke(GetAvatarUrl());
                }
            }
        }
        

        #endregion

        #region Get Avatar Url

        private string GetAvatarUrl()
        {
            return $"https://{StorageAccountName}.blob.core.windows.net/{ContainerName}/{GetBlobName()}";
        }

        private string GetBlobName()
        {
            string playFabId = PlayerInfoManager.Instance.PlayFabId ;
            return $"avatar_{playFabId}.png";
        }

        #endregion
    }
}

