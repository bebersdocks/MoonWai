module Main

type Page =
    | Home of Pages.Home.Model
    | Login of Pages.Login.Model
    | NotFound

type Model =
    { ActivePage : Page
      CurrentRoute : Router.Route option }

type Msg =
    | HomeMsg of Pages.Home.Msg
    | LoginMsg of Pages.Login.Msg

open Elmish
open Elmish.Navigation

let private setRoute (optRoute: Router.Route option) model =
    let model = { model with CurrentRoute = optRoute }

    match optRoute with
    | None ->
        { model with ActivePage = Page.NotFound }, Cmd.none

    | Some Router.Route.Home ->
        let (homeModel, homeCmd) = Pages.Home.init ()
        { model with ActivePage = Page.Home homeModel }, Cmd.map HomeMsg homeCmd

    | Some (Router.Route.Login ) ->
        let (loginModel, loginCmd) = Pages.Login.init ()
        { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd


let init (location : Router.Route option) =
    setRoute location
        { ActivePage = Page.Home (Pages.Home.init () |> (fun (model, msg) -> model))
          CurrentRoute = Some Router.Route.Home }

let update (msg : Msg) (model : Model) =
    match model.ActivePage, msg with
    | Page.NotFound, _ ->
        model, Cmd.none

    | Page.Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Pages.Home.update homeMsg homeModel
        { model with ActivePage = Page.Home homeModel }, Cmd.map HomeMsg homeCmd

    | Page.Login loginModel, LoginMsg loginMsg ->
        let (loginModel, loginCmd) = Pages.Login.update loginMsg loginModel
        { model with ActivePage = Page.Login loginModel }, Cmd.map LoginMsg loginCmd

    | _, msg ->
        model, Cmd.none

open Elements
open Fable.React

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let pageView = 
        match model.ActivePage with
        | Page.NotFound ->
            str "404 Page not found"

        | Page.Home homeModel ->
            Pages.Home.view homeModel (HomeMsg >> dispatch)

        | Page.Login loginModel ->
            Pages.Login.view loginModel (LoginMsg >> dispatch)

    div [] [ 
        viewLink Router.Route.Login "Login"
        viewLink Router.Route.Home "Home"
        pageView 
    ]

open Elmish.Debug
open Elmish.React
open Elmish.UrlParser

Program.mkProgram init update view
|> Program.toNavigable (parsePath Router.route) setRoute
|> Program.withReactHydrate "elmish-app"
|> Program.withDebugger
|> Program.run