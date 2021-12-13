module MoonWai.Elmish.Pages.Board

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Components.MessageBox
open MoonWai.Elmish.Components.Thread
open MoonWai.Elmish.Http
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
    let url = sprintf "api/boards/%s" model.BoardPath

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

let view (model: Model) (dispatch: Msg -> unit) =
    div [ ClassName "board" ] (Seq.map threadView model.Threads)
