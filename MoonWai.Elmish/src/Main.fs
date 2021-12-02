module Main

open Elements

open Elmish
open Elmish.Debug
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser

open Fable.React
open Fable.React.Props

type Page =
    | Board of Pages.Board.Model
    | Register of Pages.Register.Model
    | Login of Pages.Login.Model
    | NotFound

type Model =
    { ActivePage : Page
      CurrentRoute : Router.Route option 
      Boards: BoardDto list
      BoardsFailedMessage: string option }

type Msg =
    | RetrieveBoards
    | RetrieveBoardsFailed of string
    | RetrievedBoards of BoardDto list
    | BoardMsg of Pages.Board.Msg
    | RegisterMsg of Pages.Register.Msg
    | LoginMsg of Pages.Login.Msg

let private initPage (route: Router.Route option) model =
    let model = { model with CurrentRoute = route }

    match route with
    | None ->
        { model with ActivePage = NotFound }, Cmd.none

    | Some (Router.Route.Board boardPath) ->
        let (boardModel, boardCmd) = Pages.Board.init boardPath
        { model with ActivePage = Board boardModel }, Cmd.map BoardMsg boardCmd

    | Some Router.Route.Register -> 
        let (registerModel, registerCmd) = Pages.Register.init None
        { model with ActivePage = Register registerModel }, Cmd.map RegisterMsg registerCmd

    | Some (Router.Route.Login ) ->
        let (loginModel, loginCmd) = Pages.Login.init None
        { model with ActivePage = Login loginModel }, Cmd.map LoginMsg loginCmd

let init (route : Router.Route option) =
    initPage route
        { ActivePage = Board (Pages.Board.init "" |> (fun (model, _) -> model))
          CurrentRoute = Some (Router.Route.Board "") }

let retrieveBoards (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<BoardDto list>(json, caseStrategy=CamelCase) with
        | Ok boards -> RetrieveBoards boards
        | Result.Error e -> RetrieveBoardsFailed e

    Http.get "/boards" ofSuccess RetrieveBoardsFailed

let update (msg : Msg) (model : Model) =
    match model.ActivePage, msg with
    | _, RetrieveBoards ->
        model, Cmd.OfPromise.result (retrieveBoards model)

    | _, RetrieveBoardsFailed s ->
        { model with BoardsFailedMessage = Some s }, Cmd.none

    | _, RetrieveBoards boards ->
        { model with Boards = boards }, Cmd.none

    | Board boardModel, BoardMsg boardMsg ->
        let (boardModel, boardCmd) = Pages.Board.update boardMsg boardModel
        { model with ActivePage = Board boardModel }, Cmd.map BoardMsg boardCmd

    | Register registerModel, RegisterMsg registerMsg ->
        let (registerModel, registerCmd) = Pages.Register.update registerMsg registerModel
        { model with ActivePage = Register registerModel }, Cmd.map RegisterMsg registerCmd

    | Login loginModel, LoginMsg loginMsg ->
        let (loginModel, loginCmd) = Pages.Login.update loginMsg loginModel
        { model with ActivePage = Login loginModel }, Cmd.map LoginMsg loginCmd

    | _, _ ->
        model, Cmd.none
        
let view (model : Model) (dispatch : Dispatch<Msg>) =
    let pageView = 
        match model.ActivePage with
        | NotFound ->
            div [] [
                str "404 Page not found"
            ]

        | Board boardModel ->
            Pages.Board.view boardModel (BoardMsg >> dispatch)

        | Register registerModel -> 
            Pages.Register.view registerModel (RegisterMsg >> dispatch)

        | Login loginModel ->
            Pages.Login.view loginModel (LoginMsg >> dispatch)

    div [ ClassName "app" ] [
        navMenu None []
        div [ ClassName "content" ] [ pageView ]
    ]

Program.mkProgram init update view
|> Program.toNavigable (parsePath Router.route) initPage
|> Program.withReactHydrate "elmish-app"
|> Program.withDebugger
|> Program.run