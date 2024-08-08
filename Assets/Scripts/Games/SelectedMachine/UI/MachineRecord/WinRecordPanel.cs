using Games.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the win record panel UI for a machine.
    /// </summary>
    public class WinRecordPanel : MonoBehaviour
    {
        [SerializeField] private Text upperBigWin, upperMegaWin, upperSuperWin, upperDragonWin;
        [SerializeField] private Text thisBigWin, thisMegaWin, thisSuperWin, thisDragonWin;
        
        /// <summary>
        /// Opens the win record panel.
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the win record panel.
        /// </summary>
        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Updates the UI with win record information.
        /// </summary>
        /// <param name="upperMonthRecord">Win record data for the upper month.</param>
        /// <param name="thisMonthRecord">Win record data for the current month.</param>
        public void UpdateUI(WinRecord upperMonthRecord, WinRecord thisMonthRecord)
        {
            upperBigWin.text = $"{upperMonthRecord.BigCount}";
            upperMegaWin.text = $"{upperMonthRecord.MegaCount}";
            upperSuperWin.text = $"{upperMonthRecord.SuperCount}";
            upperDragonWin.text = $"{upperMonthRecord.DragonCount}";
            
            thisBigWin.text = $"{thisMonthRecord.BigCount}";
            thisMegaWin.text = $"{thisMonthRecord.MegaCount}";
            thisSuperWin.text = $"{thisMonthRecord.SuperCount}";
            thisDragonWin.text = $"{thisMonthRecord.DragonCount}";
        }
    }
}