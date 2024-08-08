using System;

namespace Games.Data
{
    /// <summary>
    /// Represents the types of games available in the game lobby.
    /// </summary>
    [Serializable]
    public enum GameType
    {
        /// <summary>
        /// Represents a slot machine game.
        /// </summary>
        SlotMachine,

        /// <summary>
        /// Represents a fish machine game.
        /// </summary>
        Fish,

        /// <summary>
        /// Represents a Journey to The West game.
        /// </summary>
        West,

        /// <summary>
        /// Represents a baccarat game.
        /// </summary>
        Happy,

        /// <summary>
        /// Represents a card game.
        /// </summary>
        Card
    }
}