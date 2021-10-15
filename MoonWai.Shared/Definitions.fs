namespace MoonWai.Shared.Definitions

type LanguageId =
    | English = 0
    | Russian = 1

type TranslationId =
    | BoardNotFound = 0
    | FailedToCreateNewUser = 1
    | FailedToCreateUserSettings = 2
    | PageCantBeSmallerThanOne = 3
    | PageSizeCantZeroOrNegative = 4
    | PasswordCantBeEmpty = 5
    | PasswordLengthCantBeLessThan = 6
    | UserIsAlreadyRegistered = 7
    | UserNotFound = 8
    | UsernameCantBeEmpty = 9
    | WrongPassword = 10

module Common =
    let defaultBoardId = 0
    let defaultThreadsPerPage = 15
    let maxPostsPerThread = 1000
    let minPasswordLength = 8
    let postsInPreviewCount = 5
