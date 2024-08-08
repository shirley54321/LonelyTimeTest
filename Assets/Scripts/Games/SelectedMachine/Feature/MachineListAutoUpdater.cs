using System;
using UnityEngine;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Class responsible for automatically updating the machine list at specified intervals.
    /// </summary>
    public class MachineListAutoUpdater : MonoBehaviour
    {
        [SerializeField] private float updateIntervalSeconds = 30; // Time interval for updating the machine list.
        [SerializeField] private float countdownTimer; // Timer for counting down to the next update.

        private void OnEnable()
        {
            // Subscribe to the event triggered when the machine list is fetched.
            FetchMachineListHandler.OnFetchMachineList.AddListener(OnMachineListFetched);
        }

        private void OnDisable()
        {
            // Unsubscribe from the event when the script is disabled or destroyed.
            FetchMachineListHandler.OnFetchMachineList.RemoveListener(OnMachineListFetched);
        }

        private void Start()
        {
            countdownTimer = updateIntervalSeconds; // Initialize the countdown timer.
        }

        private void Update()
        {
            countdownTimer -= Time.deltaTime; // Update the countdown timer.

            // Check if it's time to update the machine list.
            if (countdownTimer < 0)
            {
                countdownTimer = updateIntervalSeconds; // Reset the countdown timer.
                MachineLobbyMediator.Instance.FetchMachineList(); // Trigger machine list update.
            }
        }

        /// <summary>
        /// Callback method triggered when the machine list is fetched.
        /// </summary>
        private void OnMachineListFetched()
        {
            countdownTimer = updateIntervalSeconds; // Reset the countdown timer upon successful machine list fetch.
        }
    }
}