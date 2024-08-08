using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class userLevelTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayExpText;

    void Start()
    {
        initial();
    }

    private void initial()  //呼叫此function，會回傳當前level、exp給Listener
    {
        userLevel userlevel = gameObject.AddComponent<userLevel>();
        userlevel.refreshUserExp();
    }

    public void addUserExpBtnPress()  //隨便做一個button，點擊觸發此function
    {
        userLevel userlevel = gameObject.AddComponent<userLevel>();
        //userlevel.addUserExp(10000); //點一下+10000 exp
    }

    private void displayUserExp(int exp)
    {
        displayExpText.text = "exp: " + exp.ToString();
    }

    private void OnEnable()
    {
        userLevel.OnUserExpChange.AddListener(displayUserExp);
    }

    private void OnDisable()
    {
        userLevel.OnUserExpChange.RemoveListener(displayUserExp);
    }
}
