using System;

namespace Games.Data
{
    /// <summary>
    /// Represents a record of various types of wins.
    /// </summary>
    [Serializable]
    public class WinRecord
    {
        /// <summary>
        /// The identifier of the machine associated with the win record.
        /// </summary>
        public int MachineId;

        /// <summary>
        /// The year for which the win record.
        /// </summary>
        public int Year;

        /// <summary>
        /// The month for which the win record.
        /// </summary>
        public int Month;

        /// <summary>
        /// The count of occurrences of big wins.
        /// </summary>
        public int BigCount;

        /// <summary>
        /// The count of occurrences of mega wins.
        /// </summary>
        public int MegaCount;

        /// <summary>
        /// The count of occurrences of super wins.
        /// </summary>
        public int SuperCount;

        /// <summary>
        /// The count of occurrences of dragon wins.
        /// </summary>
        public int DragonCount;
    }
}
