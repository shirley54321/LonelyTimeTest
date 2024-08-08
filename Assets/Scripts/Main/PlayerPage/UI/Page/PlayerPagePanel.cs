using Main.PlayerPage.UI.Controller;
using Main.PlayerPage.UI.HonorWall;
using Main.PlayerPage.UI.PersonalPage;
using UnityEngine;

namespace Main.PlayerPage.UI.Page
{
    public class PlayerPagePanel : BasePanel
    {
        [SerializeField] protected PersonalPageDisplay personalPageDisplay;
        [SerializeField] protected HonorWallDisplay honorWallDisplay;

        [SerializeField] private PlayerPageControllerBase playerPageController;


        public override void OpenPanel(string name)
        {
            base.OpenPanel(name);
            ShowPersonalPage();
        }

        public void ClosePanelForButton()
        {
            playerPageController.ClosePanel();
        }

        public void OnPlayerPageToggleChange(bool isOn)
        {
            if (isOn)
            {
                ShowPersonalPage();
            }
        }
        
        public void OnHonorPageToggleChange(bool isOn)
        {
            if (isOn)
            {
                ShowHonorWallPage();
            }
        }


        private void ShowPersonalPage()
        {
            personalPageDisplay.ShowPanel();
            honorWallDisplay.ClosePanel();

            playerPageController.UpdatePersonalPageUI();
        }


        private void ShowHonorWallPage()
        {

        
            InstanceRemindPanel.Instance.OpenPanel("·q½Ð´Á«Ý");
            return;
            honorWallDisplay.ShowPanel();
            personalPageDisplay.ClosePanel();
            
            playerPageController.UpdateHonorHallUI();
        }
        
        
    }
}