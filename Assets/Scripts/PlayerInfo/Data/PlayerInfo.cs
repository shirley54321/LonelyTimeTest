using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// In Unity, the data for an individual player (including other players)
    /// </summary>
    [Serializable]
    public class PlayerInfo
    {
        public string playFabId;
        public UserTitleInfo userTitleInfo;
        

        public UserProfile userProfile;
        public UserLevel userLevel;
        public int activity;
        public ActivityEvent activityEvent;
        public VIP vip;
        public ChangeNameRecord changeNameRecord;
        
        public List<MessageBoard> messageBoards;

        public Avatar avatar; 
        public Sprite vipIcon;

        public System.UInt64 vipLevelFromPlayFab;

        public int dragonCoin;
        public int dragonBall;
        
        public MessageBoard GetMessageBoard()
        {
            if (messageBoards != null)
            {
                if (messageBoards.Count > 0)
                {
                    return messageBoards[messageBoards.Count - 1];
                }
            }
            
            return new MessageBoard()
            {
                Message = "",
                UpdateTime = DateTime.Now
            };

        }

    }
}