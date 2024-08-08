using GameSelectedMenu;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClassificationGame : BasePanel
{
    private UIGameSelectedMenu gameSelectedMenu = new UIGameSelectedMenu();
    private void OnEnable()
    {
        //Canvas> Middle Panel> GameSelectedMenuUI
        gameSelectedMenu = GameObject.Find("Canvas").transform.GetChild(2).GetChild(2).GetComponent<UIGameSelectedMenu>();
    }
    public void DisplayGame(string Filter)
    {
        switch (Filter)
        {
            case "All":
                gameSelectedMenu.FilterAllGame();
                break;
            case "SlotGame":
                gameSelectedMenu.FilterTriggerGame();
                break;
            case "FishGame":
                gameSelectedMenu.FilterFishGame();
                break;
            case "CardGame":
                gameSelectedMenu.FilterCardGame();
                break;
            case "Favorite":
                gameSelectedMenu.FilterFavoriteGame();
                break;
        }
    }
    public void ClosePanel()
    {
        UIManager.Instance.ClosePanel(UIConst.ClassificationPanel);
    }
}
