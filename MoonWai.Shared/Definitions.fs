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
    | FailedToUpdateSettings = 6
    | FailedToUpdateThread = 7
    | PageCantBeSmallerThanOne = 8
    | PageSizeCantZeroOrNegative = 9
    | PasswordCantBeEmpty = 10
    | PasswordLengthCantBeLessThan = 11
    | ThreadNotFound = 12
    | UserIsAlreadyRegistered = 13
    | UserNotFound = 14
    | UsernameCantBeEmpty = 15
    | WrongPassword = 16

open System

module Common =
    let allowedEditTime = TimeSpan.FromMinutes(5.0)
    let defaultBoardId = 0
    let defaultThreadsPerPage = 15
    let maxPostsPerThread = 1000
    let minPasswordLength = 8
    let postsInPreviewCount = 5
