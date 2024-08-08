using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine.ReserveMachine
{
    public class LeaveMachinePanel : MonoBehaviour
    {
        [SerializeField] private RemindPanel _remindPanel;
        [SerializeField] private TextMeshProUGUI content; 
    
        public void ShowPanel(int needSpinCount)
        {

            string info = "請問您是否要離開?\n" +
                          $"(距離保留機台門檻還差 {needSpinCount} 轉)";

            content.text = info;
            _remindPanel.OpenPanel();
        }
    }
}