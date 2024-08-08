using System;
using Games.Data;
using UnityEngine;

namespace GameSelectedMenu
{
    /// <summary>
    /// Represents the media (sprite, audio, etc.) associated with a game in the lobby.
    /// </summary>
    [Serializable]
    public class GameMedia
    {
        /// <summary>
        /// The identifier of the game associated with this media.
        /// </summary>
        public GameId GameID;
        
        /// <summary>
        /// The scene name associated with the game.
        /// </summary>
        public string sceneName;

        public GameObject iconSelectedPrefab;

        public Sprite logo;

        /// <summary>
        /// Returns a string representation of the GameMedia.
        /// </summary>
        /// <returns>A formatted string containing the values of the GameMedia's properties.</returns>
        public override string ToString()
        {
            return $"{nameof(GameID)}: {GameID}";
        }
    }
}