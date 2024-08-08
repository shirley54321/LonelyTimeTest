using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using UserAccount;
using UserAccount.AccountInitializer;
using Player;
using Loading;
public class PlayFabWIthFB : MonoBehaviour
{
    public string TitleId;

    void Start()
    {
        if (FB.IsInitialized)
            return;
        FB.Init(() => FB.ActivateApp());
    }

    // public void LoginWithFacebook()
    // {
    //     FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, Res => { LoginWithPlayfab(); });
    // }
    public void LoginWithFacebook()
    {
        Debug.Log("¶}©lµn¤Jfacebook");
        FB.LogInWithReadPermissions(new List<string> { "public_profile", "email" }, OnHandFBResult);
    }

    public void OnHandFBResult(ILoginResult loginResult)
    {
        if(loginResult.Cancelled){
            Debug.Log("loginResult.Cancelled");
        }
        else if(loginResult.Error != null){
            Debug.Log("FB ERROR : "+loginResult.Cancelled);
        }
        else{
            Debug.Log("loginResult.success");
            LoadingManger.instance.Open_Loading_animator();
            PlayFabClientAPI.LoginWithFacebook(new PlayFab.ClientModels.LoginWithFacebookRequest
            {
                TitleId = TitleId,
                AccessToken = AccessToken.CurrentAccessToken.TokenString,
                CreateAccount = true
            }, PlayfabLoginSuccess, PlayfabLoginFailed);
        }
    }

    // public void LoginWithPlayfab()
    // {
    //     LoadingManger.instance.Open_Loading_animator();
    //     PlayFabClientAPI.LoginWithFacebook(new PlayFab.ClientModels.LoginWithFacebookRequest
    //     {
    //         TitleId = TitleId,
    //         AccessToken = AccessToken.CurrentAccessToken.TokenString,
    //         CreateAccount = true
    //     }, PlayfabLoginSuccess, PlayfabLoginFailed);
    // }

    public void PlayfabLoginSuccess(PlayFab.ClientModels.LoginResult result)
    {
        Debug.Log("LoginSucessful");
        
        StartCoroutine(LoginSuccessCoroutine(result));
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
