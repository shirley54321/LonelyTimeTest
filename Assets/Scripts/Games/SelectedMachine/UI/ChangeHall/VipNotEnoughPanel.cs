using Games.Data;
using Shared.RemindPanel;
using UnityEngine;

namespace Games.SelectedMachine
{
    public class VipNotEnoughPanel : MonoBehaviour
    {
        [SerializeField] private RemindPanel remindPanel;
        
        private void OnEnable()
        {
            ChangeHallHandler.OnVipNotEnough.AddListener(ShowPanel);
        }

        private void OnDisable()
        {
            ChangeHallHandler.OnVipNotEnough.RemoveListener(ShowPanel);
        }

        private void ShowPanel(Hall hall)
        {
            remindPanel.OpenPanel();
        }

        public void EnterVipUpgradePanel()
        {
            // TODO 
        }
    }
}