using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class ChatDetail
{

    public string fromID;
    public string message;
    public DateTimeOffset sendTime;
    public string avatarUrl;
    public ChatMessage chatMessage;
    public string TimeText;
    public string targerID;
}