using System;
using System.Collections.Generic;
using System.Linq;
using Games.Data;
using UnityEngine;

namespace GameSelectedMenu
{
    /// <summary>
    /// The data list for the player favorite game in the game lobby
    /// The data save in PlayFab
    /// </summary>
    [Serializable]
    public class FavoriteGameList
    {
        public List<FavoriteGame> FavoriteGames = new List<FavoriteGame>();
        
        public FavoriteGame GetData(GameId gameID)
        {
            if (FavoriteGames == null)
            {
                Debug.Log("FavorGames is null");
                return new FavoriteGame()
                {
                    GameID = gameID,
                    IsFavorite = false
                };
            }
            
            var data = FavoriteGames.FirstOrDefault(x => x.GameID == gameID);
            if (data == null)
            {
                data = new FavoriteGame()
                {
                    GameID = gameID,
                    IsFavorite = false
                };
            }

            // Debug.Log($"Get favoriteGame {gameID}, {data}");
            
            return data;
        }

        public override string ToString()
        {
            string combinedString = string.Join("\n, ", FavoriteGames);
            return $"{nameof(FavoriteGames)}: \n{combinedString}";
        }
    }
    
    
}