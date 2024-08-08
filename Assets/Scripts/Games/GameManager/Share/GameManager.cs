using Games.SelectedMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlotTemplate
{
    public class GameManager : MonoBehaviour
    {
        public event Action Inited;

        [Header("Siblings")]
        public GameDBManager gameDBManager;
        public MachineInfoManager machineInfoManager; 
        public GameStatesManager gameStatesManager;
        public NetworkMessageSender networkMessageSender;
        public NetworkDataReceiver networkDataReceiver;

        [Header("Children")]
        public PlayerStatus playerStatus;
        [SerializeField] GUIDisplayManager _guiDisplayManager;

        public IConnectionHandler connectionHandler { get; set; }
        public IMachineDetail CurrentMachineDetail => connectionHandler.CurrentMachineDetail;

        ushort _currentMachineId = 0;
        int _targetScenceBuildIndexWhenExit = -1;

        bool _isInited = false;
        public bool IsInited => _isInited;

        void OnInited()
        {
            if (_guiDisplayManager != null)
                _guiDisplayManager.ExitButtonPressed += OnExitButtonPressed;

            machineInfoManager.enabled = true;

            Inited?.Invoke();
        }

        private void OnDestroy()
        {
            if (connectionHandler != null)
                _guiDisplayManager.ExitButtonPressed -= OnExitButtonPressed;
        }

        public void InitGame(IConnectionHandler connectionHandler, string lobbyName, ushort machineId, string playerName, decimal playerBalance, int targetSceneBuildIndexWhenExit)
        {
            machineId = (ushort)MachineLobbyMediator.Instance.SelectedMachine.MachineNumber;            
            this.connectionHandler = connectionHandler;
            _currentMachineId = machineId;
            _targetScenceBuildIndexWhenExit = targetSceneBuildIndexWhenExit;

            networkMessageSender?.Init(connectionHandler);
            networkDataReceiver?.Init(connectionHandler);

            //machineInfoManager?.SetBasicInfo(lobbyName, machineId);
            gameStatesManager?.Init(machineId);

            playerStatus?.Init();

            _isInited = true;

            OnInited();
        }
        public void InitBonusGame(IConnectionHandler connectionHandler, string lobbyName, ushort machineId, string playerName, decimal playerBalance, int targetSceneBuildIndexWhenExit)
        {
            machineId = (ushort)MachineLobbyMediator.Instance.SelectedMachine.MachineNumber;
            this.connectionHandler = connectionHandler;
            _currentMachineId = machineId;
            _targetScenceBuildIndexWhenExit = targetSceneBuildIndexWhenExit;

            networkMessageSender?.Init(connectionHandler);
            networkDataReceiver?.Init(connectionHandler);

            //machineInfoManager?.SetBasicInfo(lobbyName, machineId);
            gameStatesManager?.Init(machineId);

            //playerStatus?.Init();

            _isInited = true;

            OnInited();
        }

        public void FetchMachineDetail()
        {
            //connectionHandler?.GetMachineDetail(2413);
        }

        public void ExitMachine()
        {
            //connectionHandler.ExitMachine();
        }

        void Exit()
        {
            if (_targetScenceBuildIndexWhenExit >= 0)
                SceneManager.LoadScene(_targetScenceBuildIndexWhenExit);
            else
                Debug.Log("Try to exit machine but target scene is invalid. (It is normal when you are using the \"DebugTools\".)");
        }

        void OnExitMachine()
        {
            Exit();
        }

        void OnExitButtonPressed(object sender, EventArgs args)
        {
            if (true)
            {
                if (_guiDisplayManager != null)
                {
                    bool isPanelOpened = _guiDisplayManager.OpenExitMachinePanel();

                    if (!isPanelOpened)
                    {
                        ExitMachine();
                    }
                }
            }
            else
            {
                ExitMachine();
            }
        }
    }
}
