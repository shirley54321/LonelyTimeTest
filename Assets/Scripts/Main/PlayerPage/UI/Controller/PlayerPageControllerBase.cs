using Main.PlayerPage.UI.HonorWall;
using Main.PlayerPage.UI.PersonalPage;
using UnityEngine;

namespace Main.PlayerPage.UI.Controller
{
    /// <summary>
    /// Base class for controlling player pages, providing methods for updating UI and closing the panel.
    /// </summary>
    public abstract class PlayerPageControllerBase : MonoBehaviour
    {
        [SerializeField] protected PersonalPageDisplay personalPageDisplay;
        [SerializeField] protected HonorWallDisplay honorWallDisplay;

        /// <summary>
        /// Updates the UI elements on the personal page.
        /// </summary>
        public abstract void UpdatePersonalPageUI();

        /// <summary>
        /// Updates the UI elements on the honor hall page.
        /// </summary>
        public abstract void UpdateHonorHallUI();

        /// <summary>
        /// Closes the panel associated with the player page.
        /// </summary>
        public abstract void ClosePanel();
    }
}