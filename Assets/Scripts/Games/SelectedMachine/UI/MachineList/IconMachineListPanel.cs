using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Games.Data;
using GameSelectedMenu;

namespace Games.SelectedMachine
{
    /// <summary>
    /// Manages the icon-style machine list panel UI and interactions.
    /// </summary>
    public class IconMachineListPanel : MonoBehaviour
    {
        [SerializeField] private List<IconSelector> selectors;

        
        [Header("Swiper Page")]
        [SerializeField] private PageSwiper pageSwiper;
        [SerializeField] private IconPage pagePrefab;
        [SerializeField] private Transform spawnPosition;
        [SerializeField] private List<IconPage> pages;
        [SerializeField] private float machineCountPerPage;
        [SerializeField] private int pageCount;
        
        [Header("Calculate Page Width")]
        [SerializeField] private CanvasScaler canvasScaler;
        [SerializeField] private RectTransform pageView;
        [SerializeField] private float pageWidth;
        [SerializeField] private float viewWidth;
        
        // Calculates the page width based on the screen and canvas sizes.
        private void Awake()
        {
            CalculatePageWidth();
        }


        
        /// <summary>
        /// Calculates and sets the page width based on the screen and canvas sizes.
        /// </summary>
        private void CalculatePageWidth()
        {
            // Get the reference width of the canvas from the CanvasScaler component
            float canvasWidth = canvasScaler.referenceResolution.x;

            // Get the current width of the screen
            float screenWidth = Screen.width;

            // Calculate the aspect ratio (width/height) assuming a base ratio of 9 (common for widescreens)
            float ratio_9 = ((float)Screen.width / Screen.height) * 9;

            // Calculate the target view width based on a range of 1000 to 1300 for different aspect ratios
            float viewWidth = 1300f - ((1300f - 1000f) / (18f - 14f)) * (18f - ratio_9);

            // Update the sizeDelta of the pageView RectTransform to set the calculated view width
            // while maintaining the original height
            pageView.sizeDelta = new Vector2(viewWidth, pageView.sizeDelta.y);

            // Calculate the page width based on the adjusted pageView width and the screen-to-canvas ratio
            float pageWidth = pageView.rect.width * screenWidth / canvasWidth;

            // Set the calculated page width using the PageSwiper component
            pageSwiper.SetPageWidth(pageView.rect.width, pageWidth);
        }

        
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

        #region UpdateUI
        
        /// <summary>
        /// Updates the UI with a list of machine information.
        /// </summary>
        /// <param name="machineInfos">List of machine information.</param>
        public void UpdateUI(List<Data.MachineInfo> machineInfos)
        {
            DestroyPreviousUI();
            pageCount = 0;
            
            IconPage page = CreatePage();
            GameMedia gameMedia = MachineLobbyMediator.Instance.GetGameMedia();

            int hasCreatedCount = 0;
            for (var i = 0; i < machineInfos.Count; i++)
            {
                if (hasCreatedCount != 0 && hasCreatedCount % machineCountPerPage == 0)
                {
                    page = CreatePage();
                }

                var machineInfo = machineInfos[i];

                var prefab = gameMedia.iconSelectedPrefab;
                var selector = page.SpawnSelector(prefab);
                selector.UpdateUI(machineInfo);
                selectors.Add(selector);
                hasCreatedCount++;
                
            }
            
            foreach (var iconPage in pages)
            {
                iconPage.SetUpperAndDownWidth();
            }
            
            pageSwiper.SetPageCount(pageCount);
        }

        
        private IconPage CreatePage()
        {
            IconPage page = Instantiate(pagePrefab, spawnPosition);
            Debug.Log($"pageView.sizeDelta {pageView.sizeDelta}");
            page.SetWidth(pageView.sizeDelta.x);
            pageCount++;
            
            pages.Add(page);
            return page;
        }

        private void DestroyPreviousUI()
        {
            foreach (var page in pages)
            {
                Destroy(page.gameObject);
            }

            pages.Clear();
            selectors.Clear();
        }

        #endregion

    }
}
