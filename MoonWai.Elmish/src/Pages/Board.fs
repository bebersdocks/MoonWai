module Pages.Board

type BoardIdentifier = 
    | Id of int 
    | Path of string

open Elements
open MoonWai.Shared.Models

type Model = {
    BoardIdentifier: BoardIdentifier
    Threads: ThreadDto seq
    InfoMsg: InfoMsg
}
    
type Msg =
    | GetThreads
    | RetrievedThreads of ThreadDto seq
    | FailedToRetrieveThreads of string

open Elmish
open Router
open Thoth.Json

let getThreads (model: Model) =
    let url = 
        match model.BoardIdentifier with 
        | Id id -> "board" </> id </> "threads"
        | Path path -> path

    let ofSuccess json =
        match Decode.Auto.fromString<ThreadDto seq>(json, caseStrategy=CamelCase) with 
        | Ok threads -> RetrievedThreads threads
        | Result.Error e -> FailedToRetrieveThreads e

    Http.get url ofSuccess FailedToRetrieveThreads

let initById boardId =
    { BoardIdentifier = Id boardId; Threads = []; InfoMsg = Empty }, Cmd.ofMsg GetThreads

let initByPath boardPath =
    { BoardIdentifier = Path boardPath; Threads = []; InfoMsg = Empty }, Cmd.ofMsg GetThreads

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | GetThreads ->
        model, Cmd.OfPromise.result (getThreads model)

    | RetrievedThreads threads -> 
        { model with Threads = threads; InfoMsg = Empty }, Cmd.none
     
    | FailedToRetrieveThreads s -> 
        { model with Threads = []; InfoMsg = Error s }, Cmd.none

open Fable.React

let threadView (thread: ThreadDto) = 
    li [] [ 
        div [] [ 
            str thread.Message 
        ] 
    ]
    
let view (model: Model) (dispatch: Msg -> unit) =
    div [] [ 
        ul [] (model.Threads |> Seq.map threadView)
    ]