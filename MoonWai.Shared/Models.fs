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

[<CLIMutable>]
type PostDto = {
    PostId: int
    Message: string 
    CreateDt: DateTime
}

open System.Collections.Generic

[<CLIMutable>]
type ThreadDto = {
    ThreadId: int
    Title: string
    Message: string
    Posts: List<PostDto>
    PostsCount: int
    CreateDt: DateTime
}
