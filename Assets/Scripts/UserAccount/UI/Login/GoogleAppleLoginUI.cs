using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;
using UserAccount.UI;
using UserAccount;
using UserAccount.Tool;
using System;
using Unity.Services.Authentication.PlayerAccounts;
using PlayFab.ClientModels;
using PlayFab;

public class GoogleAppleLoginUI : MonoBehaviour
{
    [SerializeField] private Button googleLoginBtn, appleLoginBtn;

    [SerializeField] private GoogleAppleLoginHandler GoogleAppleLoginController;

    [SerializeField] private AccountLoginHandler accountLoginHandler;

    [SerializeField] private CreateNewAccountRequester createNewAccountRequester;

    public GameObject promptBox;

    private void OnEnable()
    {
        googleLoginBtn.onClick.AddListener(LoginBtnPressed);
        appleLoginBtn.onClick.AddListener(LoginBtnPressed);
        GoogleAppleLoginController.OnSignedIn += LoginController_OnSignIn;
    }

    private void LoginController_OnSignIn(PlayerInfo playerInfo, string playerName)
    {
        Debug.Log("player unity id: " + playerInfo.Id + ", player unity name: " + playerName);

        InstanceRemindPanel.Instance.ClosePanel(); //把「登入失敗，重新登入」的提醒視窗關閉

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetUserAccountInfo = true
            },
            Username = playerInfo.Id.Substring(0, 14),
            Password = playerInfo.Id.Substring(14, 14)
        },
        // Success
        result =>
        {
            Debug.Log($"Login with Account: {playerInfo.Id.Substring(0, 14)} <color=green>success(in PlayFab)</color>");
            UserAccountManager.Instance.HandlePlayFabLoginSuccess(result, isAccountLogin: true);
        },
        // Failure
        error =>
        {
            //註冊，unity id前半當帳號後半當密碼
            createNewAccountRequester.CreateUserAccountWithPlayFab(playerInfo.Id.Substring(0, 14), playerInfo.Id.Substring(14, 14));

            //登入
            accountLoginHandler.StartLogin(playerInfo.Id.Substring(0, 14), playerInfo.Id.Substring(14, 14));
        }
        );
    }

    public async void LoginBtnPressed()
    {
        await GoogleAppleLoginController.InitSignIn();

        //等1秒開啟登入失敗視窗
        System.Threading.Thread.Sleep(1000);
        InstanceRemindPanel.Instance.OpenPanel("登入失敗，重新登入");
    }

    private void OnDisable()
    {
        googleLoginBtn.onClick.RemoveListener(LoginBtnPressed);
        appleLoginBtn.onClick.RemoveListener(LoginBtnPressed);
        GoogleAppleLoginController.OnSignedIn -= LoginController_OnSignIn;
    }
}
