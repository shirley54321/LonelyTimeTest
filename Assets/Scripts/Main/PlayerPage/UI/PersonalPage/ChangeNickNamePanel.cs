using Main.PlayerPage.Model;
using Player;
using Player.AzureFunctionResult;
using Share.Tool;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.PersonalPage
{
    public class ChangeNickNamePanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nickNameInput;

        [SerializeField] private ChangeNickNameHandler changeNickNameHandler;

        #region Event

        private void OnEnable()
        {
            changeNickNameHandler.OnValidateNewNickName.AddListener(UpdateWarningText);
            changeNickNameHandler.OnChangeNickNameSuccess.AddListener(ClosePanel);
            changeNickNameHandler.OnChangeNickNameFailed.AddListener(UpdateWarningText);
        }

        private void OnDisable()
        {
            changeNickNameHandler.OnValidateNewNickName.RemoveListener(UpdateWarningText);
            changeNickNameHandler.OnChangeNickNameSuccess.RemoveListener(ClosePanel);
            changeNickNameHandler.OnChangeNickNameFailed.RemoveListener(UpdateWarningText);
        
        }
    

        #endregion

        #region Change Nick Name

        public void ChangeNickName()
        {
            var newName = nickNameInput.text;
            changeNickNameHandler.ChangeNickName(newName);
        }

        #endregion

        #region Show Result

        private void UpdateWarningText(ChangeNickNameResultType type)
        {
            switch (type)
            {
                case ChangeNickNameResultType.OtherError:
                    InstanceRemindPanel.Instance.OpenPanel("暱稱修改失敗");
                    break;
                case ChangeNickNameResultType.NameHaveUsed:
                    InstanceRemindPanel.Instance.OpenPanel("此暱稱與既有玩家重複，請重新輸入其他暱稱");
                    break;
            }
        }
    
        private void UpdateWarningText(ValidateResult result)
        {
            if (result != ValidateResult.Pass)
            {
                InstanceRemindPanel.Instance.OpenPanel(GetValidateWaringText(result));
            }
        }

        private string GetValidateWaringText(ValidateResult result)
        {
       
            switch (result)
            {
                case ValidateResult.TooLong:
                    return "暱稱字數過長";
                case ValidateResult.TooShort:
                    return "暱稱字數過短";
                case ValidateResult.ContainSensitiveWord:
                    return "無法更改，包含不雅或無效文字";
                case ValidateResult.ContainSpecialCharacters:
                    return "無法更改，包含不雅或無效文字";
            }

            return "";
        }
    

        #endregion
        
        #region Open And Close Panel

        public void ShowPanel()
        {
            gameObject.SetActive(true);
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
