namespace Player.AzureFunctionResult
{
    public class ChangeNickNameResult
    {
        public ChangeNickNameResultType ResultType;
        public string DisplayName;
    }

    public enum ChangeNickNameResultType
    {
        Success,
        NameHaveUsed,
        OtherError
    }
}