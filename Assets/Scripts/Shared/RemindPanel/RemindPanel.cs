using UnityEngine;

namespace Shared.RemindPanel
{
    /// <summary>
    /// Remind Panel
    /// </summary>
    public class RemindPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        
        [ContextMenu("Open Panel")]
        public void OpenPanel()
        {
            panel.SetActive(true);
        }

        [ContextMenu("Close Panel")]
        public void ClosePanel()
        {
            panel.SetActive(false);
        }
    }
}