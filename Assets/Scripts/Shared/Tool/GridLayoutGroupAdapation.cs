using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayoutGroupAdapation : MonoBehaviour
{
    [Header("調整寬")]
    [SerializeField] private bool AdaptationX;

    [Header("調整高")]
    [SerializeField] private bool AdaptationY;

    [Header("物件置中(勾選前請確認層級(Sroll>Viewport>Content))")]
    [SerializeField] private bool InMiddle;

    float RealWidth = Screen.width / (Screen.height / 1080f) / 2400f;//畫面真正寬度大小與原尺寸的比例
    float RealHeight = Screen.height/ (Screen.width / 2400f) / 1080f;//畫面真正高度大小與原尺寸的比例

    private void Start()
    {
        var Grid = gameObject.GetComponent<GridLayoutGroup>();

        if(AdaptationX)
        {
            var newX = Grid.cellSize.x*RealWidth;
            Grid.cellSize = new Vector2(newX, Grid.cellSize.y);
        }
        
        if(AdaptationY)
        {
            var newY = Grid.cellSize.y*RealHeight;
            Grid.cellSize = new Vector2(Grid.cellSize.x, newY);
        }
        
        if(InMiddle)
        {
            float cellWidth = Grid.cellSize.x;
            var ScrollView = gameObject.transform.parent.parent;//如果content被content size fitter會沒有width，View port也會因為錨點的關係沒有width，所以只能抓Scroll View
            float contentSize = ScrollView.GetComponent<RectTransform>().rect.width;

            float newPadding = (contentSize - (cellWidth * Grid.constraintCount) - (Grid.spacing.x * (Grid.constraintCount - 1))) / 2;

            Grid.padding.left = (int)newPadding;
        }
    }
}
