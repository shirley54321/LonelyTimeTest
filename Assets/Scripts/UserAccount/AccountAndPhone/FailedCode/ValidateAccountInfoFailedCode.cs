namespace UserAccount
{
    /// <summary>
    /// Enumeration of possible failure codes when validating account information.
    /// </summary>
    public enum ValidateAccountInfoFailedCode
    {
        /// <summary>
        /// The account or phone provided is incorrect.
        /// </summary>
        AccountOrPhoneWrong,

        /// <summary>
        /// The image validation code provided is incorrect.
        /// </summary>
        ImageValidationCodeWrong
    }
}