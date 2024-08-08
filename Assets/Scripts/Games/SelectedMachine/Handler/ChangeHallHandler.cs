using Games.Data;
using Loading;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Handles the process of changing the selected hall and related events.
    /// </summary>
    public class ChangeHallHandler : MonoBehaviour
    {
        #region Event

        /// <summary>
        /// Event triggered when VIP level is not sufficient for a selected hall.
        /// </summary>
        public static readonly UnityEvent<Hall> OnVipNotEnough = new UnityEvent<Hall>();

        /// <summary>
        /// Event triggered when a successful hall change occurs.
        /// </summary>
        public static readonly UnityEvent OnChangeHallSuccess = new UnityEvent();

        public static readonly UnityEvent<Hall> OnTryChangeHall = new UnityEvent<Hall>();

        #endregion

        #region Change Hall

        /// <summary>
        /// Changes the selected hall and requests the updated machine list if VIP level is sufficient.
        /// </summary>
        /// <param name="hall">The new selected hall.</param>
        public void ChangeHall(Hall hall)
        {
            // Open Loading Screen
            LoadingManger.Instance.Open_Loading_animator();
            
            OnTryChangeHall.Invoke(hall);
            if (EnableChangeHall(hall))
            {
                MachineLobbyMediator.Instance.SelectedHall = hall;
                MachineLobbyMediator.Instance.FetchMachineList();
                InventoryManager.Instance.InvokeUpdateDragonCoinEvent();
            }
            else
            {
                OnVipNotEnough.Invoke(hall);
                
                // Close Loading Screen
                LoadingManger.Instance.Close_Loading_animator();
            }
        }

        /// <summary>
        /// Checks if changing to the specified hall is allowed based on VIP level.
        /// </summary>
        /// <param name="hall">The hall to check.</param>
        /// <returns>True if changing to the hall is allowed; false otherwise.</returns>
        private bool EnableChangeHall(Hall hall)
        {
            var vipRight = PlayerInfoManager.Instance.GetVipRight();
            Debug.Log($"Vip Right {vipRight.SlotMachine}, Want Enter {hall}");
            return vipRight.SlotMachine >= hall;
        }

        #endregion
    }
}
