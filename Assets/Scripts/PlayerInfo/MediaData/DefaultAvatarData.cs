using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "AvatarData", menuName = "MediaData/AvatarData")]
    public class DefaultAvatarData: ScriptableObject
    {
        public List<Avatar> Avatars;


        public bool IsCustomerAvatar(string url)
        {
            var avatarIndex = GetAvatarIndex(url);
            
            return avatarIndex == -1;
        }

        public int GetAvatarIndex(string url)
        {
            for (var i = 0; i < Avatars.Count; i++)
            {
                if (Avatars[i].Url == url)
                    return i;
            }

            return -1;
        }
    }
}

