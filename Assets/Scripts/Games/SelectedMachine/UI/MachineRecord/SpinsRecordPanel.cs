using Games.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the spins record panel UI for a machine.
    /// </summary>
    public class SpinsRecordPanel : MonoBehaviour
    {
        [SerializeField] private Text withoutBonus, firstBonus, secondBonus, thirdBonus;

        /// <summary>
        /// Opens the spins record panel.
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the spins record panel.
        /// </summary>
        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// Updates the UI with spins record information.
        /// </summary>
        /// <param name="spinsRecord">Spins record data.</param>
        public void UpdateUI(SpinsRecord spinsRecord)
        {
            withoutBonus.text = $"{spinsRecord.WithoutBonus}";
            firstBonus.text =  spinsRecord.FirstBonus == -1 ? "-" : $"{spinsRecord.FirstBonus}";
            secondBonus.text = spinsRecord.SecondBonus == -1 ? "-" : $"{spinsRecord.SecondBonus}";
            thirdBonus.text = spinsRecord.ThirdBonus == -1 ? "-" : $"{spinsRecord.ThirdBonus}";
        }
    }
}