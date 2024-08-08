using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Games.SelectedMachine
{
    /// <summary>
    /// Allows swiping between pages using drag gestures.
    /// </summary>
    public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        private Vector3 panelLocation;
        public float percentThreshold = 0.2f;
        public float easing = 0.5f;
        public int totalPages = 1;
        [SerializeField] private int currentPage = 1;


        [SerializeField] private float _rawPageWidth;
        [SerializeField] private float pageWidth;
        
        private float canvasScaler = 108;
        private float _swiperX => _rawPageWidth / canvasScaler;

        // Start is called before the first frame update
        void Start()
        {
            panelLocation = transform.position;
        }

        #region Set Variable

        /// <summary>
        /// Sets the width of a single page.
        /// </summary>
        /// <param name="width">The width of a page.</param>
        public void SetPageWidth(float rawPageWidth, float width)
        {
            _rawPageWidth = rawPageWidth;
            pageWidth = width;
        }

        /// <summary>
        /// Sets the total count of pages.
        /// </summary>
        /// <param name="count">The total count of pages.</param>
        public void SetPageCount(int count)
        {
            totalPages = count;
        }

        #endregion

        #region Swip Page

        public void OnDrag(PointerEventData data)
        {
            float difference = data.pressPosition.x - data.position.x;
            transform.position = panelLocation - new Vector3(difference, 0, 0) / canvasScaler;
        }

        public void OnEndDrag(PointerEventData data)
        {
            float percentage = (data.pressPosition.x - data.position.x) / _swiperX;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                if (percentage > 0)
                {
                    TryNextPage();
                }
                else
                {
                    TryLastPage();
                }
            }
            else
            {
                StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            }
        }

        public void TryNextPage()
        {
            Vector3 newLocation = panelLocation;
            if (currentPage < totalPages)
            {
                currentPage++;
                // Debug.Log($"new Vector3(-_swiperX, 0, 0) {new Vector3(-_swiperX, 0, 0)}");
                newLocation += new Vector3(-_swiperX, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }

        public void TryLastPage()
        {
            Vector3 newLocation = panelLocation;
            if (currentPage > 1)
            {
                currentPage--;
                newLocation += new Vector3(_swiperX, 0, 0);
            }
            StartCoroutine(SmoothMove(transform.position, newLocation, easing));
            panelLocation = newLocation;
        }

        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
        {
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        #endregion
    }
}
