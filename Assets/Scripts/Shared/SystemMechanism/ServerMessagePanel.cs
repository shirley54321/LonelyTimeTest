using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayFab;

public class ServerMessagePanel : MonoBehaviour
{

    private bool Back_to_login = false;
    public GameObject NonInteractPanel;

    public TextMeshProUGUI MessageText;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    } //

    private void OnEnable()
    {
        ServerEventHandler.Client_connection_error_event += OnClient_connection_error_event;
        ServerEventHandler.Server_connection_error_event += OnServer_connection_error_event;
        ServerEventHandler.MultiLogin_error_event += OnMultiLogin_error_event;
        ServerEventHandler.Server_Error_Event += OnServer_Error_Event;
    }


    private void OnDisable()
    {
        ServerEventHandler.Client_connection_error_event -= OnClient_connection_error_event;
        ServerEventHandler.Server_connection_error_event -= OnServer_connection_error_event;
        ServerEventHandler.MultiLogin_error_event -= OnMultiLogin_error_event;
        ServerEventHandler.Server_Error_Event -= OnServer_Error_Event;
    }

    private void OnServer_Error_Event(PlayFabError error)
    {
       
        Debug.Log("On server error event ： "+error.Error.ToString());
        switch( error.Error)
        {

            case PlayFabErrorCode.ServiceUnavailable:
                MessageText.text = "伺服器連結失敗!請檢查您的網際網路是否連接正常並重新嘗試";
                NonInteractPanel.SetActive(true);
                Back_to_login = false;
                break;

            case PlayFabErrorCode.APIClientRequestRateLimitExceeded:
                MessageText.text = "呼叫伺服器頻率過高，請稍後再試";
                NonInteractPanel.SetActive(true);
                Back_to_login = false;
                break;

            case PlayFabErrorCode.DataUpdateRateExceeded:
                MessageText.text ="呼叫伺服器頻率過高，請稍後再試";
                NonInteractPanel.SetActive(true);
                Back_to_login = false;
                break;


            case PlayFabErrorCode.NotAuthenticated:
            case PlayFabErrorCode.NotAuthorized:
            case PlayFabErrorCode.OverLimit:
                // 1214 1089 1074
                MessageText.text = "伺服器發生錯誤!請重新登入!";
                NonInteractPanel.SetActive(true);
                Back_to_login = true;
                break;


            default:
                MessageText.text = "伺服器暫無回應，請稍後再試";
                NonInteractPanel.SetActive(true);
                Back_to_login = false;
                break;
        }

    }

    private void OnServer_connection_error_event()
    {
        MessageText.text = "伺服器發生錯誤 ! 請重新登入 !";
        NonInteractPanel.SetActive(true);
        Back_to_login = true;

    }

    private void OnClient_connection_error_event()
    {
        MessageText.text = "伺服器連結失敗 ! 請檢查您的網際網路是否連接正常並重新嘗試";
        NonInteractPanel.SetActive(true);
        if (SceneManager.GetActiveScene().name != "GameLobby")SceneManager.LoadScene("GameLobby");
        //Back_to_login = true;

    }


    private void OnMultiLogin_error_event()
    {
        MessageText.text = "您於其他裝置登入，如有帳號問題請立即反應至客服";
        NonInteractPanel.SetActive(true);
        Back_to_login = true;

    }


    public void ClosPanel()
    {
        NonInteractPanel.SetActive(false);
        if(Back_to_login == true&& (SceneManager.GetActiveScene().name != "Login") )SceneManager.LoadScene("Login");
    
    }

}


