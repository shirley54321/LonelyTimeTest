using Games.Data;
using Games.SelectedMachine;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameLobby.TopBar
{
    /// <summary>
    /// A MonoBehaviour to display the player's coin count in the UI.
    /// </summary>
    public class CoinDisplay : MonoBehaviour
    {
        [SerializeField] private bool needCheckHall; // Indicates if the display needs to check the selected Hall

        [SerializeField] private TextMeshProUGUI coin; // UI Text component to display coins

        /// <summary>
        /// Adds a listener to update the coin display whenever the dragon coin amount changes.
        /// </summary>
        /// 

        private void Update()
        {
            InventoryManager.OnDragonCoinChange.AddListener(UpdateCoin);
            if (int.TryParse(coin.text, out int coinValue))
            {
                if (coinValue <= 0)
                {
                    coin.text = "0";
                    InventoryManager.Instance.UpdateDragonCoin();                  
                }
            }          
        }
        private void OnEnable()
        {
            InventoryManager.OnDragonCoinChange.AddListener(UpdateCoin);
        }

        /// <summary>
        /// Removes the listener when the GameObject is disabled.
        /// </summary>
        private void OnDisable()
        {
            InventoryManager.OnDragonCoinChange.RemoveListener(UpdateCoin);
        }

        /// <summary>
        /// Updates the coin display based on the current amount of coins.
        /// </summary>
        /// <param name="rawCoin">The raw coin amount to be displayed.</param>
        private void UpdateCoin(int rawCoin)
        {
            if (rawCoin > -1)
            {
                decimal decimalCoin = rawCoin;
                // Check if the display needs to consider the selected Hall and apply specific logic if in the Beginner Hall
                if (needCheckHall)
                {
                    if (MachineLobbyMediator.Instance.SelectedHall == Hall.Beginner)
                    {
                        coin.text = $"{rawCoin:N2}"; // Format and display the coin value to 2 decimal places

                        return;
                    }
                }

                coin.text = $"{(decimalCoin / 100):N2}";  // Display the raw coin count as is for other cases
            }
            else
            {
                coin.text = "0";
                PlayerInfoManager.Instance.SetDragonCoin(0);

            }

        }
    }
}