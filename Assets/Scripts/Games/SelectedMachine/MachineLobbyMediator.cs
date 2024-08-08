using System.Collections.Generic;
using Games.Data;
using GameSelectedMenu;
using UnityEngine;
using SlotTemplate;


namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the selected machine and related functionality.
    /// </summary>
    public class MachineLobbyMediator : MonoBehaviour
    {
        #region Instance (Singleton)
        private static MachineLobbyMediator _instance;

        /// <summary>
        /// Singleton instance of the MachineLobbyMediator.
        /// </summary>
        public static MachineLobbyMediator Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<MachineLobbyMediator>();

                    if (_instance == null)
                    {
                        Debug.LogError($"The GameObject of type {typeof(MachineLobbyMediator)} is not present in the scene, " +
                                       $"yet its method is being called. Please add {typeof(MachineLobbyMediator)} to the scene.");
                    }
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
        
        #region Variables

        [SerializeField] private GameMediaList _gameMediaList;
        
        [Header("Variable")]
        [SerializeField] private List<MachineInfo> _machineInfos;
        [SerializeField] private GameProfile _gameProfile;

        /// <summary>
        /// ID of the selected game.
        /// </summary>
        public Games.Data.GameId SelectedGameId;

        /// <summary>
        /// Gets or sets the selected hall.
        /// </summary>
        public Hall SelectedHall;
        
        /// <summary>
        /// Gets the list of machines.
        /// </summary>
        public List<MachineInfo> MachineInfos => _machineInfos;

        /// <summary>
        /// Gets the game profile.
        /// </summary>
        public GameProfile GameProfile => _gameProfile;
        
        /// <summary>
        /// Gets the information of the currently selected machine.
        /// </summary>
        public MachineInfo SelectedMachine => _machineSelectionHandler.GetSelectedMachine();

        /// <summary>
        /// Gets the ID of the currently selected machine.
        /// </summary>
        public int SelectedMachineId => _machineSelectionHandler.GetSelectedMachineId();

        #endregion

        #region Handlers

        [Header("Handlers")]
        /// <summary>
        /// Handles the process of entering a machine, including random selection and reservation.
        /// </summary>
        [SerializeField] private EnterMachineHandler _enterMachineHandler;
        
        /// <summary>
        /// Handles the retrieval and processing of machine lists for a specific game and hall.
        /// </summary>
        [SerializeField] private FetchMachineListHandler _fetchMachineListHandler;
        
        /// <summary>
        /// Handles the selection and management of a game machine.
        /// </summary>
        [SerializeField] private MachineSelectionHandler _machineSelectionHandler;
        
        /// <summary>
        /// Handles the process of changing the selected hall and related events.
        /// </summary>
        [SerializeField] private ChangeHallHandler _changeHallHandler;

        /// <summary>
        /// Handles player's machine reservation.
        /// </summary>
        [SerializeField] private MachineReservedHandler machineReservedHandler;
        
        #endregion
        
        #region Fetch Machine List

        /// <summary>
        /// Requests the machine list for the selected game and hall.
        /// </summary>
        [ContextMenu("FetchMachineList")]
        public void FetchMachineList()
        {
            _fetchMachineListHandler.FetchMachineList(SelectedGameId, SelectedHall);
        }

        /// <summary>
        /// Sets the machine list and game profile.
        /// </summary>
        /// <param name="machineList">The machine list to set.</param>
        public void SetMachineList(MachineList machineList)
        {
            _machineInfos = machineList.MachineInfos;
            _gameProfile = machineList.GameProfile;
            machineReservedHandler.SetPlayerReserve(machineList.PlayerReserve);
        }

        #endregion
        
        #region Change Hall

        /// <summary>
        /// Changes the selected hall and requests the updated machine list if VIP level is sufficient.
        /// </summary>
        /// <param name="hall">The new selected hall.</param>
        public void TryChangeHall(Hall hall)
        {
            _changeHallHandler.ChangeHall(hall);
        }
        
        #endregion
        
        #region Selected Machine

        public void SelectNextMachine()
        {
            _machineSelectionHandler.SelectNextMachine();
        }

        public void SelectPreviousMachine()
        {
            _machineSelectionHandler.SelectPreviousMachine();
        }

        /// <summary>
        /// Selects a machine for gameplay.
        /// </summary>
        /// <param name="machineInfo">The machine information to select.</param>
        /// <param name="invokeEvent">Whether to invoke the selection event.</param>
        public void SetSelectedMachine(Data.MachineInfo machineInfo, bool invokeEvent = false)
        {
            _machineSelectionHandler.SelectMachine(machineInfo, invokeEvent);
        }

        #endregion
        
        #region Enter Machine
        public void RandomEnterMachine()
        {
            _enterMachineHandler.RandomEnterMachine(MachineInfos);
            NetworkMessageSender networkMessageSender = gameObject.AddComponent<NetworkMessageSender>();
            networkMessageSender.EnterMachine();
        }

        public void TryEnterMachine(Data.MachineInfo machineInfo)
        {
            _machineSelectionHandler.SelectMachine(machineInfo);
            TryEnterSelectedMachine();
        }

        public void TryEnterSelectedMachine()
        {
            _enterMachineHandler.StartEnterMachineProcess(SelectedMachine);
            NetworkMessageSender networkMessageSender = gameObject.AddComponent<NetworkMessageSender>();
            networkMessageSender.EnterMachine();
        }

        public void CancelReserveMachineThenEnterMachine()
        {
            machineReservedHandler.CancelCurrentGameReserve();
            TryEnterSelectedMachine();
        }
        #endregion

        #region Player Reserved

        /// <summary>
        /// Gets the player reserve handler.
        /// </summary>
        public MachineReservedHandler MachineReservedHandler => machineReservedHandler;
        
        public bool IsEnabledPlayedMachine(MachineInfo machineInfo)
        {
            return machineReservedHandler.IsEnabledPlayedMachine(machineInfo);
        }

        
        #endregion
        
        #region Get Data
        /// <summary>
        /// Gets the game media associated with the selected game ID.
        /// </summary>
        /// <returns>The GameMedia object containing game information.</returns>
        public GameMedia GetGameMedia()
        {
            // Retrieve the game media based on the selected game ID
            return _gameMediaList.GetData(SelectedGameId);
        }

        public List<MachineInfo> GetFilteredMachineList(bool filterIdleMachine)
        {
            var filteredMachineList = new List<MachineInfo>();
            foreach (var machineInfo in MachineInfos)
            {
                // To filter Idle machines
                if (filterIdleMachine)
                {
                    // When the machine is Idle or being played, reserved by the current player, it will be included in the filtered list
                    if (machineReservedHandler.IsEnabledPlayedMachine(machineInfo))
                    {
                        filteredMachineList.Add(machineInfo);
                    }
                }
                else
                {
                    filteredMachineList.Add(machineInfo);
                }
            }

            return filteredMachineList;
        }
        
        #endregion
    }
}
