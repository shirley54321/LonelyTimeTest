using System;
using Games.Data;
using Games.Tool;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine.ReserveMachine
{
    [RequireComponent(typeof(RemindPanel))]
    public class CancelEarliestReserveThenReservePanel : MonoBehaviour
    {
        [SerializeField] private RemindPanel _remindPanel;
        [SerializeField] private TextMeshProUGUI content;
        [SerializeField] private GameInfoProvider infoProvider;

        
        public void ShowPanel(PlayerReserve playerReserve, int haveReserveCount, int stillCanReserveCount)
        {
            string machineText = "";
            foreach (var machine in playerReserve.reserveMachines)
            {
                machineText += $"{infoProvider.GetGameName(machine.GameId)} " +
                               $"{infoProvider.GetHallName(machine.Hall)} {machine.MachineNumber} 號機台\n";
            }

            string info = $"請問您是否要保留此機台\n" +
                          "請注意在您選擇保留機台後\n" +
                          "將自動放棄您所保留剩餘時間最少的機台\n" +
                          $"您目前已保留機台 {haveReserveCount} 台：\n" +
                          $"{machineText}" +
                          $"您目前剩餘可保留機台數 {stillCanReserveCount} 台\n";

            content.text = info;
            _remindPanel.OpenPanel();
        }
    }
}