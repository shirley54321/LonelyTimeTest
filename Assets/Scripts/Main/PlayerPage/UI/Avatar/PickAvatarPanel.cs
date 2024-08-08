using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Loading;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.Avatar
{
    /// <summary>
    /// Panel to pick avatar
    /// </summary>
    public class PickAvatarPanel : MonoBehaviour
    {
        [SerializeField] private AvatarManager avatarManager;

        [SerializeField] private Transform spawnTransform;
        [SerializeField] private AvatarOption avatarOptionPrefab;
        [SerializeField] private DefaultAvatarData defaultAvatarData;

        [SerializeField] private List<AvatarOption> avatarOptions;
        [SerializeField] private AvatarOption currentOption;
        public ToggleGroup ToggleGroup;

        // 存儲當前顯示的頭像選項
        private List<Player.Avatar> savedAvatars = new List<Player.Avatar>();
        // 滾動視圖
        [SerializeField] private ScrollRect scrollRect;

        #region Subscribe Event

        private void OnEnable()
        {
            avatarManager.OnPickedImage.AddListener(OnPickedImage);
            avatarManager.OnUploadUrlToPlayFabSuccess.AddListener(ClosePanel);
        }

        private void OnDisable()
        {
            avatarManager.OnPickedImage.RemoveListener(OnPickedImage);
            avatarManager.OnUploadUrlToPlayFabSuccess.AddListener(ClosePanel);
        }


        #endregion

        #region Show and Close Panel

        [ContextMenu("Show Panel")]
        public void ShowPanel()
        {
            gameObject.SetActive(true);
            DestroyPreviousOptions();
            SelectCurrentAvatarOption();
            scrollRect.verticalNormalizedPosition = 1f; // 初始化到頂部
        }

        private void SelectCurrentAvatarOption()
        {
            // 處理當前頭像
            Player.Avatar currentAvatar = PlayerInfoManager.Instance.PlayerInfo.avatar;
            Debug.Log($"當前 Avatar URL: {currentAvatar.Url}");

            if (savedAvatars.Count == 0)
            {
                // 創建預設頭像選項
                foreach (var avatar in defaultAvatarData.Avatars)
                {
                    CreateAvatarOption(avatar);
                }
            }
            else
            {
                // 創建先前保存的頭像選項
                foreach (var avatar in savedAvatars)
                {
                    CreateAvatarOption(avatar);
                }
            }

            // 找到當前頭像的選項並設定為正在使用
            if (defaultAvatarData.IsCustomerAvatar(currentAvatar.Url))
            {
                currentOption = avatarOptions.FirstOrDefault(option => option.avatar.Url == currentAvatar.Url);
                if (currentOption == null)
                {
                    currentOption = CreateAvatarOption(currentAvatar);
                }
            }
            else
            {
                // 如果是預設頭像，根據URL找到對應的頭像選項
                var avatarIndex = defaultAvatarData.GetAvatarIndex(currentAvatar.Url);
                currentOption = avatarOptions[avatarIndex];
            }

            if (currentOption != null)
            {
                currentOption.SetIsUsing(); // 設定為正在使用的頭像選項
            }
            else
            {
                Debug.LogError("未能找到當前頭像的選項");
            }
        }


        public void ClosePanel(Player.Avatar avatar)
        {
            // 記下當前面板上所有的AvatarOption
            savedAvatars = avatarOptions.Select(option => option.avatar).ToList();
            ClosePanel();
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }


        #endregion


        #region Pick Image

        public void PickImage()
        {
            avatarManager.PickImage();
        }

        private void OnPickedImage(Player.Avatar avatar)
        {
            var avatarOption = CreateAvatarOption(avatar);
            avatarOption.ChangeSelected();

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // 滾動到最下面
        }


        #endregion

        #region Confirm Selected Avatar

        [ContextMenu("ConfirmAvatar")]
        public void ConfirmAvatar()
        {
            avatarManager.StartCoroutine(ConfirmAvatarWithLoadingDelay());
        }

        private IEnumerator ConfirmAvatarWithLoadingDelay()
        {
            LoadingManger.Instance.Open_Loading_animator(); // 開啟 Loading 動畫

            yield return new WaitForSeconds(3f); // 延遲3秒

            bool hasNotUploadToStorage = currentOption.avatar.Url == "";
            if (hasNotUploadToStorage)
            {
                avatarManager.UploadImageToStorage(currentOption.avatar);
            }
            else
            {
                avatarManager.UploadUrlToPlayFab(currentOption.avatar);
                ClosePanel();
            }

            LoadingManger.Instance.Close_Loading_animator(); // 關閉 Loading 動畫
        }


        #endregion


        #region Create/Destory Avatar Option

        private AvatarOption CreateAvatarOption(Player.Avatar avatar)
        {
            AvatarOption avatarOption = Instantiate(avatarOptionPrefab, spawnTransform);
            avatarOption.Init(this, avatar);
            avatarOptions.Add(avatarOption);

            return avatarOption;
        }


        private void DestroyPreviousOptions()
        {
            foreach (var view in avatarOptions)
            {
                Destroy(view.gameObject);
            }

            avatarOptions = new List<AvatarOption>();
        }

        public void SetSelectedAvatar(AvatarOption option)
        {
            currentOption = option;
        }

        #endregion
    }
}