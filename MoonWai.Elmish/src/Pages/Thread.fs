module MoonWai.Elmish.Pages.Thread

open Elmish

open MoonWai.Elmish.Components.MessageBox
open MoonWai.Elmish.Components.Thread
open MoonWai.Elmish.Http
open MoonWai.Shared.Models.Thread

open Thoth.Json

type Model = 
    { ThreadId: int
      Thread: ThreadDto option
      InfoMsg: InfoMsg option }
    
type Msg =
    | GetThread
    | RetrievedThread of ThreadDto
    | FailedToRetrieveThread of string

let getThread (model: Model) =
    let url = sprintf "api/threads/%i" model.ThreadId

    let ofSuccess json =
        match Decode.Auto.fromString<ThreadDto>(json, caseStrategy=CamelCase) with
        | Ok thread -> RetrievedThread thread
        | Result.Error e -> FailedToRetrieveThread e

    get url ofSuccess FailedToRetrieveThread

let init threadId =
    { ThreadId = threadId; Thread = None; InfoMsg = None }, Cmd.ofMsg GetThread

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | GetThread ->
        model, Cmd.OfPromise.result (getThread model)

    | RetrievedThread thread ->
        { model with Thread = Some thread; InfoMsg = None }, Cmd.none
     
    | FailedToRetrieveThread s ->
        { model with Thread = None; InfoMsg = Some (Error s) }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    match model.Thread with
    | Some thread -> threadView thread
    | None -> messageBox model.InfoMsg
