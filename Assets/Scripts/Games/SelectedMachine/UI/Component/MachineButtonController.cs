using System.Linq;
using Games.Data;
using UnityEngine;

namespace Games.SelectedMachine
{
    public class MachineButtonController : MonoBehaviour
    {
        [SerializeField] private ReserveMachineButton reserveMachineButton;
        [SerializeField] private GameObject randomEnterButton;

        [SerializeField] private bool hasReservedMachine;
        [SerializeField] private Data.MachineInfo reservedMachine;

        public void UpdateUI(MachineList machineList)
        {
            var matchingReserveMachine = machineList.PlayerReserve.reserveMachines
                .FirstOrDefault(reserveMachine => 
                    reserveMachine.GameId == machineList.GameProfile.GameId && reserveMachine.Hall == machineList.Hall);

            
            reservedMachine = matchingReserveMachine;
            hasReservedMachine = matchingReserveMachine != null;

            if (hasReservedMachine)
            {
                reserveMachineButton.SetReserveMachine(reservedMachine);
            }

            SetReserveMachineButtonActive(hasReservedMachine);
        }

        public void SetReserveMachineButtonActive(bool showReserveMachineButton)
        {
            reserveMachineButton.SetButtonActive(showReserveMachineButton);
            randomEnterButton.SetActive(!showReserveMachineButton);
        }
    }
}