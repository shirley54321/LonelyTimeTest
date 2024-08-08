using UnityEngine;

namespace Main.PlayerPage.UI.HonorWall
{
    public class HonorWallDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        #region Show/Close Panel

        public void ShowPanel()
        {
            panel.SetActive(true);
        }

        public void ClosePanel()
        {
            panel.SetActive(false);
        }
        
        
        #endregion

        public void UpdateUI()
        {
            
        }
    }
}