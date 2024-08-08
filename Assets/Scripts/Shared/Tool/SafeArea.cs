using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SafeArea : MonoBehaviour
{
    float safeArea_left;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Save Area: " + Screen.safeArea.xMin);
        if(Screen.safeArea.xMin!=0)
        {
            safeArea_left = Screen.safeArea.xMin - 30;
        }
        else
        {
            safeArea_left = Screen.safeArea.xMin;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(safeArea_left, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }
        else
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, gameObject.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }
}
