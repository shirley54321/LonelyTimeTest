using System.Collections;
using Player;
using UnityEngine;
using UserAccount.LinkAccount;
using UserAccount.ThirdPartyLogin.LinkAccount;

namespace UserAccount.UI.LinkAccount
{
    /// <summary>
    /// Handles the UI elements and interactions for linking different types of accounts.
    /// </summary>
    public class LinkAccountUI : BasePanel
    {
        private readonly PlayerLinkStatusChecker playerLinkStatusChecker = new PlayerLinkStatusChecker();

        [SerializeField] private RegisterAccountUI registerAccountUI;

        [SerializeField] private GameObject userAccountOffButton;
        [SerializeField] private GameObject linkAccountGift;

        [SerializeField] private GameObject[] linkThirdPartyOffButtons;

        [SerializeField] private GameObject remindToLinkAccountText;

        private Coroutine smsvilidateCoroutine;
        
        /// <summary>
        /// Opens the link account panel and updates its UI.
        /// </summary>
        /// <param name="name">Name of the panel</param>
        public override void OpenPanel(string name)
        {
            UpdateUI();
            base.OpenPanel(name);
        }

        /// <summary>
        /// Subscribes to the event triggered upon successful account linking.
        /// </summary>
        private void OnEnable()
        {
            LinkAccountEvent.LinkAccountSuccessful.AddListener(UpdateUI);
        }

        /// <summary>
        /// Unsubscribes from the event upon disabling the UI.
        /// </summary>
        private void OnDisable()
        {
            LinkAccountEvent.LinkAccountSuccessful.RemoveListener(UpdateUI);
        }

        /// <summary>
        /// Updates the UI elements based on the player's account linking status.
        /// </summary>
        private void UpdateUI()
        {
            remindToLinkAccountText.SetActive(!playerLinkStatusChecker.HasLinkAnyAccount());

            userAccountOffButton.SetActive(playerLinkStatusChecker.HasLinkUserAccount());
            linkAccountGift.SetActive(!playerLinkStatusChecker.HasLinkUserAccount());

            foreach (var thirdPartyButton in linkThirdPartyOffButtons)
            {
                thirdPartyButton.SetActive(playerLinkStatusChecker.HasLinkThirdPartyAccount());
            }
        }

        #region For Buttons

        /// <summary>
        /// Closes the link account panel.
        /// </summary>
        public void ClosePanelForButton()
        {
            UIManager.Instance.ClosePanel(UIConst.LinkAccountPanel);
        }

        /// <summary>
        /// Initiates the process of linking the user account.
        /// </summary>
        public void LinkUserAccount()
        {
            if (playerLinkStatusChecker.HasLinkPhone())
            {
                OpenSmsValidationPanel();
            }
            else
            {
                registerAccountUI.OpenPanel();
            }
        }

        private void OpenSmsValidationPanel()
        {
            StopValidateCoroutine();
            smsvilidateCoroutine = StartCoroutine(SmsValidateCoroutine());
        }

        private IEnumerator SmsValidateCoroutine()
        {
            string phoneNumber = PlayerInfoManager.Instance.PhoneInfo.phoneNumber;
            string userName = PlayerInfoManager.Instance.PlayFabId;
            OpenVerificationProvisional.Instance.TakePhoneAndName(phoneNumber, userName);
            OpenVerificationProvisional.Instance.SendSMS();
            OpenVerificationProvisional.Instance.OpenPanel();
            
            var sMSHandler = OpenVerificationProvisional.Instance.SMSHandler;
            
            yield return new WaitUntil(()=>sMSHandler.IsValidateSuccessful());
            
            registerAccountUI.OpenPanel();
            OpenVerificationProvisional.Instance.ClosePanel();
        }

        public void StopValidateCoroutine()
        {
            if(smsvilidateCoroutine != null)
                StopCoroutine(smsvilidateCoroutine);    
        }



    /// <summary>
        /// Initiates the process of linking the Facebook account.
        /// </summary>
        public void LinkFacebookAccount()
        {
            var facebookLinkAccountHandler = new FacebookLinkAccountHandler();
            facebookLinkAccountHandler.StartLinkAccount();
        }

        /// <summary>
        /// Initiates the process of linking the Apple account.
        /// </summary>
        public void LinkAppleAccount()
        {
            var appleLinkAccountHandler = new AppleLinkAccountHandler();
            appleLinkAccountHandler.StartLinkAccount();
        }

        /// <summary>
        /// Initiates the process of linking the Google account.
        /// </summary>
        public void LinkGoogleAccount()
        {
            var googleLinkAccountHandler = new GoogleLinkAccountHandler();
            googleLinkAccountHandler.StartLinkAccount();
        }

        #endregion
    }
}
