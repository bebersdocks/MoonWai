module MoonWai.Elmish.Main

open Elmish
open Elmish.Debug
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Elements
open MoonWai.Elmish.Pages
open MoonWai.Elmish.Router
open MoonWai.Shared.Models

type Page =
    | Catalog of Catalog.Model
    | Board of Board.Model
    | Register of Register.Model
    | Login of Login.Model
    | NotFound

type Model =
    { ActivePage : Page
      CurrentRoute : Route option }

type Msg =
    | CatalogMsg of Catalog.Msg
    | BoardMsg of Board.Msg
    | RegisterMsg of Register.Msg
    | LoginMsg of Login.Msg

let private initPage (route: Route option) model =
    let model = { model with CurrentRoute = route }

    let (activePage, cmd) =
        match route with
        | None -> NotFound, Cmd.none
        | Some Route.Catalog -> Catalog.init |> (fun (model, cmd) -> Catalog model, Cmd.map CatalogMsg cmd)
        | Some (Route.Board boardPath) -> Board.init boardPath |> (fun (model, cmd) -> Board model, Cmd.map BoardMsg cmd)
        | Some Route.Register -> Register.init None |> (fun (model, cmd) -> Register model, Cmd.map RegisterMsg cmd)
        | Some Route.Login -> Login.init None |> (fun (model, cmd) -> Login model, Cmd.map LoginMsg cmd)

    { model with ActivePage = activePage}, cmd

let init (route: Route option) =
    let activePage = Catalog (Catalog.init |> (fun (model, _) -> model))
    let model = { ActivePage = activePage; CurrentRoute = Some Route.Catalog }

    initPage route model

let update (msg: Msg) (model: Model) =
    let (activePage, cmd) =
        match model.ActivePage, msg with
        | Catalog model, CatalogMsg msg -> Catalog.update msg model |> (fun (model, cmd) -> Catalog model, Cmd.map CatalogMsg cmd)
        | Board model, BoardMsg msg -> Board.update msg model |> (fun (model, cmd) -> Board model, Cmd.map BoardMsg cmd)
        | Register model, RegisterMsg msg -> Register.update msg model |> (fun (model, cmd) -> Register model, Cmd.map RegisterMsg cmd)
        | Login model, LoginMsg msg -> Login.update msg model |> (fun (model, cmd) -> Login model, Cmd.map LoginMsg cmd)
        | _, _ -> NotFound, Cmd.none
    { model with ActivePage = activePage }, cmd
        
let view (model: Model) (dispatch: Dispatch<Msg>) =
    let pageView = 
        match model.ActivePage with
        | NotFound -> div [] [ str "404 Page not found" ]
        | Catalog catalogModel -> Catalog.view catalogModel (CatalogMsg >> dispatch)
        | Board boardModel -> Board.view boardModel (BoardMsg >> dispatch)
        | Register registerModel -> Register.view registerModel (RegisterMsg >> dispatch)
        | Login loginModel -> Login.view loginModel (LoginMsg >> dispatch)

    div [ ClassName "app" ] [
        navMenu None []
        div [ ClassName "content" ] [ pageView ]
    ]

Program.mkProgram init update view
|> Program.toNavigable (parsePath route) initPage
|> Program.withReactHydrate "elmish-app"
|> Program.withDebugger
|> Program.run