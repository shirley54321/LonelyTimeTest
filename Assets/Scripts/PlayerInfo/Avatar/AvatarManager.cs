using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Loading;
using PlayFab;
using PlayFab.ClientModels;
using Share.Tool;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    /// <summary>
    /// Get, Upload, Pick Player Avatar
    /// </summary>
    public class AvatarManager : MonoBehaviour
    {
        private int maxImageSize = 1000000;
        
        [SerializeField] private DefaultAvatarData defaultAvatarData;
        [SerializeField] private UrlImageGetter urlImageGetter;
        [SerializeField] private AzureBlobUploader azureBlobUploader;
        
        [SerializeField] private Avatar buildingAvatar;
        [SerializeField] private Avatar uploadingAvatar;


        #region Event

        public readonly UnityEvent<Avatar> OnGetAvatarWithUrl = new UnityEvent<Avatar>();
        
        public UnityEvent<Avatar> OnPickedImage;

        public UnityEvent<Avatar> OnUploadUrlToPlayFabSuccess = new UnityEvent<Avatar>();
        
        #endregion


        #region Subscribe Event

        private void OnEnable()
        {
            azureBlobUploader.OnAvatarUploadSuccess.AddListener(UploadUrlToPlayFab);
        }

        private void OnDisable()
        {
            azureBlobUploader.OnAvatarUploadSuccess.RemoveListener(UploadUrlToPlayFab);
        }

        #endregion

        #region Pick Image in local

        public void PickImage()
        {
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(path, maxImageSize, false);
                    
                    if (texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height) , Vector2.zero);
                        sprite.name = Path.GetFileName(path);
                        
                        var avatar = new Avatar()
                        {
                            Sprite = sprite,
                            IsDefault = false
                        };
                        
                        OnPickedImage.Invoke(avatar);
                    }
                    else
                    {
                        Debug.Log("Couldn't load texture from " + path);
                    }
                }
            }, "Select a PNG image", "image/png");
        }

        #endregion

        #region Upload Avatar

        public void UploadImageToStorage(Avatar avatar)
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            uploadingAvatar = avatar;
            azureBlobUploader.UploadImageToBlobStorage(avatar.Sprite.texture);
        }

        public void UploadUrlToPlayFab(Avatar avatar)
        {
            LoadingManger.Instance.Open_Loading_animator();
            
            uploadingAvatar = avatar;
            UploadUrlToPlayFab(avatar.Url);
        }

        private void UploadUrlToPlayFab(string url)
        {
            PlayFabClientAPI.UpdateAvatarUrl(new UpdateAvatarUrlRequest()
            {
                ImageUrl = url
            }, (result) =>
            {
                Debug.Log($"upload avatar url to PlayFab success");
                uploadingAvatar.Url = url;
                OnUploadUrlToPlayFabSuccess.Invoke(uploadingAvatar);
                PlayerInfoManager.Instance.SetLocalAvatar(uploadingAvatar);
                LoadingManger.Instance.Close_Loading_animator();
            }, (error) =>
            {
                ServerEventHandler.Call_Server_Error_Event(error);
                Debug.Log($"update avatar fail: {error}");
            });
        }

        #endregion

        #region Get Avatar with url

        public void GetAvatar(string avatarUrl)
        {
            StartCoroutine(StartBuildAvatar(avatarUrl)); 
            
        }


        private IEnumerator StartBuildAvatar(string avatarUrl)
        {
            yield return BuildAvatar(avatarUrl);

            OnGetAvatarWithUrl.Invoke(buildingAvatar);
        }

        private IEnumerator BuildAvatar(string avatarUrl)
        {
            // If player avatarUrl is null, return first default Avatar
            if (string.IsNullOrEmpty(avatarUrl))
            {
                buildingAvatar = defaultAvatarData.Avatars[0];
                yield break;
            }
            
            // Check Whether Player Avatar is default avatar
            foreach (var avatar in defaultAvatarData.Avatars)
            {
                if (avatarUrl == avatar.Url)
                {
                    buildingAvatar = avatar;
                    yield break;
                }
            }
            
            // Get Player Avatar with url
            yield return urlImageGetter.GettingSprite(avatarUrl, "avatar");
            buildingAvatar = new Avatar()
            {
                Url = avatarUrl,
                IsDefault = false,
                Sprite = urlImageGetter.sprite
            };
        }

        #endregion
    }
}