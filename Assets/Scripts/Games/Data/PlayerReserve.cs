using System;
using System.Collections.Generic;

namespace Games.Data
{
    // <summary>
    /// Represents a class for player's reserved game machines.
    /// </summary>
    [Serializable]
    public class PlayerReserve
    {
        /// <summary>
        /// List of game machines reserved by the player.
        /// </summary>
        public List<MachineInfo> reserveMachines;
    }
}