using System;
using System.Collections.Generic;
using System.Linq;
using Games.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameSelectedMenu
{
    /// <summary>
    /// The data for the game selector in the game lobby
    /// </summary>
    [Serializable]
    public class GameSelectorList
    {
        #region Data

        public List<GameSelector> GameSelectors;
        #endregion
        
        
        #region Create Game Selector List
        public void CreateGameMenuList(List<GameProfile> gameProfileList,  FavoriteGameList favoriteGameList)
        {
            GameSelectors = new List<GameSelector>();
            foreach (var gameProfile in gameProfileList)
            {
                GameId gameID = gameProfile.GameId;
                
                GameSelector selector = new GameSelector();
                selector.GameID = gameProfile.GameId;
                selector.GameProfile = gameProfile;
                selector.FavoriteGame = favoriteGameList.GetData(gameID);
                
                // Debug.Log($"add {nameof(GameSelector)} : {gameID} , {selector}");
                GameSelectors.Add(selector);
            }
        }

        public GameSelector GetData(GameId gameID)
        {
            var data = GameSelectors.FirstOrDefault(x => x.GameID == gameID);
            if (data == null)
            {
                Debug.LogError($"GameId: {gameID} has not set {nameof(GameSelector)}\n" +
                               $"Please go to PlayFab set data");
            }
            return data;
        }
        
        
        #endregion
        
        public override string ToString()
        {
            string combinedString = string.Join("\n, ", GameSelectors);
            return $"{nameof(GameSelectors)}: \n{combinedString}";
        }
    }
}