using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.Controller
{
    /// <summary>
    /// Controller for managing UI and interactions on the other player's page.
    /// </summary>
    public class OtherPlayerPageController : PlayerPageControllerBase
    {
        [SerializeField] private Text friendText, blockText;

        private void Awake()
        {
            LobbyEventHandler.CallOpenOtherPlayerInfoPage(transform);
        }



        #region Update UI

        /// <summary>
        /// Updates the UI elements on the other player's personal page.
        /// </summary>
        public override void UpdatePersonalPageUI()
        {
         //   personalPageDisplay.UpdateUI(OtherPlayerInfoManager.Instance.OtherPlayerInfo);

         //   UpdateFriendButtonUI();
         //   UpdateBlockButtonUI();
        }

        private void UpdateFriendButtonUI()
        {
            // TODO: Check Friend Status
            friendText.text = "加好友";
        }

        private void UpdateBlockButtonUI()
        {
            // TODO: Check Friend Status
            blockText.text = "封鎖";
        }

        #endregion

        /// <summary>
        /// Updates the UI elements on the honor hall page.
        /// </summary>
        public override void UpdateHonorHallUI()
        {
            honorWallDisplay.UpdateUI();
        }

        /// <summary>
        /// Placeholder for the ClosePanel method. Not implemented for OtherPlayerPageController.
        /// </summary>
        public override void ClosePanel()
        {
            UIManager.Instance.ClosePanel(UIConst.OtherPlayerPage);
        }

        #region For Button

        /// <summary>
        /// Handles the click event for the friend button.
        /// </summary>
        public void ClickFriendButton()
        {
            // TODO: Friend System
            // FriendManager.Instance.AddRequest(_playFabId);
        }

        /// <summary>
        /// Handles the click event for the block button.
        /// </summary>
        public void ClickBlockButton()
        {
            // TODO: Friend System
            // FriendManager.Instance.BlockRequest(_playFabId);
        }

        /// <summary>
        /// Handles the click event for the chat button.
        /// </summary>
        public void ClickChatButton()
        {
            // TODO: Chat System
        }

        /// <summary>
        /// Handles the click event for the give gift button.
        /// </summary>
        public void ClickGiveGiftButton()
        {
            // TODO: Friend System
            // GiftRecordManager.Instance.OpenGiftPanel();
        }

        /// <summary>
        /// Handles the click event for the report button.
        /// </summary>
        public void ClickReportButton()
        {
            // TODO: Report
        }

        #endregion


    }
}
