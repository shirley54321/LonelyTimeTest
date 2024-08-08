using Main.PlayerPage.Model;
using Player;
using UnityEngine;
using UnityEngine.UI;
using UserAccount.LinkAccount;

namespace Main.PlayerPage.UI.Controller
{
    /// <summary>
    /// Controller for the local player's page, managing UI updates and panel interactions.
    /// </summary>
    public class LocalPlayerPageController : PlayerPageControllerBase
    {
        [SerializeField] private GameObject firstTopUpButton, bindAccountButton, bindPhoneButton;

        [SerializeField] private GameObject changeNickNameButton;
        private readonly PlayerLinkStatusChecker playerLinkStatusChecker = new PlayerLinkStatusChecker();

        [SerializeField] private GameObject warningLinkText;
        #region Event

        void OnEnable()
        {
            PlayerInfoManager.OnPlayerInfoChange.AddListener(OnPlayerInfoChange);

            PlayerInfoManager.OnVipChanged.AddListener(personalPageDisplay.UpdateVIPUI);
            PlayerInfoManager.OnUserLevelChanged.AddListener(personalPageDisplay.UpdateLevelUI);
            InventoryManager.OnDragonCoinChange.AddListener(personalPageDisplay.UpdateDragonCoinUI);
            InventoryManager.OnDragonBallChange.AddListener(personalPageDisplay.UpdateDragonBallUI);
            
            LinkAccountEvent.LinkAccountSuccessful.AddListener(UpdatePersonalPageUI);
        }

        void OnDisable()
        {
            PlayerInfoManager.OnPlayerInfoChange.RemoveListener(OnPlayerInfoChange);

            PlayerInfoManager.OnVipChanged.RemoveListener(personalPageDisplay.UpdateVIPUI);
            PlayerInfoManager.OnUserLevelChanged.RemoveListener(personalPageDisplay.UpdateLevelUI);
            InventoryManager.OnDragonCoinChange.RemoveListener(personalPageDisplay.UpdateDragonCoinUI);
            InventoryManager.OnDragonBallChange.RemoveListener(personalPageDisplay.UpdateDragonBallUI);
            
            LinkAccountEvent.LinkAccountSuccessful.RemoveListener(UpdatePersonalPageUI);
        }

        #endregion

        private void OnPlayerInfoChange(PlayerInfo playerInfo)
        {
            UpdatePersonalPageUI();
        }

        /// <summary>
        /// Updates the UI elements on the personal page.
        /// </summary>
        public override void UpdatePersonalPageUI()
        {
            personalPageDisplay.UpdateUI(PlayerInfoManager.Instance.PlayerInfo);

            UpdateChangeNickNameButton();
            bool haveLinkBothAccount = playerLinkStatusChecker.HasLinkUserAccount() &&
                                       playerLinkStatusChecker.HasLinkThirdPartyAccount();
            
            ChangeButtonState(bindAccountButton, !haveLinkBothAccount);
            ChangeButtonState(bindPhoneButton, !playerLinkStatusChecker.HasLinkPhone());
            ChangeButtonState(firstTopUpButton, !playerLinkStatusChecker.HasTopUp());

            warningLinkText.SetActive(!playerLinkStatusChecker.HasLinkAnyAccount());
        }

        /// <summary>
        /// 根據狀態讓按鈕顯示或關閉
        /// </summary>
        /// <param name="button"></param>
        /// <param name="state"></param>
        void ChangeButtonState(GameObject button, bool state)
        {
            Button btn = button.GetComponent<Button>();//取得按鈕組件
            Image img = button.GetComponent<Image>();//取得圖片組件

            btn.interactable = state;//按鈕是否可交互

            //調整按鈕的透明度
            Color newColor = img.color;
            newColor.a = state ? 1f : 0f;
            img.color = newColor;

            //設定子物件的顯示狀態
            for (int i = 0; i < button.transform.childCount; i++)
            {
                button.transform.GetChild(i).gameObject.SetActive(state);
            }

        }

        private void UpdateChangeNickNameButton()
        {
            var haveChangedNickName = PlayerInfoManager.Instance.PlayerInfo.changeNameRecord.haveChangedNickName;
      
            changeNickNameButton.SetActive(!haveChangedNickName);
        }

        /// <summary>
        /// Updates the UI elements on the honor hall page.
        /// </summary>
        public override void UpdateHonorHallUI()
        {
            honorWallDisplay.UpdateUI();
        }

        /// <summary>
        /// Closes the panel associated with the local player page.
        /// </summary>
        public override void ClosePanel()
        {
            UIManager.Instance.ClosePanel(UIConst.LocalPlayerPage);
        }

        #region Button Function

        /// <summary>
        /// Displays the first top-up panel or reminds the user to link their phone first.
        /// </summary>
        public void FirstTopUpPanel()
        {
            if (playerLinkStatusChecker.HasLinkPhone())
            {
                // TODO: Open Shop Panel
                Debug.Log("Show Top Up Panel");
            }
            else
            {
                // Remind: Please Link Phone First
            }
        }

        /// <summary>
        /// Displays the link account panel.
        /// </summary>
        public void LinkAccountPanel()
        {
            Debug.Log("Show Link Account Panel");
            UIManager.Instance.OpenPanel(UIConst.LinkAccountPanel);
        }

        /// <summary>
        /// Displays the link phone panel.
        /// </summary>
        public void LinkPhonePanel()
        {
            Debug.Log("Show Link Phone Panel");
        }

        /// <summary>
        /// Opens the VIP right panel.
        /// </summary>
        public void OpenVipRightPanel()
        {
            Debug.Log("Open Vip Right Panel");
            UIManager.Instance.OpenPanel(UIConst.VIPPanel);
        }

        #endregion
    }
}
