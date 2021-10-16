namespace MoonWai.Shared.Definitions

type LanguageId =
    | English = 0
    | Russian = 1

type TranslationId =
    | AllowedEditTime = 1
    | BoardNotFound = 2
    | FailedToCreateNewThread = 3
    | FailedToCreateNewUser = 4
    | FailedToCreateUserSettings = 5
    | FailedToUpdateThread = 6
    | PageCantBeSmallerThanOne = 7
    | PageSizeCantZeroOrNegative = 8
    | PasswordCantBeEmpty = 9
    | PasswordLengthCantBeLessThan = 10
    | ThreadNotFound = 11
    | UserIsAlreadyRegistered = 12
    | UserNotFound = 13
    | UsernameCantBeEmpty = 14
    | WrongPassword = 15

open System

module Common =
    let allowedEditTime = TimeSpan.FromMinutes(5.0)
    let defaultBoardId = 0
    let defaultThreadsPerPage = 15
    let maxPostsPerThread = 1000
    let minPasswordLength = 8
    let postsInPreviewCount = 5
