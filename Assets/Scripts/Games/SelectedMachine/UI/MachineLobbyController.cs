using System;
using System.Collections.Generic;
using Games.Data;
using Shared.RemindPanel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the user interface and interactions for the selected machine lobby panel.
    /// </summary>
    public class MachineLobbyController : MonoBehaviour
    {
        #region UI

        [Header("UI feature")]
        [SerializeField] private GameProfilePanel gameProfilePanel;

        [SerializeField] private IconMachineListPanel iconMachineListPanel;
        [SerializeField] private BarMachineListPanel barMachineListPanel;

        [SerializeField] private MachineButtonController machineButtonController;
        
        [SerializeField] private HallToggleController hallToggleController;

        #endregion

        #region Variable


        [Header("Variable")]
        [SerializeField] private ListDisplay listDisplay;
        [SerializeField] private bool filterIdleMachine;
        
        
        private enum ListDisplay
        {
            Icon,
            Bar
        }

        #endregion

        // Subscribes to events when the panel is enabled.
        private void OnEnable()
        {
            FetchMachineListHandler.OnRecievedMachineList.AddListener(UpdateUI);
            
        }

        // Unsubscribes from events when the panel is disabled.
        private void OnDisable()
        {
            FetchMachineListHandler.OnRecievedMachineList.RemoveListener(UpdateUI);
        }

        // Updates the UI with machine list information.
        private void UpdateUI(MachineList machineList)
        {
            gameProfilePanel.UpdateUI(machineList.GameProfile, machineList.HistoryRecord);
            UpdateMachineList();
            
            machineButtonController.UpdateUI(machineList);
            
        }

        #region Button Method
        

        public void RandomEnterMachine()
        {
            Debug.Log("Random Enter Machine");
            MachineLobbyMediator.Instance.RandomEnterMachine();
        }

        public void ShowIdleMachines(bool isOn)
        {
            if (isOn)
            {
                filterIdleMachine = true;
                UpdateMachineList();
                Debug.Log("ShowIdleMachines");
            }
            
        }

        public void ShowAllMachines(bool isOn)
        {
            if (isOn)
            {
                filterIdleMachine = false;
                UpdateMachineList();
                Debug.Log("ShowAllMachines");
            }
        }

        #endregion

        #region Machine List Display

        // Sets the machine list display to bar view.
        public void SetBarListDisplay()
        {
            listDisplay = ListDisplay.Bar;
            UpdateMachineList();
        }

        // Sets the machine list display to icon view.
        public void SetIconListDisplay()
        {
            listDisplay = ListDisplay.Icon;
            UpdateMachineList();
        }


        // Updates the machine list UI based on the chosen display mode.
        private void UpdateMachineList()
        {
            var filterMachineList = MachineLobbyMediator.Instance.GetFilteredMachineList(filterIdleMachine);
            if (listDisplay == ListDisplay.Icon)
            {
                barMachineListPanel.ClosePanel();

                iconMachineListPanel.OpenPanel();
                iconMachineListPanel.UpdateUI(filterMachineList);
            }
            else
            {
                iconMachineListPanel.ClosePanel();

                barMachineListPanel.OpenPanel();
                barMachineListPanel.UpdateUI(filterMachineList);
            }
        }

        
      

        #endregion


    }
}
