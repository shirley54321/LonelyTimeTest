using System;
using System.Collections.Generic;
using Player;

namespace Games.Data
{
    /// <summary>
    /// Represents a collection of machines, along with associated game profile and history records.
    /// </summary>
    [Serializable]
    public class MachineList
    {
        /// <summary>
        /// The game profile associated with this collection of machines.
        /// </summary>
        public GameProfile GameProfile;

        /// <summary>
        /// The historical highest record related to the game.
        /// </summary>
        public HistoryRecord HistoryRecord;

        /// <summary>
        /// A list containing detailed information about individual machines.
        /// </summary>
        public List<MachineInfo> MachineInfos;

        /// <summary>
        /// The hall where this machine is located.
        /// </summary>
        public Hall Hall;
        
        /// <summary>
        /// player's reserved game machines.
        /// </summary>
        public PlayerReserve PlayerReserve;
    }

}