using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby.TopBar
{
    public class TopBarHandler : MonoBehaviour
    {
        [SerializeField] private Image avatar, vipIcon;
        [SerializeField] private TextMeshProUGUI playerName;

        private void OnEnable()
        {
            PlayerInfoManager.OnPlayerInfoChange.AddListener(UpdatePlayerInfo);
            LobbyEventHandler.RefreshVIP += OnRefreshVIP;
        }

        private void OnDisable()
        {
            PlayerInfoManager.OnPlayerInfoChange.RemoveListener(UpdatePlayerInfo);
            LobbyEventHandler.RefreshVIP -= OnRefreshVIP;
        }

        public void OpenPlayerPagePanel()
        {
            UIManager.Instance.OpenPanel(UIConst.LocalPlayerPage);
        }
        private void OnRefreshVIP()
        {
            UpdatePlayerInfo(PlayerInfoManager.Instance.PlayerInfo);
        }


        private void UpdatePlayerInfo(PlayerInfo playerInfo)
        {
            avatar.sprite = playerInfo.avatar.Sprite;
            vipIcon.sprite = playerInfo.vipIcon;
            playerName.text = playerInfo.userTitleInfo.DisplayName;
        }
    }
}
