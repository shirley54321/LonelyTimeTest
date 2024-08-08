using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Server_Message : MonoBehaviour
{
    // Start is called before the first frame update



    public bool client_error;
    public bool server_error;
    public bool multi_error;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (client_error) {
            ServerEventHandler.CallClient_connection_error_event(); 
        
        
        }
        if( server_error) ServerEventHandler.CallServer_connection_error_event();
        if (multi_error) ServerEventHandler.CallMultiLogin_error_event();
    }
}
