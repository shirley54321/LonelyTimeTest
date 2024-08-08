using Games.Data;
using Games.SelectedMachine;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace SlotTemplate
{

    public class MachineSelectingTool : MonoBehaviour
    {
        [SerializeField] int _initBalance = 0;

        [Header("REFS")]
        [SerializeField] ConnectionScript _connectionHandler;
        [SerializeField] GameManager _gameManager;

        [Header("Children")]
        [SerializeField] InputField _machineIdInputField;
        [SerializeField] Text _outputMessageTextContainer;


        bool _iswWaitingForReponse = false;
        public static ushort MachineId;

        void OnEnable()
        {
            _outputMessageTextContainer.text = "";

            _connectionHandler.MachineEntered += OnMachineEntered;
            _connectionHandler.MachineEnteringFailed += OnMachineEnteringFailed;

            if (_gameManager != null)
            {
                _gameManager.Inited += OnGameInited;

                if (_gameManager.IsInited)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Game Manager Null");
            }
        }

        void OnDisable()
        {
            _connectionHandler.MachineEntered -= OnMachineEntered;
            _connectionHandler.MachineEnteringFailed -= OnMachineEnteringFailed;

            if (_gameManager != null)
            {
                _gameManager.Inited -= OnGameInited;
            }
        }

        private void Start()
        {
            SubmitMahcineId();
        }
        public void SubmitMahcineId()
        {
            /*ushort machineId = MachineId;
            _connectionHandler.EnterMachine(machineId);
            _iswWaitingForReponse = true;
            if (ushort.TryParse("2", out machineId))
            {
                _connectionHandler.EnterMachine(machineId);

                _iswWaitingForReponse = true;
                _outputMessageTextContainer.text = "Waiting for server...";
            }
            else
            {
                _outputMessageTextContainer.text = "<color=red>Invalid input!</color>";
            }*/
        }

        void OnMachineEntered() //We can call function at this part 
        {
            _gameManager.InitGame(_connectionHandler, "MAJAJA", 0, "<PlayerName>", (decimal) _initBalance, 1);
        }

        void OnMachineEnteringFailed()
        {
            _outputMessageTextContainer.text = "<color=red>Entering machine failed. Please select another machine.</color>";
        }

        void OnGameInited()
        {
            gameObject.SetActive(false);
        }

    }
}
