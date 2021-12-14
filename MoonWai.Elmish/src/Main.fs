module MoonWai.Elmish.Main

open Elmish
open Elmish.Debug
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Components.Nav
open MoonWai.Elmish.Elements
open MoonWai.Elmish.Pages
open MoonWai.Elmish.Router
open MoonWai.Shared.Models

type Page =
    | Catalog of Catalog.Model
    | Board of Board.Model
    | Thread of Thread.Model
    | Register of Register.Model
    | Login of Login.Model
    | NotFound

type Model =
    { ActivePage: Page
      CurrentRoute: Route option
      User: UserDto option }

type Msg =
    | CatalogMsg of Catalog.Msg
    | BoardMsg of Board.Msg
    | ThreadMsg of Thread.Msg
    | RegisterMsg of Register.Msg
    | LoginMsg of Login.Msg

let private initPage (route: Route option) (model: Model) =
    let (activePage, cmd) =
        match route with
        | None -> NotFound, Cmd.none
        | Some Route.Catalog -> Catalog.init |> (fun (model, cmd) -> Catalog model, Cmd.map CatalogMsg cmd)
        | Some (Route.Board boardPath) -> Board.init boardPath |> (fun (model, cmd) -> Board model, Cmd.map BoardMsg cmd)
        | Some (Route.Thread threadId) -> Thread.init threadId |> (fun (model, cmd) -> Thread model, Cmd.map ThreadMsg cmd)
        | Some Route.Register -> Register.init model.User |> (fun (model, cmd) -> Register model, Cmd.map RegisterMsg cmd)
        | Some Route.Login -> Login.init model.User |> (fun (model, cmd) -> Login model, Cmd.map LoginMsg cmd)

    { model with ActivePage = activePage; CurrentRoute = route }, cmd

let init (route: Route option) =
    let activePage = Catalog (Catalog.init |> (fun (model, _) -> model))
    let model = { ActivePage = activePage; CurrentRoute = Some Route.Catalog; User = None }

    initPage route model

let update (msg: Msg) (model: Model) =
    match model.ActivePage, msg with
    | Catalog catalogModel, CatalogMsg msg ->
        let (catalogModel, cmd) = Catalog.update msg catalogModel
        { model with ActivePage = Catalog catalogModel }, Cmd.map CatalogMsg cmd

    | Board boardModel, BoardMsg msg ->
        let (boardModel, cmd) = Board.update msg boardModel
        { model with ActivePage = Board boardModel }, Cmd.map BoardMsg cmd

    | Thread threadModel, ThreadMsg msg ->
        let (threadModel, cmd) = Thread.update msg threadModel
        { model with ActivePage = Thread threadModel }, Cmd.map ThreadMsg cmd

    | Register registerModel, RegisterMsg msg ->
        let (registerModel, cmd) = Register.update msg registerModel 
        { model with ActivePage = Register registerModel; User = registerModel.User }, Cmd.map RegisterMsg cmd

    | Login loginModel, LoginMsg msg ->
        let (loginModel, cmd) = Login.update msg loginModel
        { model with ActivePage = Login loginModel; User = loginModel.User }, Cmd.map LoginMsg cmd

    | _, _ ->
        { model with ActivePage = NotFound }, Cmd.none
        
let view (model: Model) (dispatch: Dispatch<Msg>) =
    let pageView = 
        match model.ActivePage with
        | NotFound -> div [] [ str "404 Page not found" ]
        | Catalog model -> Catalog.view model (CatalogMsg >> dispatch)
        | Board model -> Board.view model (BoardMsg >> dispatch)
        | Thread model -> Thread.view model (ThreadMsg >> dispatch)
        | Register model -> Register.view model (RegisterMsg >> dispatch)
        | Login model -> Login.view model (LoginMsg >> dispatch)

    div [ ClassName "app" ] [
        navMenu None []
        div [ ClassName "content" ] [ pageView ]
    ]

Fable.Core.JsInterop.importAll "../public/css/main.scss"

Program.mkProgram init update view
|> Program.toNavigable (parsePath route) initPage
|> Program.withReactHydrate "elmish-app"
|> Program.withDebugger
|> Program.run