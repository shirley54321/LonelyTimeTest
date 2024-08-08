using System;
using Games.Data;

namespace GameSelectedMenu
{
    /// <summary>
    /// The data for each game selector in the game lobby
    /// </summary>
    [Serializable]
    public class GameSelector
    {
        public GameId GameID; 
        /// <summary>
        /// The data for the game
        /// </summary>
        public GameProfile GameProfile;
        /// <summary>
        /// The data for the player favorite game
        /// </summary>
        public FavoriteGame FavoriteGame;

        public override string ToString()
        {
            return $"{nameof(GameProfile)}: {GameProfile},  {nameof(FavoriteGame)}: {FavoriteGame}";
        }
    }
}


