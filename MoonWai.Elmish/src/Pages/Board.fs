module Pages.Board

open Elements
open MoonWai.Shared.Models

type Model = {
    BoardPath: string
    Threads: ThreadDto list
    InfoMsg: InfoMsg
}
    
type Msg =
    | GetThreads
    | RetrievedThreads of ThreadDto list
    | FailedToRetrieveThreads of string

open Elmish
open Router
open Thoth.Json

let getThreads (model: Model) =
    let url = "boards" </> model.BoardPath </> "threads"

    let ofSuccess json =
        match Decode.Auto.fromString<ThreadDto list>(json, caseStrategy=CamelCase) with
        | Ok threads -> RetrievedThreads threads
        | Result.Error e -> FailedToRetrieveThreads e

    Http.get url ofSuccess FailedToRetrieveThreads

let init boardPath =
    { BoardPath = boardPath; Threads = []; InfoMsg = Empty }, Cmd.ofMsg GetThreads

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
        msgBox model.InfoMsg
    
        ul [] (model.Threads |> Seq.map threadView)
    ]