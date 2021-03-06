namespace MoonWai.Api.Definitions
{
    public enum ErrorId 
    {
        AllowedEditTime = 0,
        BoardNotFound,
        FailedToCreateNewThread,
        FailedToCreateNewUser,
        FailedToCreateUserSettings,
        FailedToUpdateSettings,
        FailedToUpdateThread,
        NotAllowedToPostInThisBoard,
        PageCantBeSmallerThanOne,
        PageSizeCantZeroOrNegative,
        PasswordCantBeEmpty,
        PasswordLengthCantBeLessThan,
        PasswordsDontMatch,
        PostNotFound,
        ThreadNotFound,
        UserIsAlreadyRegistered,
        UserNotFound,
        UsernameCantBeEmpty,
        WrongPassword,
    }
}
