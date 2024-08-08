using System;

namespace Games.Data
{
    /// <summary>
    /// Represents a record of spins counts before free bonus games for each individual machine.
    /// </summary>
    [Serializable]
    public class SpinsRecord
    {
        /// <summary>
        /// The identifier of the machine associated with the record.
        /// </summary>
        public int MachineId;

        /// <summary>
        /// The year for which the record.
        /// </summary>
        public int Year;

        /// <summary>
        /// The month for which the record.
        /// </summary>
        public int Month;

        /// <summary>
        /// The count of spins before triggering any bonus game.
        /// </summary>
        public int WithoutBonus;

        /// <summary>
        /// The count of spins before triggering the first bonus game.
        /// </summary>
        public int FirstBonus;

        /// <summary>
        /// The count of spins before triggering the second bonus game.
        /// </summary>
        public int SecondBonus;

        /// <summary>
        /// The count of spins before triggering the third bonus game.
        /// </summary>
        public int ThirdBonus;

        /// <summary>
        /// The count of spins before triggering the fourth bonus game.
        /// </summary>
        public int FourthBonus;
    }

}