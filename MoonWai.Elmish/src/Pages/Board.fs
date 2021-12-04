module MoonWai.Pages.Board

open System

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elements
open MoonWai.Http
open MoonWai.Router
open MoonWai.Shared.Models

open Thoth.Json

type Model = 
    { BoardPath: string
      Threads: ThreadDto list
      InfoMsg: InfoMsg option }
    
type Msg =
    | GetThreads
    | RetrievedThreads of ThreadDto list
    | FailedToRetrieveThreads of string

let getThreads (model: Model) =
    let url = "boards" </> model.BoardPath

    let ofSuccess json =
        match Decode.Auto.fromString<ThreadDto list>(json, caseStrategy=CamelCase) with
        | Ok threads -> RetrievedThreads threads
        | Result.Error e -> FailedToRetrieveThreads e

    get url ofSuccess FailedToRetrieveThreads

let init boardPath =
    { BoardPath = boardPath; Threads = []; InfoMsg = None }, Cmd.ofMsg GetThreads

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | GetThreads ->
        model, Cmd.OfPromise.result (getThreads model)

    | RetrievedThreads threads -> 
        { model with Threads = threads; InfoMsg = None }, Cmd.none
     
    | FailedToRetrieveThreads s -> 
        { model with Threads = []; InfoMsg = Some (Error s) }, Cmd.none

let dateTimeToStr (dt: DateTime) = 
    dt.ToLocalTime().ToString("dd/MMM/yyyy hh:mm:tt")

let postView (post: PostDto) = 
    let idStr = sprintf "%s #%i" (dateTimeToStr post.CreateDt) post.PostId

    div [ ClassName "post" ] [
        div [ ClassName "idBox" ] [ str idStr ]
        div [ ClassName "postMsg" ] [ str post.Message ]
    ]

let threadView (thread: ThreadDto) =
    let idStr = sprintf "%s %s #%i" thread.Title (dateTimeToStr thread.CreateDt) thread.ThreadId

    let postsView = Seq.map postView thread.Posts
    let view = [
        div [ ClassName "idBox" ] [ str idStr ]
        div [ ClassName "threadMsg" ] [ str thread.Message ]
    ]

    div [ ClassName "thread" ] (Seq.append view postsView)
    
let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "board" ] (Seq.map threadView model.Threads)
