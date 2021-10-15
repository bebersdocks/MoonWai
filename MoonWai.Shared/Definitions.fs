namespace MoonWai.Shared.Definitions

type LanguageId =
    | English = 0
    | Russian = 1

type TranslationId = 
    | BoardNotFound = 0
    | FailedToCreateNewUser = 1
    | FailedToCreateUserSettings = 2
    | PasswordCantBeEmpty = 3
    | PasswordLengthCantBeLessThan = 4
    | UserIsAlreadyRegistered = 5
    | UserNotFound = 6
    | UsernameCantBeEmpty = 7
    | WrongPassword = 8

module Common =
    let defaultBoardId = 0
    let minPasswordLength = 8
