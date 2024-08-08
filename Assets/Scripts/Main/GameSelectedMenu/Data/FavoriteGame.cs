using System;
using Games.Data;

namespace GameSelectedMenu
{
    /// <summary>
    /// The data for the player favorite game in the game lobby
    /// </summary>
    [Serializable]
    public class FavoriteGame
    {
        public GameId GameID;
        public bool IsFavorite;

        public override string ToString()
        {
            return $"{nameof(GameID)}: {GameID}, {nameof(IsFavorite)}: {IsFavorite}";
        }
    }
}