module Main

type Page =
    | Home of Pages.Home.Model
    | Register of Pages.Register.Model
    | Login of Pages.Login.Model
    | NotFound

type Model =
    { ActivePage : Page
      CurrentRoute : Router.Route option }

type Msg =
    | HomeMsg of Pages.Home.Msg
    | RegisterMsg of Pages.Register.Msg
    | LoginMsg of Pages.Login.Msg

open Elmish
open Elmish.Navigation

let private initPage (route: Router.Route option) model =
    let model = { model with CurrentRoute = route }

    match route with
    | None ->
        { model with ActivePage = NotFound }, Cmd.none

    | Some Router.Route.Home ->
        let (homeModel, homeCmd) = Pages.Home.init ()
        { model with ActivePage = Home homeModel }, Cmd.map HomeMsg homeCmd

    | Some Router.Route.Register -> 
        let (registerModel, registerCmd) = Pages.Register.init () 
        { model with ActivePage = Register registerModel }, Cmd.map RegisterMsg registerCmd

    | Some (Router.Route.Login ) ->
        let (loginModel, loginCmd) = Pages.Login.init ()
        { model with ActivePage = Login loginModel }, Cmd.map LoginMsg loginCmd

let init (route : Router.Route option) =
    initPage route
        { ActivePage = Home (Pages.Home.init () |> (fun (model, _) -> model))
          CurrentRoute = Some Router.Route.Home }

let update (msg : Msg) (model : Model) =
    match model.ActivePage, msg with
    | NotFound, _ ->
        model, Cmd.none

    | Home homeModel, HomeMsg homeMsg ->
        let (homeModel, homeCmd) = Pages.Home.update homeMsg homeModel
        { model with ActivePage = Home homeModel }, Cmd.map HomeMsg homeCmd

    | Register registerModel, RegisterMsg registerMsg -> 
        let (registerModel, registerCmd) = Pages.Register.update registerMsg registerModel
        { model with ActivePage = Register registerModel }, Cmd.map RegisterMsg registerCmd

    | Login loginModel, LoginMsg loginMsg ->
        let (loginModel, loginCmd) = Pages.Login.update loginMsg loginModel
        { model with ActivePage = Login loginModel }, Cmd.map LoginMsg loginCmd

    | _, msg ->
        model, Cmd.none
        
open Elements
open Fable.React

let view (model : Model) (dispatch : Dispatch<Msg>) =
    let pageView = 
        match model.ActivePage with
        | NotFound ->
            div [] [
                str "404 Page not found"
            ]

        | Home homeModel ->
            Pages.Home.view homeModel (HomeMsg >> dispatch)

        | Register registerModel -> 
            Pages.Register.view registerModel (RegisterMsg >> dispatch)

        | Login loginModel ->
            Pages.Login.view loginModel (LoginMsg >> dispatch)

    div [] [ 
        viewLink Router.Route.Login "Login"
        viewLink Router.Route.Register "Register"
        viewLink Router.Route.Home "Home"
        pageView 
    ]

open Elmish.Debug
open Elmish.React
open Elmish.UrlParser

Program.mkProgram init update view
|> Program.toNavigable (parsePath Router.route) initPage
|> Program.withReactHydrate "elmish-app"
|> Program.withDebugger
|> Program.run