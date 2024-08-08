using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;



public static class ServerEventHandler 
{


    public static event Action<PlayFabError> Server_Error_Event;
    public static void Call_Server_Error_Event(PlayFabError error)
    {
        Server_Error_Event?.Invoke(error);
    }

    public static event Action Client_connection_error_event;

    public static void CallClient_connection_error_event()
    {
        Client_connection_error_event?.Invoke();
    }




    public static event Action Server_connection_error_event;
    public static void CallServer_connection_error_event()
    {
        Server_connection_error_event?.Invoke();
    }


    public static event Action MultiLogin_error_event ;

    public static void CallMultiLogin_error_event()
    {
        MultiLogin_error_event?.Invoke();
    }




}
