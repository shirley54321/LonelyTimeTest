using System;
using Games.Data;

namespace Games.Data
{
    /// <summary>
    /// Represents the profile for a specific game.
    /// </summary>
    [Serializable]
    public class GameProfile
    {
        /// <summary>
        /// The identifier of the game.
        /// </summary>
        public GameId GameId;

        /// <summary>
        /// The type of the game.
        /// </summary>
        public GameType GameType;

        /// <summary>
        /// The name of the game.
        /// </summary>
        public string GameName;

        /// <summary>
        /// The mark with the game(New, Hot).
        /// </summary>
        public Mark Mark;

        /// <summary>
        /// The unlock player level required to access the game.
        /// </summary>
        public int UnlockLevel;

        /// <summary>
        /// The burst level of the game.
        /// </summary>
        public int BurstLevel;

        /// <summary>
        /// The stable level of the game.
        /// </summary>
        public int StableLevel;

        public override string ToString()
        {
            return $"{nameof(GameId)}: {GameId}, {nameof(GameType)}: {GameType}, {nameof(GameName)}: {GameName}, {nameof(Mark)}: {Mark}, {nameof(UnlockLevel)}: {UnlockLevel}, {nameof(BurstLevel)}: {BurstLevel}, {nameof(StableLevel)}: {StableLevel}";
        }
    }

}
