using Games.SelectedMachine.Enum;
using Shared.RemindPanel;
using UnityEngine;

namespace Games.SelectedMachine
{
    public class EnterMachineController : MonoBehaviour
    {
        [SerializeField] private CancelCurrentReservePanel cancelCurrentReservePanel;
        [SerializeField] private RemindPanel enterMachineFailPanel, serverErrorPanel;
    
        private void OnEnable()
        {
            EnterMachineHandler.OnHaveReserveInCurrentGame.AddListener(ShowPanel);
            EnterMachineHandler.TryEnterMachineFail.AddListener(OnEnterMachineFail);
        }

        private void OnDisable()
        {
            EnterMachineHandler.OnHaveReserveInCurrentGame.RemoveListener(ShowPanel);
            EnterMachineHandler.TryEnterMachineFail.RemoveListener(OnEnterMachineFail);
        }

        public void ShowPanel(Data.MachineInfo reserveMachine)
        {
            cancelCurrentReservePanel.ShowPanel(reserveMachine);
        }

        private void OnEnterMachineFail(EnterMachineFailCode failCode)
        {
            switch (failCode)
            {
                case EnterMachineFailCode.HaveBePlayingOrReserved:
                    enterMachineFailPanel.OpenPanel();
                    MachineLobbyMediator.Instance.FetchMachineList();
                    break;
                case EnterMachineFailCode.ServerError:
                    serverErrorPanel.OpenPanel();
                    break;
            }
        }

        public void CancelReserveMachineThenEnter()
        {
            MachineLobbyMediator.Instance.CancelReserveMachineThenEnterMachine();
        }

    }
}