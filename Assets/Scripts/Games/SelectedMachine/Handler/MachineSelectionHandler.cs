using Games.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles the selection and management of a game machine.
    /// </summary>
    public class MachineSelectionHandler : MonoBehaviour
    {
        /// <summary>
        /// Event triggered when a machine is selected.
        /// </summary>
        public static readonly UnityEvent<Data.MachineInfo> OnMachineSelected = new UnityEvent<Data.MachineInfo>();

        /// <summary>
        /// Data for the selected machine.
        /// </summary>
        [SerializeField] private MachineInfo selectedMachine;

        /// <summary>
        /// Index for the selected machine.
        /// </summary>
        [SerializeField] private int selectedMachineIndex;

        /// <summary>
        /// Selects a machine for gameplay.
        /// </summary>
        /// <param name="machineInfo">The machine information to select.</param>
        /// <param name="invokeEvent">Whether to invoke the selection event.</param>
        public void SelectMachine(Data.MachineInfo machineInfo, bool invokeEvent = false)
        {
            selectedMachine = machineInfo;
            selectedMachineIndex = selectedMachine.MachineNumber - 1;
            // ConnectionScript.MachineInfo = selectedMachine;
            // Invoke the event to notify listeners that a machine is selected
            if (invokeEvent)
                OnMachineSelected.Invoke(machineInfo);

            // InfoPageManager.GetSelectedMachine(machineInfo, selectedGameId);
        }

        /// <summary>
        /// Selects the machine before the currently selected machine. 
        /// If the current index is higher than the list index, it wraps around to the first entry.
        /// If the current index is lower than 0, it selects the last entry.
        /// </summary>
        public void SelectPreviousMachine()
        {
            int totalMachines = MachineLobbyMediator.Instance.MachineInfos.Count;
            selectedMachineIndex = (selectedMachineIndex - 1 + totalMachines) % totalMachines;
            var nextMachine = MachineLobbyMediator.Instance.MachineInfos[selectedMachineIndex];
            SelectMachine(nextMachine, true);
        }

        /// <summary>
        /// Selects the machine after the currently selected machine. 
        /// If the current index is higher than the list index, it wraps around to the first entry.
        /// If the current index is lower than 0, it selects the last entry.
        /// </summary>
        public void SelectNextMachine()
        {
            int totalMachines = MachineLobbyMediator.Instance.MachineInfos.Count;
            selectedMachineIndex = (selectedMachineIndex + 1) % totalMachines;
            var nextMachine = MachineLobbyMediator.Instance.MachineInfos[selectedMachineIndex];
            SelectMachine(nextMachine, true);
        }

        /// <summary>
        /// Retrieves the selected machine.
        /// </summary>
        /// <returns>The selected machine.</returns>
        public Data.MachineInfo GetSelectedMachine()
        {
            return selectedMachine;
        }

        

        /// <summary>
        /// Gets the ID of the selected machine.
        /// </summary>
        /// <returns>The ID of the selected machine.</returns>
        public int GetSelectedMachineId()
        {
            return selectedMachine.MachineId;
        }
    }
}
