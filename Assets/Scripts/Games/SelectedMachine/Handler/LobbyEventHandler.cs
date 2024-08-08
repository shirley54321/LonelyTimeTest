using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LobbyEventHandler
{
    public static event Action<Transform> OpenOtherPlayerInfoPage;
    public static void CallOpenOtherPlayerInfoPage( Transform transform)
    {

        OpenOtherPlayerInfoPage?.Invoke(transform);

    }


    public static event Action RefreshFriendList;
    public static void CallRefreshFriendList()
    {

        RefreshFriendList?.Invoke();

    }


    public static event Action RefreshVIP;
    public static void CallRefreshVIP()
    {
        RefreshVIP?.Invoke();

    }

}
