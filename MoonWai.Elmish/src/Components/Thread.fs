module MoonWai.Elmish.Components.Thread

open System

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Components.Nav
open MoonWai.Elmish.Elements
open MoonWai.Elmish.Router
open MoonWai.Shared.Models.Post
open MoonWai.Shared.Models.Thread

let dateTimeToStr (dt: DateTime) = 
    dt.ToLocalTime().ToString("dd/MMM/yyyy hh:mm")

let postView (post: PostDto) = 
    let idStr = sprintf "%s #%i" (dateTimeToStr post.CreateDt) post.PostId

    div [ ClassName "post" ] [
        div [ ClassName "post__header" ] [ str idStr ]
        div [ ClassName "post__body" ] [ str post.Message ]
    ]

let threadView (thread: ThreadDto) =
    let idStr = sprintf "%s %s #%i" thread.Title (dateTimeToStr thread.CreateDt) thread.ThreadId

    let postsView = Seq.map postView thread.Posts
    let view = [
        div [ ClassName "thread__header" ] [ 
            str idStr
            navLink (Thread thread.ThreadId) "Open"
        ]
        div [ ClassName "thread__body" ] [ str thread.Message ]
    ]

    div [ ClassName "thread" ] (Seq.append view postsView)
