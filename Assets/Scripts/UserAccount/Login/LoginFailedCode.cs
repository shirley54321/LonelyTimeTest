namespace UserAccount
{
    /// <summary>
    /// Enumeration representing various failure codes for user login.
    /// </summary>
    public enum LoginFailedCode
    {
        /// <summary>
        /// The user account is banned, and login is not allowed.
        /// </summary>
        AccountBanned,
        
        /// <summary>
        /// Incorrect account or password provided during login.
        /// </summary>
        IncorrectAccountOrPassword,

        /// <summary>
        /// The number of incorrect password attempts has exceeded the allowed limit.
        /// </summary>
        IncorrectPasswordTimesExceedLimited,

        /// <summary>
        /// The user account has been canceled, and login is not allowed.
        /// </summary>
        AccountCanceledInConsiderationPeriod,
        
        /// <summary>
        /// The user account has been canceled.
        /// </summary>
        AccountCanceled,

        /// <summary>
        /// Other unspecified reasons for login failure.
        /// </summary>
        OtherReason,
    }
}