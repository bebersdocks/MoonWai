module MoonWai.Elmish.Pages.Catalog

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Elements
open MoonWai.Elmish.Http
open MoonWai.Elmish.Router
open MoonWai.Shared.Models

open Thoth.Json

type Model = 
    { Boards: BoardDto list
      InfoMsg: InfoMsg option
      Waiting: bool }

type Msg =
    | GetBoards
    | RetrieveBoardsFailed of string
    | RetrievedBoards of BoardDto list

let init =
    { Boards = []
      InfoMsg = None
      Waiting = false }, Cmd.ofMsg GetBoards

let getBoards (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<BoardDto list>(json, caseStrategy=CamelCase) with
        | Ok boards -> RetrievedBoards boards
        | Result.Error e -> RetrieveBoardsFailed e

    get "api/boards" ofSuccess RetrieveBoardsFailed

let update msg model : Model * Cmd<Msg> =
    match msg with
    | GetBoards ->
        { model with Waiting = true }, Cmd.OfPromise.result (getBoards model)

    | RetrieveBoardsFailed s ->
        { model with InfoMsg = Some (Error s); Waiting = false }, Cmd.none

    | RetrievedBoards boards ->
        { model with Boards = boards; Waiting = false }, Cmd.none


let boardView (board: BoardDto) =
    div [] [
        (link (Board board.Path) (sprintf "/%s/ - %s" board.Path board.Name))
    ]

let boardSectionView (boardSection: BoardSectionDto, boards: BoardDto list) =
    let boardSectionName = div [ ClassName "board-section__name" ] [ str (boardSection.Name) ]
    div [ ClassName "board-section" ] (boardSectionName :: (List.map boardView boards))

let catalogView (boards: BoardDto list) =
    boards 
    |> List.groupBy (fun (x: BoardDto) -> x.BoardSection)
    |> List.map boardSectionView

let view (model: Model) (dispatch: Dispatch<Msg>) =
    div [ ClassName "catalog" ] (catalogView model.Boards)
