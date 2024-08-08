//using SlotTemplateK2B;
using System;
using System.Collections;
using System.Collections.Generic;
using Shared.RemindPanel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SlotTemplate;
using Unity.VisualScripting;


public class NotificationManager : MonoBehaviour
{
    public RemindPanel NonInteractPanel;
    //  public SlotMainBoard SlotStatus;
    public TextMeshProUGUI countDownText;

    public int inactivityDelay = 10; // Meaning is waiting for Two minutes

   
    public bool isLoginPage = false;
    public bool DisplayCanSleep = false;

    bool isStart = false;
    int pauseDateTime; 
    bool isPause = false;
    bool isNotSpin = true;
    bool closepanel = false;

    public float timer = 0;

    private WU_GameStatesManager wu_States;
    private PB_GameStatesManager pb_States;
    private WushihGameStatesManager wushih_States;
    private GameStatesManager chu_States;
    private AL_GameStatesManager al_States;


    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Time.timeScale = 1;
        isNotSpin = false;
       
        int NM_num = 0;
        foreach (GameObject fooObj in GameObject.FindGameObjectsWithTag("NotificationManager"))
        {
            NM_num++;
        }

        if (NM_num > 1) Destroy(gameObject);
    } 

    void Start()
    {
        isStart = true;
        isPause = false;
        //Allow Player to click 
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }


    void Update()
    {
        timer += Time.unscaledDeltaTime;

        // Debug.Log(timer);

        if (SceneManager.GetActiveScene().name == "Login")
        {
           
            isLoginPage = true;
        }


//        Debug.Log("IsNotSpin :" + isNotSpin);
       // if(SlotStatus != null && SlotStatus.CurrentStateName != "Idle") { isNotSpin = true; }
       // else { isNotSpin = false; }


        if(Input.GetMouseButtonDown(0) ) timer = 0;


        if (closepanel == true|| Input.touchCount > 0 || isNotSpin) 
        {
            //Debug.Log("Reset timer");
            //任何動作會重置計時器
            timer = 0;
           // NonInteractPanel.SetActive(false);
        }
        else
        {
            //Debug.Log($"The Time Checking: {elapsedTime/6000} Seconds");
            if (timer > 600 && !isPause && isStart)
            {
                NonInteractPanel.OpenPanel();

                countDownText.text = "連線逾時";
                //SceneManager.LoadScene("start");
                //if (SceneManager.GetActiveScene().name != "Login")
                //{
                //    SceneManager.LoadScene("GameLobby");
                //    //SceneManager.LoadScene("Login");
                //}
                isStart = false;
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                if (DisplayCanSleep) Debug.Log("允許睡眠");
                //     PlayFabClientAPI.ForgetAllCredentials();


            }

            else if(timer <600) { //十分鐘內 不允許變暗
               Screen.sleepTimeout = SleepTimeout.NeverSleep;
                if (DisplayCanSleep) Debug.Log("不允許睡眠");

            }

            else
            {
                Screen.sleepTimeout = SleepTimeout.SystemSetting;
                if (DisplayCanSleep) Debug.Log("允許睡眠");
            }
        }

        if(SceneManager.GetActiveScene().name == "WuMayNiang")
        {
            wu_States = FindObjectOfType<WU_GameStatesManager>();
            if (wu_States.CurrentStateName == "RoundRunning")
            {
                timer = 0;
            }
        }
        if (SceneManager.GetActiveScene().name == "PolarBear")
        {
            pb_States = FindObjectOfType<PB_GameStatesManager>();
            if (pb_States.CurrentStateName == "RoundRunning")
            {
                timer = 0;
            }
        }
        if (SceneManager.GetActiveScene().name == "LionDance")
        {
            wushih_States = FindObjectOfType<WushihGameStatesManager>();
            if (wushih_States.CurrentStateName == "RoundRunning")
            {
                timer = 0;
            }
        }
        if (SceneManager.GetActiveScene().name == "ChuHeHanJie")
        {
            chu_States = FindObjectOfType<GameStatesManager>();
            if (chu_States.CurrentStateName == "RoundRunning")
            {
                timer = 0;
            }
        }
        if (SceneManager.GetActiveScene().name == "ALaDing")
        {
            al_States = FindObjectOfType<AL_GameStatesManager>();
            if (al_States.CurrentStateName == "RoundRunning")
            {
                timer = 0;
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        isPause = pause;
    }
    public void ClosPanel()
    {
        Debug.Log("ClosePanel");
     
        timer = 0;
        isStart=true;
        SceneManager.LoadScene("start");
    }

}

//Spare Code

/*
public float runningTime = 5f;

runningTime -= Time.deltaTime;

//Not allow device to sleep 
Screen.sleepTimeout = SleepTimeout.NeverSleep;

if (!isLoginPage)
{
    countDownText.text = "您已10分鐘未操作，將於 " + Mathf.CeilToInt(runningTime).ToString() + " 秒後退出遊戲大廳";

    if (runningTime <= 0f)
    {
        ReturnToLogin();
    }
}
public void ReturnToLogin()
    {
        SceneManager.LoadScene("Login");
    }
 */
