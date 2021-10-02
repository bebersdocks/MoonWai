namespace MoonWai.Shared.Auth

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
