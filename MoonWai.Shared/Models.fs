namespace MoonWai.Shared.Models

type LoginDto = {
    Username: string
    Password: string
    Trusted: bool
}

open MoonWai.Shared.Definitions

type RegisterDto = {
    Username: string 
    Password: string
    LanguageId: LanguageId
}

type UserSettingsDto = {
    LanguageId: LanguageId
    DefaultBoardPath: string
}

open System

type ThreadDto = {
    Title: string 
    Message: string
    CreateDt: DateTime
}
