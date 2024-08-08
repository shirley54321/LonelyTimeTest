using SlotTemplateK2B.Legacy.UIControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollViewContentAdaption : MonoBehaviour
{
    private void Start()
    {
        RectTransform rectPage = gameObject.GetComponent<RectTransform>();
        float height = rectPage.sizeDelta.y;
        float width = gameObject.transform.parent.GetComponent<RectTransform>().rect.width;
        int ChildCount = transform.childCount;
        rectPage.sizeDelta = new Vector2(width * (ChildCount - 1), height);

        Vector3 parentPosition = gameObject.transform.parent.position;
        Vector3 selfPosition = gameObject.transform.position;
        Debug.Log(parentPosition.x + " " + selfPosition.x);

        if(parentPosition.x<0 && selfPosition.x<0)//同為負數
        {
            gameObject.transform.position = new Vector3(parentPosition.x, selfPosition.y, selfPosition.z);
        }
        else if(parentPosition.x > 0 && selfPosition.x > 0)//同為正數
        {
            gameObject.transform.position = new Vector3(parentPosition.x, selfPosition.y, selfPosition.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(-parentPosition.x, selfPosition.y, selfPosition.z);
        }
         gameObject.transform.parent.parent.parent.GetComponent<PageView>().Start();
    }
}
