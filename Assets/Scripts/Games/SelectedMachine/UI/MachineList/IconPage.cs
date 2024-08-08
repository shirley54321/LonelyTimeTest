using System;
using UnityEngine;

namespace Games.SelectedMachine
{
    [RequireComponent(typeof(RectTransform))]
    public class IconPage : MonoBehaviour
    {
        [SerializeField] private RectTransform upperPage, downPage;
        [SerializeField] private int halfPageCount = 5;
        private int currentCount;
        [SerializeField]private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            currentCount = 0;
        }

        public IconSelector SpawnSelector(GameObject selectorPrefab)
        {
            var icon = Instantiate(selectorPrefab, GetSpawnTransform());
            icon.transform.localScale = new Vector3(1, 1, 1);
            var iconSelector = icon.GetComponent<IconSelector>();
            currentCount++;

            return iconSelector;
        }

        private Transform GetSpawnTransform()
        {
            if (currentCount < halfPageCount)
            {
                return upperPage;
            }
            else
            {
                return downPage;
            }
        }

        public void SetWidth(float width)
        {
            _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
            upperPage.sizeDelta = _rectTransform.sizeDelta;
            downPage.sizeDelta = _rectTransform.sizeDelta;
        }

        public void SetUpperAndDownWidth()
        {
            int upperCount = 0; 
            int downCount = 0;
            
            if (currentCount < halfPageCount)
            {
                upperCount = currentCount;
            }
            else
            {
                upperCount = halfPageCount;
                downCount = currentCount - halfPageCount;
            }
            
            upperPage.sizeDelta *= (float) upperCount / halfPageCount;
            downPage.sizeDelta *= (float) downCount / halfPageCount;
        }
    }
}