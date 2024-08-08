using System;
using System.Collections.Generic;
using System.Linq;
using Games.Data;
using PlayFab.SharedModels;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.Data
{
    /// <summary>
    /// The data for the game in the lobby
    /// The data save in PlayFab
    /// </summary>
    [Serializable]
    public class GameProfileList : PlayFabBaseModel
    {
        public List<GameProfile> GameProfiles;
        
        public GameProfile GetData(GameId gameID)
        {
            var data = GameProfiles.FirstOrDefault(x => x.GameId == gameID);
            if (data == null)
            {
                Debug.LogError($"GameId: {gameID} has not set {nameof(GameProfile)}\n" +
                               $"Please go to PlayFab set data");
            }
            return data;
        }
        
        public override string ToString()
        {
            string combinedString = string.Join("\n, ", GameProfiles);
            return $"{nameof(GameProfiles)}: \n{combinedString}";
        }
    }
}