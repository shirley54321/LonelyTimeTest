using Games.Data;
using Games.SelectedMachine.Star;
using GameSelectedMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the user interface elements in the game profile panel.
    /// </summary>
    public class GameProfilePanel : MonoBehaviour
    {
        [SerializeField] private StarLevelUI burstStars, stableStars;
        [SerializeField] private GameMediaList gameMediaList;
        [SerializeField] private Image logo;

        /// <summary>
        /// Updates the UI elements with the game profile and history record information.
        /// </summary>
        /// <param name="gameProfile">The game profile information.</param>
        /// <param name="historyRecord">The history record information.</param>
        public void UpdateUI(GameProfile gameProfile, HistoryRecord historyRecord)
        {
            
            burstStars.SetLevel(gameProfile.BurstLevel);
            stableStars.SetLevel(gameProfile.StableLevel);
            logo.sprite = gameMediaList.GetData(gameProfile.GameId).logo;
        }
    }
}