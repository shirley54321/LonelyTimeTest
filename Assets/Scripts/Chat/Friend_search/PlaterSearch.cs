using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine;
using PlayFab.DataModels;
using PlayFab.ProfilesModels;
using System.Collections.Generic;
using PlayFab.Json;
using System.Collections;
using UnityEngine.TextCore.Text;
using System;
using TMPro;
using System.Linq;


public class PlaterSearch : MonoBehaviour
{
    #region Friends
    public static PlaterSearch PlaterSearchInstance;
    //public GameObject leaderboardPanel;
    public GameObject listingPrefab;
    //public Transform listingContainer;
    //[SerializeField]
    Transform friendScrollView;
    public static List<FriendInfo> myFriends;
    public static List<FriendInfo> _friends = null;
    public GameObject parentForPlayerData;
    public GameObject PlayerData;
    string friendSearch;
    //[SerializeField]
    //GameObject friendPanel;
    enum FriendIdType { PlayFabId, Username, Email, DisplayName };

    public void Start()
    {
        PlaterSearchInstance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetFriends();
        }

    }
    #region button
    public void RunWaitFunction()
    {
        StartCoroutine(WaitForFriend());
    }
    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {

        }, result => {
            _friends = result.Friends;
            DisplayFriends(_friends); // triggers your UI
        }, DisplayPlayFabError);
    }
    public void InputFriendID(string idIn)
    {
        friendSearch = idIn;
    }
    public void SubmitFriendRequest()
    {
        AddFriend(FriendIdType.Username, friendSearch);
    }

   
    #endregion

    void UpatePlayerData()//ChatPagePanel
    {
        for (int i = 1; i < parentForPlayerData.transform.childCount; i++)
        {
            Transform child = parentForPlayerData.transform.GetChild(i);
            Destroy(child.gameObject);
        }
        for (int i = 0; i < myFriends.Count; i++)
        {
            GameObject newChild = Instantiate(PlayerData, parentForPlayerData.transform);

           //GameObject name= newChild.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
           GameObject name= newChild.transform.GetChild(0).gameObject;
            name.GetComponent<TMP_Text>().text= myFriends[i].TitleDisplayName;
        }
    }

    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        myFriends = friendsCache;
        Debug.Log("fall");

        foreach (FriendInfo f in friendsCache)
        {

            bool isFound = false;
            if (myFriends != null)
            {

                foreach (FriendInfo g in myFriends)
                {
                    if (f.FriendPlayFabId == g.FriendPlayFabId)
                        isFound = true;
                }

            }
            else
            {
                Debug.Log("fall");
            }
            Debug.Log(f.TitleDisplayName);

            if (isFound == false)
            {

                //GameObject listing = Instantiate(listingPrefab, friendScrollView);
                Debug.Log(f.TitleDisplayName);
            }
        }
       if(parentForPlayerData.activeSelf == false) UpatePlayerData();
    }



    IEnumerator WaitForFriend()
    {
        yield return new WaitForSeconds(2);
        GetFriends();
    }
   

    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        PlayFabClientAPI.AddFriend(request, result => {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }

    public static void RemoveFriend(string friendInfo)
    {
        FriendInfo friendWithSameDisplayName = myFriends.FirstOrDefault(friend => friend.TitleDisplayName == friendInfo);

        PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
        {
            FriendPlayFabId = friendWithSameDisplayName.FriendPlayFabId
        }, result => {
            _friends.Remove(friendWithSameDisplayName);
        }, DisplayPlayFabError);
        PlaterSearchInstance.RunWaitFunction();
    }
    public static void DisplayPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion Friends
}

