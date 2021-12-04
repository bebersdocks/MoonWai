module MoonWai.Main

open Elmish
open Elmish.Debug
open Elmish.Navigation
open Elmish.React
open Elmish.UrlParser

open Fable.React
open Fable.React.Props

open MoonWai.Elements

type Page =
    | Catalog of Pages.Catalog.Model
    | Board of Pages.Board.Model
    | Register of Pages.Register.Model
    | Login of Pages.Login.Model
    | NotFound

type Model =
    { ActivePage : Page
      CurrentRoute : Router.Route option  }

type Msg =
    | CatalogMsg of Pages.Catalog.Msg
    | BoardMsg of Pages.Board.Msg
    | RegisterMsg of Pages.Register.Msg
    | LoginMsg of Pages.Login.Msg

let private initPage (route: Router.Route option) model =
    let model = { model with CurrentRoute = route }

    match route with
    | None ->
        { model with ActivePage = NotFound }, Cmd.none

    | Some Router.Route.Catalog ->
        let (catalogModel, catalogCmd) = Pages.Catalog.init
        { model with ActivePage = Catalog catalogModel }, Cmd.map CatalogMsg catalogCmd 

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

let update (msg : Msg) (model : Model) =
    match model.ActivePage, msg with
    | Catalog catalogModel, CatalogMsg catalogMsg ->
        let (catalogModel, catalogCmd) = Pages.Catalog.update catalogMsg catalogModel
        { model with ActivePage = Catalog catalogModel }, Cmd.map CatalogMsg catalogCmd

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

        | Catalog catalogModel -> 
            Pages.Catalog.view catalogModel (CatalogMsg >> dispatch)

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