using System;

namespace Games.Data
{
    /// <summary>
    /// Represents detailed information about an individual machine.
    /// </summary>
    [Serializable]
    public class MachineInfo
    {
        /// <summary>
        /// The unique identifier for this machine.
        /// </summary>
        public int MachineId;

        /// <summary>
        /// The identifier of the game associated with this machine.
        /// </summary>
        public GameId GameId;

        /// <summary>
        /// The hall where this machine is located.
        /// </summary>
        public Hall Hall;

        /// <summary>
        /// The machine's serial number
        /// </summary>
        public int MachineNumber;

        /// <summary>
        /// The win record for the machine in the previous month.
        /// </summary>
        public WinRecord UpperMonthRecord;

        /// <summary>
        /// The win record for the machine in the current month.
        /// </summary>
        public WinRecord ThisMonthRecord;

        /// <summary>
        /// Represents a record of spins counts before free bonus games for each individual machine.
        /// </summary>
        public SpinsRecord SpinsRecord;

        /// <summary>
        /// Information related to holding the machine.
        /// </summary>
        public ReserveMachineInfo ReserveMachineInfo;
    }

}