namespace Games.Data
{
    /// <summary>
    /// Represents the different states a machine can be in.
    /// </summary>
    public enum MachineState
    {
        /// <summary>
        /// The machine is currently idle and not in use.
        /// </summary>
        Idle,

        /// <summary>
        /// The machine is currently being played by a user.
        /// </summary>
        Playing,

        /// <summary>
        /// The machine is currently on hold
        /// </summary>
        Reserved
    }
}