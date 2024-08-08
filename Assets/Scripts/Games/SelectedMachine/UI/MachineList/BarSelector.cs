using Games.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    /// <summary>
    /// The Machine Selector for the bar display
    /// </summary>
    public class BarSelector : SelectorBase
    {
        [SerializeField] private Text machineNumber, unspinCount;
        [SerializeField] private Text dragonBonus, superBonus, megaBonus, bigBonus;

        [SerializeField] private GameObject enterButton, isPlayingButton, reservedButton;

        public override void TryEnterMachine()
        {
            MachineLobbyMediator.Instance.SetSelectedMachine(_machineInfo);
            MachineLobbyMediator.Instance.TryEnterSelectedMachine();
        }


        /// <summary>
        /// Updates the UI with machine information.
        /// </summary>
        /// <param name="info">Machine information.</param>
        public void UpdateUI(Data.MachineInfo info)
        {
            _machineInfo = info;
            
            machineNumber.text = $"{info.MachineNumber}";
            unspinCount.text = $"{info.SpinsRecord.WithoutBonus}";

            dragonBonus.text = $"{info.ThisMonthRecord.DragonCount}";
            superBonus.text = $"{info.ThisMonthRecord.SuperCount}";
            megaBonus.text = $"{info.ThisMonthRecord.MegaCount}";
            bigBonus.text = $"{info.ThisMonthRecord.BigCount}";

            UpdateButtonUI(info.ReserveMachineInfo.State);
        }

        private void UpdateButtonUI(MachineState state)
        {
            enterButton.SetActive(false);
            reservedButton.SetActive(false);
            isPlayingButton.SetActive(false);
            
            if (IsUsedByCurrentPlayer())
            {
                enterButton.SetActive(true);
                return ;
            }
            
            switch (state)
            {
                case MachineState.Idle:
                    enterButton.SetActive(true);
                    return;
                case MachineState.Playing:
                    isPlayingButton.SetActive(true);
                    return;
                case MachineState.Reserved:
                    reservedButton.SetActive(true);
                    return ;
            }

        }
        
    }
}
