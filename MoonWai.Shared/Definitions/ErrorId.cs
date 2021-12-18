namespace MoonWai.Shared.Definitions
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
        ThreadNotFound,
        UserIsAlreadyRegistered,
        UserNotFound,
        UsernameCantBeEmpty,
        WrongPassword,
    }
}