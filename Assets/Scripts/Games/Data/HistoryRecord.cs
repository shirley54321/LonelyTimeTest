using System;

namespace Games.Data
{
    /// <summary>
    /// Represents the historical highest record for a specific game.
    /// </summary>
    [Serializable]
    public class HistoryRecord
    {
        /// <summary>
        /// The identifier of the game associated with this historical record.
        /// </summary>
        public GameId GameId;

        /// <summary>
        /// The hall where this record is located.
        /// </summary>
        public Hall Hall;

        /// <summary>
        /// The highest reward achieved in the game's history.
        /// </summary>
        public int HighestReward;

        /// <summary>
        /// The highest reward achieved of winners in the game's history.
        /// </summary>
        public string WinnerPlayFabId;

        /// <summary>
        /// The highest reward achieved of winners in the game's history.
        /// </summary>
        public string WinnerName;
    }

}