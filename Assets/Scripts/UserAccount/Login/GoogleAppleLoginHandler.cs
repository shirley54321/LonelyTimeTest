using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;

public class GoogleAppleLoginHandler : MonoBehaviour
{
    public event Action<PlayerInfo, string> OnSignedIn;

    private PlayerInfo playerInfo;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        PlayerAccountService.Instance.SignedIn += SignedIn;
    }

    private async void SignedIn()
    {
        try
        {
            string accessToken = PlayerAccountService.Instance.AccessToken;

            await SignInWithUnityAsync(accessToken);

            //不要用底下這行程式碼，會讓google或apple帳號與unity斷連，不知道怎麼找回來
            //await UnlinkUnityAsync(accessToken);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    async public Task InitSignIn()
    {
        await PlayerAccountService.Instance.StartSignInAsync();
    }

    async Task SignInWithUnityAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUnityAsync(accessToken);

            Debug.Log(message: "SignIn is successful");

            playerInfo = AuthenticationService.Instance.PlayerInfo;

            string name = await AuthenticationService.Instance.GetPlayerNameAsync();

            OnSignedIn?.Invoke(playerInfo, name);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async Task UnlinkUnityAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.UnlinkUnityAsync();
            Debug.Log("Unlink Unity is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    async Task UnlinkGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.UnlinkGoogleAsync();
            Debug.Log("Unlink Google is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    private void OnDestroy()
    {
        PlayerAccountService.Instance.SignedIn -= SignedIn;
    }
}