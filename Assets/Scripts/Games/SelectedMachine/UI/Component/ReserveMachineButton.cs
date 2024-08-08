using System;
using Games.Data;
using GameSelectedMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class ReserveMachineButton : MonoBehaviour
    {
        [SerializeField] private Text machineText, reserveText;

         private bool _hasReserveMachine;

        [SerializeField] private GameObject button;
        [SerializeField] private Data.MachineInfo _reserveMachine;

        [SerializeField] private MachineButtonController machineButtonController;

        private void Update()
        {
            UpdateUI();
        }

        /// <summary>
        /// Sets whether the player has reserved any machines.
        /// </summary>
        public void SetReserveMachine(MachineInfo reserveMachine)
        {
            _reserveMachine = reserveMachine;
            _hasReserveMachine = true;
        }

        /// <summary>
        /// Updates the UI display of the reserved machine.
        /// </summary>
        private void UpdateUI()
        {
            if (!_hasReserveMachine)
            {
                return;
            }
            
            var info = _reserveMachine.ReserveMachineInfo;
            TimeSpan duration = info.EndReserveTime - DateTime.Now;
            if (duration.TotalSeconds <= 0)
            {
                machineButtonController.SetReserveMachineButtonActive(false);
                _hasReserveMachine = false;
            }
            else
            {
                switch(_reserveMachine.Hall)
                {
                    case Hall.Beginner:
                        machineText.text = "體驗廳";
                        break;
                    case Hall.General:
                        machineText.text = "一般廳";
                        break;
                    case Hall.Master:
                        machineText.text = "高手廳";
                        break;
                    case Hall.Honor:
                        machineText.text = "尊榮廳";
                        break;
                }
                machineText.text += $" {_reserveMachine.MachineNumber} 號";
                reserveText.text = $"{((int)duration.TotalHours):D2}：{duration.Minutes:D2}：{duration.Seconds:D2}";
            }
        }

        public void SetButtonActive(bool active)
        {
            button.gameObject.SetActive(active);
        }

        /// <summary>
        /// Attempts to enter the reserved machine.
        /// </summary>
        public void EnterReserveMachine()
        {
            MachineLobbyMediator.Instance.TryEnterMachine(_reserveMachine);
        }
    }
}