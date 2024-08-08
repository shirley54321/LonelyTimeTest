using System;

namespace Games.Data
{
    /// <summary>
    /// Represents information about reserved machine.
    /// </summary>
    [Serializable]
    public class ReserveMachineInfo
    {
        /// <summary>
        /// The identifier of the machine associated with the record.
        /// </summary>
        public int MachineId;

        /// <summary>
        /// The current state of the machine.
        /// </summary>
        public MachineState State;

        /// <summary>
        /// The PlayFab ID associated with the user who is holding the machine.
        /// </summary>
        public string PlayFabId;

        /// <summary>
        /// The date and time when the machine last play(enter machine, spin).
        /// </summary>
        public DateTime LastPlayTime;

        /// <summary>
        ///  The date and time when the machine was end reserved.
        /// </summary>
        public DateTime EndReserveTime;
    }


}