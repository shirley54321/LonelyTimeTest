using System;
using Games.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    public class MachineInfoPanel : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private WinRecordPanel winRecordPanel;
        [SerializeField] private SpinsRecordPanel spinsRecordPanel;
        
        [SerializeField] private Text machineNumber, gameName;

        [SerializeField] private Button enterMachineButton;
        [SerializeField] private Text enterMachineText;
        
        private void Start()
        {
            MachineSelectionHandler.OnMachineSelected.AddListener(OpenPanel);
        }


        public void OpenPanel(MachineInfo machineInfo)
        {
            mainPanel.SetActive(true);
            machineNumber.text = $"{machineInfo.MachineNumber} ";
            gameName.text = $"{MachineLobbyMediator.Instance.GameProfile.GameName}";

            var selectedMachine = MachineLobbyMediator.Instance.SelectedMachine;
            winRecordPanel.UpdateUI(selectedMachine.UpperMonthRecord, selectedMachine.ThisMonthRecord);
            spinsRecordPanel.UpdateUI(selectedMachine.SpinsRecord);
      
            UpdateButtonUI(machineInfo);
        }

        private void UpdateButtonUI(MachineInfo machineInfo)
        {
            if (MachineLobbyMediator.Instance.IsEnabledPlayedMachine(machineInfo) )
            {
                enterMachineButton.interactable = true;
                enterMachineText.text = "進入遊戲";
            }
            else
            {
                enterMachineButton.interactable = false;
                enterMachineText.text = "人數已滿";
            }
        }

        public void ClosePanel()
        {
            mainPanel.SetActive(false);
        }

        #region For Button

        public void EnterMachine()
        {
            MachineLobbyMediator.Instance.TryEnterSelectedMachine();
        }

        public void SelectNextMachine()
        {
            MachineLobbyMediator.Instance.SelectNextMachine();
        }

        public void SelectedPreviousMachine()
        {
            MachineLobbyMediator.Instance.SelectPreviousMachine();
        }

        #endregion
        
    }
}