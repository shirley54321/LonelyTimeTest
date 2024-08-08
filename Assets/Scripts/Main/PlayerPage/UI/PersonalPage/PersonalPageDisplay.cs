using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Player;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Main.PlayerPage.UI.PersonalPage
{
    /// <summary>
    /// Display PlayerPage Info
    /// </summary>
    public class PersonalPageDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI displayName, UID,
            dragonCoin, dragonBall,
            activity, 
            level, experience, 
            registerDate, messageBox;
        [SerializeField]
        private TextMeshProUGUI VIPLVText;
       [SerializeField] private Image avatar;
        [SerializeField] private Slider experienceBar;

        [SerializeField] private Image vipIcon;

  

        #region Show/Close Panel

        public void ShowPanel()
        {
            panel.SetActive(true);
        }

        public void ClosePanel()
        {
            panel.SetActive(false);
        }


        public void UpdateUI(PlayerInfo playerInfo)
        {
            displayName.text = playerInfo.userTitleInfo.DisplayName;
            avatar.sprite = playerInfo.avatar.Sprite;

            UID.text = $"{playerInfo.userProfile.UID}";

            UpdateDragonCoinUI(playerInfo.dragonCoin);
            UpdateDragonBallUI(playerInfo.dragonBall); 
            //UpdateActivityUI(playerInfo.activity);

            registerDate.text = $"註冊時間: {playerInfo.userTitleInfo.Created:yyyy/MM/dd}";
            messageBox.text = $"{playerInfo.GetMessageBoard().Message}";

            
            UpdateVIPUI(playerInfo.vip.level, playerInfo.vipIcon);
            UpdateActivity(playerInfo.activity);
            UpdateLevelUI(playerInfo.userLevel.Level, playerInfo.userLevel.Experience);

        }

        

        #endregion

        
        #region Update UI
        public void UpdateDragonCoinUI(int coin)
        {
            dragonCoin.text = $"{coin / 100:N2}";
        }
        
        public void UpdateDragonBallUI(int ball)
        {
            dragonBall.text = $"{ball:N}";
        }
        
        // private void UpdateActivityUI(int playerInfoActivity)
        // {
        //     activity.text = $"活躍度:{playerInfoActivity.WeeklyValue}";
        // }
        
        public void UpdateLevelUI(int playerLevel, decimal nowExperience) // TODO 修改傳入方式
        {   
            level.text = $"{playerLevel}";
            decimal nextLevelExperience = 90000 ;
            
            experience.text =
               // $"{nowExperience} / {nextLevelExperience}";
               $"{nowExperience} / {nextLevelExperience}";
            // experienceBar.fillAmount = (float) nowExperience / (float) nextLevelExperience;
            float fillAmount = ((float)nowExperience % 90000) / (float)90000;
            Debug.Log($"(float)nowExperience%90000) {(float)nowExperience%90000}, fillAmount {fillAmount}");
            experienceBar.value = fillAmount;
            Debug.Log($"UpdateLevelUI - player level: {playerLevel}, nowExperience: {nowExperience}");

        }

        public void UpdateVIPUI(int vipLevel, Sprite vipSprite)
        {
            vipIcon.sprite = vipSprite;
            VIPLVText.text = "層級:VIP"+vipLevel;
            
        }


        public void UpdateActivity(int activityNum)
        {
            activity.text = "活躍度:"+activityNum;
        }
        #endregion


        #region Get PlayerLevel Info From PlayFab
        public async Task<GetUserDataResult> GetLevelInfo()
        {
            var tcs = new TaskCompletionSource<GetUserDataResult>();
            List<string> userReadOnlyDataKeys = new List<string> { "PlayerLevel", "TotalBet" };
            GetUserDataRequest userLevelRequest = new GetUserDataRequest { Keys = userReadOnlyDataKeys };

            PlayFabClientAPI.GetUserReadOnlyData(userLevelRequest,
                    (result) => tcs.SetResult(result),
                    (error) => tcs.SetException(new Exception($"PlayFab Error: {error.ErrorMessage}"))
            );

            return await tcs.Task;
        }

        #endregion


    }
}