using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class receiveAmount : MonoBehaviour
{
    public TMP_InputField inputField; //輸入送禮的金額
    public GameObject ErrorPanel; //如果低於金額會顯示的視窗
    public Button closeButton;


    // Start is called before the first frame update
    void Start()
    {
        ErrorPanel.SetActive(false); //先將低於送禮金額的視窗不顯示(初始狀態)
        inputField.onEndEdit.AddListener(CheckInputAmount); //確認輸入的數字是否有低於最低線制
        closeButton.onClick.AddListener(CloseErrorPanel);
    }

    //確任輸入金額是否低於最低額度，如果有的話會跳出AmountErrorPanel讓玩家自己關閉
   void CheckInputAmount(string input)
    {
        if (int.TryParse(input, out int amount))
        {
            if (amount < 200000)
            {
                ErrorPanel.SetActive(true);
            }
            else
            {
                ErrorPanel.SetActive(false);
            }
        }
        else
        {
            Debug.Log("輸入的不是有效數字");
        }

        
    }
    void CloseErrorPanel()
        {
            ErrorPanel.SetActive(false);
            Debug.Log("Panel closed by button.");
        }

}
