namespace MoonWai.Shared.Definitions

open System

type LanguageId =
    | English = 0
    | Russian = 1

type TranslationId =
    | AllowedEditTime              = 1
    | BoardNotFound                = 2
    | FailedToCreateNewThread      = 3
    | FailedToCreateNewUser        = 4
    | FailedToCreateUserSettings   = 5
    | FailedToUpdateSettings       = 6
    | FailedToUpdateThread         = 7
    | NotAllowedToPostInThisBoard  = 8
    | PageCantBeSmallerThanOne     = 9
    | PageSizeCantZeroOrNegative   = 10
    | PasswordCantBeEmpty          = 11
    | PasswordLengthCantBeLessThan = 12
    | ThreadNotFound               = 13
    | UserIsAlreadyRegistered      = 14
    | UserNotFound                 = 15
    | UsernameCantBeEmpty          = 16
    | WrongPassword                = 17

module Common =
    let allowedEditTime       = TimeSpan.FromMinutes(5.0)
    let defaultBoardId        = 0
    let defaultThreadsPerPage = 15
    let maxPostsPerThread     = 1000
    let minPasswordLength     = 8
    let postsInPreviewCount   = 5
