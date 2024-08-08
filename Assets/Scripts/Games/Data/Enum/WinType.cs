namespace Games.Data
{
    /// <summary>
    /// Represents the types of wins in a game.
    /// </summary>
    public enum WinType
    {
        /// <summary>
        /// 10 times(inclusive) ~ 50 times 
        /// </summary>
        Big,
        /// <summary>
        /// 50 times(inclusive) ~ 100 times 
        /// </summary>
        Mega,
        /// <summary>
        /// 100 times(inclusive) ~ 300 times 
        /// </summary>
        Super,
        /// <summary>
        /// 300 times(inclusive) or more
        /// </summary>
        Dragon,
    }
}