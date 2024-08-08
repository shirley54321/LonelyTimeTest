using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace SlotTemplateK2B.Legacy.UIControl {
    public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        private ScrollRect rect;
        private float targethorizontal = 0;
        private List<float> posList = new List<float>();//存四張圖片的位置(0, 0.333, 0.666, 1)
        private bool isDrag = true;
        private float startTime = 0;
        private float startDragHorizontal;
        private float endDragHorizontal;
        public static int curIndex = 0;

        public float speed = 4;      //滑動速度
        public float sensitivity = 0;
        public float[] dotPos;
        public GameObject dot;
        // public Slotgame_GUIController slotgame_GUIController;
        //public Toggle[] toggleArray;  //toggle开关
        //public Text curPage;


        public void Start()
        {
            rect = GetComponent<ScrollRect>();
            float horizontalLength = rect.content.rect.width - GetComponent<RectTransform>().rect.width;
            var _rectWidth = GetComponent<RectTransform>().rect.width;
            posList.Clear();
            for (int i = 0; i < rect.content.transform.childCount; i++)
            {
                posList.Add(_rectWidth * i / horizontalLength);
            }
            curIndex = 0;
            //toggleArray[0].isOn = true;
            //curPage.text = String.Format("當前頁碼：0");
        }

        void Update()
        {
            if (!isDrag)
            {
                startTime += Time.deltaTime;
                float t = startTime * speed;
                //rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, t);  //加速滑動效果
                rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, Time.deltaTime * speed); //缓慢均速滑動效果
            }
            // slotgame_GUIController.RefreshMechineData();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
            //開始拖動
            startDragHorizontal = rect.horizontalNormalizedPosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log("OnEndDrag");
            endDragHorizontal = rect.horizontalNormalizedPosition;
            //float posX = rect.horizontalNormalizedPosition;
            //int index = 0;
            //float offset = Mathf.Abs(posList[index] - posX);  //計算當前位置與第一頁的偏移量
            float dragOffset = startDragHorizontal - endDragHorizontal;
            if (dragOffset >= sensitivity)
            {
                if (curIndex != 0)
                {
                    curIndex -= 1;
                }
            }
            else if (dragOffset < -sensitivity)
            {
                if (curIndex != posList.Count-1)
                {
                    curIndex += 1;
                }         
            } 
            //for (int i = 1; i < posList.Count; i++)
            //{    //遍歷頁簽，選取偏移量最小的那個頁面
            //    float temp = Mathf.Abs(posList[i] - posX);
            //    if (temp < offset)
            //    {
            //        index = i;
            //        offset = temp;
            //    }
            //}
            //curIndex = index;
            targethorizontal = posList[curIndex]; //設置當前座標，更新函数進行插值
            isDrag = false;
            startTime = 0;
            //toggleArray[curIndex].isOn = true;
            //curPage.text = String.Format("當前頁碼：{0}", curIndex.ToString());
            if(dot!=null)
            {
                dot.transform.localPosition = new Vector3(dotPos[curIndex], dot.transform.localPosition.y, 0);
            }
           
        }

        public void pageTo(int index)
        {
            //Debug.Log("pageTo......");
            curIndex = index;
            targethorizontal = posList[curIndex]; //設置當前座標，更新函数進行插值
            isDrag = false;
            startTime = 0;
            //toggleArray[curIndex].isOn = true;
            //curPage.text = String.Format("當前頁碼：{0}", curIndex.ToString());
            dot.transform.localPosition = new Vector3(dotPos[curIndex], dot.transform.localPosition.y, 0);
        }

        public void NextPage()
        {
            if(curIndex < posList.Count-1)
            {
                curIndex++;
            }
            targethorizontal = posList[curIndex]; 
            isDrag = false;
            startTime = 0;
        }
        public void PreviousPage()
        {
            if (curIndex > 0)
            {
                curIndex--;
            }
            targethorizontal = posList[curIndex];
            isDrag = false;
            startTime = 0;
        }
    }
}
