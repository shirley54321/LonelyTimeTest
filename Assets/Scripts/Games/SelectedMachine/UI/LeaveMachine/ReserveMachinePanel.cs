using Games.Tool;
using Shared.RemindPanel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine.ReserveMachine
{
    [RequireComponent(typeof(RemindPanel))]
    public class ReserveMachinePanel : MonoBehaviour
    {
        [SerializeField] private RemindPanel _remindPanel;
        [SerializeField] private TextMeshProUGUI content; 

        public void ShowPanel(int stillCanReserveCount)
        {

            string info = $"已達保留機台門檻請問\n" +
                          "您是否要保留此機台?\n" +
                          $"您目前剩餘可保留機台數  {stillCanReserveCount} 台";

            content.text = info;
            _remindPanel.OpenPanel();
        }
    }
}