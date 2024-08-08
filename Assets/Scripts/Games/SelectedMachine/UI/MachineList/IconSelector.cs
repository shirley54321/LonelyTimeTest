using System.Collections;
using Games.Data;
using GameSelectedMenu;
using Main.PlayerPage;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Avatar = Player.Avatar;

namespace Games.SelectedMachine
{
    /// <summary>
    /// The Machine Selector for the icon display
    /// </summary>
    public class IconSelector : SelectorBase
    {
        [SerializeField] private TextMeshProUGUI machineNumber;
        [SerializeField] private Image avatarImage;

        [SerializeField] private GameObject avatarIcon, reserveIcon;
        [SerializeField] private AvatarManager avatarManager;
        
        /// <summary>
        /// Updates the UI elements with machine information and game media.
        /// </summary>
        /// <param name="info">The machine information.</param>
        public void UpdateUI(MachineInfo info)
        {
            _machineInfo = info;
            
            machineNumber.text = $"{info.MachineNumber}";

            UpdateMachineStateUI(info);
        }

        private void UpdateMachineStateUI(Data.MachineInfo info)
        {
            reserveIcon.SetActive(false);
            avatarIcon.SetActive(false);

            switch (info.ReserveMachineInfo.State)
            {
                case MachineState.Playing:
                {
                    if (!IsUsedByCurrentPlayer())
                    {
                        avatarIcon.SetActive(true);
                        GetPlayerAvatar(info.ReserveMachineInfo.PlayFabId);
                    }

                    break;
                }
                case MachineState.Reserved:
                {
                    if (!IsUsedByCurrentPlayer())
                    {
                        avatarIcon.SetActive(true);
                        GetPlayerAvatar(info.ReserveMachineInfo.PlayFabId);

                        if (info.ReserveMachineInfo.State == MachineState.Reserved)
                        {
                            reserveIcon.SetActive(true);
                        }
                    }

                    break;
                }
            }
        }
        
        private void GetPlayerAvatar(string playFabId)
        {
            PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest()
            {
                PlayFabId =  playFabId,
                ProfileConstraints = new PlayerProfileViewConstraints()
                {
                    ShowAvatarUrl = true
                }
            }, infoResult =>
            {
                string avatarUrl = infoResult.PlayerProfile.AvatarUrl;
                Debug.Log($"avatarUrl {avatarUrl}");
                avatarManager.OnGetAvatarWithUrl.AddListener(OnGainAvatar);
                avatarManager.GetAvatar(avatarUrl);
            }, error =>
            {
                Debug.Log($"Fail to get avatar from {playFabId}, {error.ErrorMessage}");
            });
        }
        
        private void OnGainAvatar(Avatar avatar)
        {
            avatarImage.sprite = avatar.Sprite;
            avatarManager.OnGetAvatarWithUrl.RemoveListener(OnGainAvatar);
        }

        /// <summary>
        /// Open Player Info Panel for the player playing or reserve this machine
        /// </summary>
        public void OpenPlayerInfoPanel()
        {
            OtherPlayerInfoManager.Instance.OpenOtherPlayerPanel(_machineInfo.ReserveMachineInfo.PlayFabId);
        }
    }
}