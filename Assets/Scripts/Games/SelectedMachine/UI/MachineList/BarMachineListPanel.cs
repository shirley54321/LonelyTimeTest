using System.Collections.Generic;
using Games.Data;
using UnityEngine;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the bar-style machine list panel UI and interactions.
    /// </summary>
    public class BarMachineListPanel : MonoBehaviour
    {
        [SerializeField] private List<BarSelector> selectors;

        [SerializeField] private BarSelector SelectorPrefab;
        [SerializeField] private Transform spawnPosition;

        /// <summary>
        /// Opens the panel, making it visible.
        /// </summary>
        public void OpenPanel()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Closes the panel, making it invisible.
        /// </summary>
        public void ClosePanel()
        {
            gameObject.SetActive(false);
        }
            
        
        /// <summary>
        /// Updates the UI with a list of machine information.
        /// </summary>
        /// <param name="machineInfos">List of machine information.</param>
        public void UpdateUI(List<Data.MachineInfo> machineInfos)
        {
            DestroyPreviousUI();
            
            foreach (var machineInfo in machineInfos)
            {
                var selector = Instantiate(SelectorPrefab, spawnPosition);
                selector.UpdateUI(machineInfo);
                selectors.Add(selector);
                
            }
        }
        
        /// <summary>
        /// Destroys the previously created UI elements.
        /// </summary>
        private void DestroyPreviousUI()
        {
            foreach (var selector in selectors)
            {
                Destroy(selector.gameObject);
            }

            selectors.Clear();
        }

    }
}