using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;
using Loading;
using System.Collections;
using UserAccount.AccountInitializer;
using UserAccount;
using UnityEngine.Networking;
using UnityEngine.Purchasing;


public class PlayFabWithGoogle : MonoBehaviour
{

    public string webClientId = "105060803328-vcgr74lljhp8prroavjd5i2f4qc58j8e.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;


    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestAuthCode = true,
            UseGameSignIn = false
        };
        GoogleSignIn.Configuration = configuration;
    }

    public void LoginWithGoogle()
    {
        Debug.Log("開始登入google");
        OnSignIn();
    }
    public void OnSignIn()
    {
        Debug.Log("嘗試登入");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
          OnAuthenticationFinished);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        Debug.Log("登入結果:");
        if(task.IsFaulted)
        {
            using(IEnumerator<System.Exception> enumerator =
                    task.Exception.InnerExceptions.GetEnumerator())
            {
                if(enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error =
                            (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if(task.IsCanceled)
        {
            Debug.Log("取消登入");
        }
        else
        {
            Debug.Log("登入成功： " + task.Result.DisplayName + "!");
            string token = task.Result.AuthCode;
            Debug.Log("AuthCode : " + token);

            LoadingManger.instance.Open_Loading_animator();

            PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest
            {
                TitleId = PlayFabSettings.TitleId,
                ServerAuthCode = token,
                CreateAccount = true
            },
                result =>
                {
                    Debug.Log("google登入成功，且Playfab登入成功");
                    StartCoroutine(LoginSuccessCoroutine(result));
                },
                error =>
                {
                    Debug.Log("google登入成功，但Playfab登入失敗");
                    Debug.LogError("error訊息： " + error.GenerateErrorReport());
                    foreach(var kvp in error.ErrorDetails)
                    {
                        Debug.LogError($"{kvp.Key}: {string.Join(", ", kvp.Value)}");
                    }
                    PlayfabLoginFailed(error);
                });
            //LoginWithPlayFab(token);
        }
    }


    private IEnumerator LoginSuccessCoroutine(PlayFab.ClientModels.LoginResult result)
    {
        // Create Nick Name if create New Account
        if(result.NewlyCreated)
        {
            yield return AccountInitializer.CreateNickNameCoroutine();
        }
        UserAccountManager.Instance.HandlePlayFabLoginSuccess(result);
        LoadingManger.instance.Close_Loading_animator();
    }

    public void PlayfabLoginFailed(PlayFabError error)
    {
        UserAccountManager.Instance.HandlePlayFabLoginFailed(error);
        LoadingManger.instance.Close_Loading_animator();
    }

}

