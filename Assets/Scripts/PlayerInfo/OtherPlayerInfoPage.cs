using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Share.Tool;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
public class OtherPlayerInfoPage : MonoBehaviour
{
    public Unity.Services.Lobbies.Models.Player player;

    public string playFabId;
    public string id;
    public string Name;
    public string HeadIconURL;
    public string Activity;
    public string VIP;
    public string USERLEVEL;
    public string DC;
    public string DB;
    public string MessageBoard;
    public List<Sprite> VipList;


    public void SetText()
    {
        NameText.GetComponent<TMP_Text>().text = Name;
        ActivityText.GetComponent<TMP_Text>().text = "活躍度:" + Activity;
        VIPText.GetComponent<TMP_Text>().text = "層級:VIP" + VIP;
        LEVELText.GetComponent<TMP_Text>().text = USERLEVEL;
        UIDText.GetComponent<TMP_Text>().text = id;
        DCText.GetComponent<TMP_Text>().text = DC;
        DBText.GetComponent<TMP_Text>().text = DB;
        DashBoardText.GetComponent<TMP_Text>().text = MessageBoard;
        if (VIP != "")
        {
            int tmp = int.Parse(VIP) - 1;
            if (tmp < 0) tmp = 0;

            VipCard.sprite = VipList[tmp];
        }


        if (IsFriend) AddFriendButtonText.text = "刪好友";
        else AddFriendButtonText.text = "加好友";

        if (IsBlackListPlayer) BlackListButtonText.text = "解黑單";
        else BlackListButtonText.text = "黑名單";
    }

    private void Update()
    {
        SetText();
    }
    public void SetValue(OpenOtherPlayerPanel.OtherPlayerInfoPageData otherPlayerInfoPageData)
    {
        NameText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.Name;
        DashBoardText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.DashBoard;
        ActivityText.GetComponent<TMP_Text>().text = "活躍度:" + otherPlayerInfoPageData.Activity;
        VIPText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.VIP;
        UIDText.GetComponent<TMP_Text>().text=otherPlayerInfoPageData.id;
        LEVELText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.LEVEL;
        DCText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.DC;
        DBText.GetComponent<TMP_Text>().text = otherPlayerInfoPageData.DB;

    }
    

    public GameObject HeadIconOBJ;
    public GameObject NameText;
    public GameObject DashBoardText;
    public GameObject ActivityText;
    public GameObject VIPText;
    public GameObject UIDText;
    public GameObject LEVELText;
    public GameObject DCText;
    public GameObject DBText;

    public Image VipCard;
    public bool IsFriend;
    public bool IsBlackListPlayer;
    public void CheckIfFriend(string playFabId)
    {
        var request = new GetFriendsListRequest
        {
            // 根据需要可以设置这个为true来包含更多的好友信息
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };

        PlayFabClientAPI.GetFriendsList(request, result =>
        {
            bool isFriend = false;
            if (result.Friends != null)
            {
                foreach (var friend in result.Friends)
                {
                    if (friend.FriendPlayFabId == playFabId)
                    {
                        isFriend = true;
                        break;
                    }
                }
            }

            if (isFriend)
            {
                Debug.Log($"{playFabId} is already a friend.");
                IsFriend = true;
            }
            else
            {
                Debug.Log($"{playFabId} is not a friend.");
                IsFriend = false;
            }
        }, error =>
        {
            Debug.LogError($"Error retrieving friends list: {error.GenerateErrorReport()}");
        });
    }
    BlacklistManager blacklistManager = new BlacklistManager();
    public void CheckIfBlackList(string playFabId)
    {
        Debug.Log("DOCHECK :" + playFabId);
        blacklistManager.CheckBlacklist(playFabId, isInBlacklist =>
        {
            if (isInBlacklist)
            {
                Debug.Log("Player is in the blacklist.");
                IsBlackListPlayer = true;
                
            }
            else
            {
                Debug.Log("Player is not in the blacklist.");
                IsBlackListPlayer = false;
            }
        });

    }



    public Text AddFriendButtonText;
    public Text BlackListButtonText;

    public void AddBlackListClick()
    {
        if(IsBlackListPlayer == false)
        {
            GetComponent<BlacklistManager>().ClickAddToBlacklist();


        }
        else
        {
            GetComponent<BlacklistManager>().ClickRemoveBlacklist();


        }

        IsBlackListPlayer = !IsBlackListPlayer;
    }


    public void AddFriendButtonClick()
    {
        if (IsFriend == false) AddFriendByPlayFabId(playFabId);

        else RemoveFriendByPlayFabId(playFabId);

        IsFriend = !IsFriend;
        //CheckIfFriend(playFabId);

        LobbyEventHandler.CallRefreshFriendList();
    }

    private void AddFriendByPlayFabId(string playFabId)
    {
        var request = new AddFriendRequest
        {
            FriendPlayFabId = playFabId
        };

        PlayFabClientAPI.AddFriend(request, OnAddFriendSuccess, OnAddFriendError);
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("Friend added successfully.");
    }

    private void OnAddFriendError(PlayFabError error)
    {
        Debug.LogError($"Error adding friend: {error.GenerateErrorReport()}");
    }

    private void RemoveFriendByPlayFabId(string playFabId)
    {
        var request = new RemoveFriendRequest
        {
            FriendPlayFabId = playFabId
        };

        PlayFabClientAPI.RemoveFriend(request, OnRemoveFriendSuccess, OnRemoveFriendError);
    }

    private void OnRemoveFriendSuccess(RemoveFriendResult result)
    {
        Debug.Log("Friend removed successfully.");
    }

    private void OnRemoveFriendError(PlayFabError error)
    {
        Debug.LogError($"Error removing friend: {error.GenerateErrorReport()}");
    }
}
