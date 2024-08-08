using Games.Tool;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    [RequireComponent(typeof(RemindPanel))]
    public class CancelCurrentReservePanel : MonoBehaviour
    {
        private RemindPanel _remindPanel;
        [SerializeField] private TextMeshProUGUI content; 
        [SerializeField] private GameInfoProvider provider;
        
        private void Awake()
        {
            _remindPanel = GetComponent<RemindPanel>();
        }
        
        public void ShowPanel(Data.MachineInfo machineInfo)
        {
            string info = $"您的{provider.GetGameName(machineInfo.GameId)}有保留機台\n" +
                          $"{provider.GetHallName(machineInfo.Hall)} {machineInfo.MachineNumber}號\n" +
                          "是否放棄並進入?";

            content.text = info;
            _remindPanel.OpenPanel();
        }
    }
}