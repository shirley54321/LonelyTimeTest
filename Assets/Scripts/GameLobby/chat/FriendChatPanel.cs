using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLobby;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Player;
using PlayFab.ClientModels;

using PlayFab;
using System;
using Tool;
using System.Threading.Tasks;
using Unity.VisualScripting;
using System.Net.Http;
using TMPro;
using UnityEngine.UI;
using Share.Tool;
using Loading;
using System.Threading;
using Unity.Services.Core.Environments;
using PlayFab.CloudScriptModels;


public class FriendChatPanel : MonoBehaviour
{

    private void OnEnable()
    {
        LobbyEventHandler.RefreshFriendList += OnRefreshFriendList;
    }



    private void OnDisable()
    {
        LobbyEventHandler.RefreshFriendList -= OnRefreshFriendList;


    }
    private void OnRefreshFriendList()
    {
        FriendChatPanelOpen();
    }

    public void FriendChatPanelOpen()
    {
        destroy();
        GetFriendsList();


    }

    private void GetFriendsList()
    {

        Debug.Log("GetFriendsList()");
        var request = new GetFriendsListRequest
        {

            ProfileConstraints = new PlayerProfileViewConstraints
            {

                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };

        PlayFabClientAPI.GetFriendsList(request, OnGetFriendsListSuccess, OnGetFriendsListError);




    }
    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        Debug.Log("Friends List Retrieved:");
        if (result.Friends != null)
        {
            foreach (var friend in result.Friends)
            {
                Debug.Log($"Friend PlayFabId: {friend.FriendPlayFabId}");
                Debug.Log($"Display Name: {friend.Profile?.DisplayName}");
                Debug.Log($"Avatar URL: {friend.Profile?.AvatarUrl}");
                // 打印更多的好友信息
                playerListContain.GetComponent<FriendToggle>().id = friend.FriendPlayFabId;
                playerListContain.GetComponent <FriendToggle>().Name = friend.Profile?.DisplayName;
                playerListContain.GetComponent <FriendToggle>().HeadIconURL = friend.Profile?.AvatarUrl;
    
                // objList[1].text = "活躍度：" + player.Data["activity"].Value;

                //   urlImageGetter.GettingSprite(playerListContain.GetComponent<FriendObj>().HeadIconURL, "avatar");

                //  objList2[2].sprite = urlImageGetter.sprite;

                GameObject friendItem = Instantiate(playerListContain, scrollContainer.transform);
                friendItem.GetComponent<FriendToggle>().SetHeadIconAndName(playerListContain.GetComponent<FriendToggle>().HeadIconURL);

            }
        }
        else
        {
            Debug.Log("No friends found.");
        }
    }



    private static void OnGetFriendsListError(PlayFabError error)
    {
        Debug.Log($"Error retrieving friends list: {error.ErrorMessage}");
    }
    public GameObject playerListContain;
    public GameObject scrollContainer;
    private async void destroy()
    {

        var lists = scrollContainer.GetComponentsInChildren<Transform>();
        for (int i = 1; i < lists.Length; i++)
        {
            Destroy(lists[i].gameObject);
        }
    }

}
