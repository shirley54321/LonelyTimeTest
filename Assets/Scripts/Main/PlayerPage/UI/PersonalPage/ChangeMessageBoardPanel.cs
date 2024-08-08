using Main.PlayerPage.Model;
using Player;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.PersonalPage
{
    /// <summary>
    /// Control Change Message Board
    /// </summary>
    public class ChangeMessageBoardPanel : MonoBehaviour
    {
        [SerializeField] private RemindPanel remindPanel;
        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private ChangeMessageBoardHandler changeMessageBoardHandler;

        #region Subscribe Event

        private void OnEnable()
        {
            changeMessageBoardHandler.OnValidateMessageBoardFailed.AddListener(ShowWarningText);
            changeMessageBoardHandler.OnChangeMessageBoardSuccess.AddListener(ClosePanel);
        }

        private void OnDisable()
        {
            changeMessageBoardHandler.OnValidateMessageBoardFailed.RemoveListener(ShowWarningText);
            changeMessageBoardHandler.OnChangeMessageBoardSuccess.RemoveListener(ClosePanel);
        }
        
        private void ShowWarningText()
        {
            InstanceRemindPanel.Instance.OpenPanel("無法更改，包含不雅或無效文字");
        }

        private void ClosePanel()
        {
            remindPanel.ClosePanel();
        }

        #endregion

        #region Method For Button

        public void OpenPanel()
        {
            inputField.text = PlayerInfoManager.Instance.PlayerInfo.GetMessageBoard().Message;
            remindPanel.OpenPanel();
        }

        public void ConfirmChange()
        {
            changeMessageBoardHandler.ChangeMessageBoard(inputField.text);
        }
        

        #endregion
    }
}