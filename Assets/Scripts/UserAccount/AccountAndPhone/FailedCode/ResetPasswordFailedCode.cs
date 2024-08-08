namespace UserAccount
{
    /// <summary>
    /// Enumeration of possible failure codes when resetting the password.
    /// </summary>
    public enum ResetPasswordFailedCode
    {
        /// <summary>
        /// The password format is incorrect.
        /// </summary>
        PasswordFormatWrong,

        /// <summary>
        /// The password and confirm password are not equal.
        /// </summary>
        PasswordUnequal,

        /// <summary>
        /// The new password is the same as the old password.
        /// </summary>
        PasswordIsSameAsOldPassword,

        /// <summary>
        /// The validation code for password reset is incorrect.
        /// </summary>
        ValidationCodeWrong
    }
}