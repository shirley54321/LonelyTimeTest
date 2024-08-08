using UnityEngine;
using UnityEngine.UI;
using UserAccount.Tool;

namespace UserAccount.UI
{
    public class UserRuleUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Text textComponent;

        [SerializeField] private Button button;

        [SerializeField] private TextAsset startText;

        [SerializeField] private Toggle agreeToggle;
        [SerializeField] private Toggle startPageToggle;

        [SerializeField] private GameObject agreementPanel;

        [SerializeField] private PlayFabWIthFB playFabWithFB;
        [SerializeField] private PlayFabWithGoogle playFabWithGoogle;
        [SerializeField] private PlayFabWithApple playFabWithApple;
        private void Awake()
        {
            button.onClick.AddListener(OnClickAgreeButton);
        }

        #region Open Close Panel
        public void OpenPanelWithCheckHaveAgree()
        {
            OpenPanel();
            UpdateAgreementPanel(!HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule());
        }
        
        /// <summary>
        /// Open the main registration panel.
        /// </summary>
        [ContextMenu("Open Panel")]
        public virtual void OpenPanel()
        {
            mainPanel.SetActive(true);
            
            agreeToggle.SetIsOnWithoutNotify(false);
            OnClickAgreeToggle(false);
            
            startPageToggle.SetIsOnWithoutNotify(true);
            OnToggleChanged(startText);

            UpdateAgreementPanel(true);
        }
        public virtual void OpenPanelForFB()
        {
            if (HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule())
            {
               playFabWithFB.LoginWithFacebook();
            }
            else{
                mainPanel.SetActive(true);
                
                agreeToggle.SetIsOnWithoutNotify(false);
                OnClickAgreeToggle(false);
                
                startPageToggle.SetIsOnWithoutNotify(true);
                OnToggleChanged(startText);

                UpdateAgreementPanel(true);
            }
        }

        public virtual void OpenPanelForGoogle()
        {
            if(HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule())
            {
                playFabWithGoogle.LoginWithGoogle();
            }
            else
            {
                mainPanel.SetActive(true);

                agreeToggle.SetIsOnWithoutNotify(false);
                OnClickAgreeToggle(false);

                startPageToggle.SetIsOnWithoutNotify(true);
                OnToggleChanged(startText);

                UpdateAgreementPanel(true);
            }
        }

        public virtual void OpenPanelForApple()
        {
            if(HaveAgreeUserRuleHandler.IsDeviceHaveAgreeRule())
            {
                playFabWithApple.LoginWithApple();
            }
            else
            {
                mainPanel.SetActive(true);

                agreeToggle.SetIsOnWithoutNotify(false);
                OnClickAgreeToggle(false);

                startPageToggle.SetIsOnWithoutNotify(true);
                OnToggleChanged(startText);

                UpdateAgreementPanel(true);
            }
        }
        private void UpdateAgreementPanel(bool setActive)
        {
            agreementPanel.SetActive(setActive);
        }

        /// <summary>
        /// Close the main registration panel.
        /// </summary>
        [ContextMenu("Close Panel")]
        public virtual void ClosePanel()
        {
            mainPanel.SetActive(false);
        }

        #endregion

        public void OnToggleChanged( TextAsset textAsset)
        {
            if (textAsset != null)
            {
                textComponent.text = textAsset.text;

                RectTransform contentRect = scrollView.content.GetComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, textComponent.preferredHeight);
            }
            if (scrollView.verticalScrollbar != null)
            {
                scrollView.verticalScrollbar.value = 1f;
            }       
        }

        public void OnClickAgreeToggle(bool agree)
        {
            button.interactable = agree;
        }

        public void OnClickAgreeButton()
        {
            HaveAgreeUserRuleHandler.SetDeviceHaveAgreeRule(true);
        }

        [ContextMenu("清除設備已經入的紀錄")]
        private void SetDeviceHaveAgreeRuleFalse()
        {
            HaveAgreeUserRuleHandler.SetDeviceHaveAgreeRule(false);
        }
    }
}