namespace MoonWai.Shared.Models

open System
open System.Collections.Generic

open MoonWai.Shared.Definitions

[<CLIMutable>]
type ErrorDto =
    { Message: string }

[<CLIMutable>]
type BoardSectionDto =
    { BoardSectionId: int 
      Name: string }

[<CLIMutable>]
type BoardDto =
    { BoardId: int
      BoardSection: BoardSectionDto 
      Path: string
      Name: string }

type LoginDto =
    { Username: string
      Password: string
      Trusted: bool }

type RegisterDto =
    { Username: string 
      Password: string
      LanguageId: LanguageId }

type UserSettingsDto =
    { LanguageId: LanguageId
      DefaultBoardId: int
      DefaultBoardPath: string }

[<CLIMutable>]
type PostDto =
    { PostId: int
      Message: string 
      CreateDt: DateTime }

[<CLIMutable>]
type ThreadDto =
    { ThreadId: int
      ParentId: int
      Title: string
      Message: string
      Posts: List<PostDto>
      PostsCount: int
      CreateDt: DateTime }

type InsertThreadDto =
    { Title: string
      Message: string
      BoardId: int
      UserId: Nullable<int> }

type UpdateThreadDto =
    { ThreadId: int
      Title: string 
      Message: string }
