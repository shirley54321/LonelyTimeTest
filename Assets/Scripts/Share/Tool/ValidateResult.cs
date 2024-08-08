namespace Share.Tool
{
    public enum ValidateResult
    {
        Pass,
        ContainSensitiveWord,
        ContainSpecialCharacters,
        TooShort,
        TooLong
    }
}