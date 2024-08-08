namespace UserAccount
{
    /// <summary>
    /// Enumeration representing various failure codes for user registration.
    /// </summary>
    public enum RegisterFailedCode
    {
        /// <summary>
        /// The provided account or password format is incorrect.
        /// </summary>
        AccountOrPasswordFormatWrong,

        /// <summary>
        /// The entered passwords do not match.
        /// </summary>
        PasswordUnequal,

        /// <summary>
        /// The account being registered has already been registered.
        /// </summary>
        AccountHaveRegistered,

        /// <summary>
        /// The registration count has reached its limit.
        /// </summary>
        RegisterCountReachLimited,

        /// <summary>
        /// Other unspecified registration failure.
        /// </summary>
        Other
    }
}