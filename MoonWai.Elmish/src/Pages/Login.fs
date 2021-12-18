module MoonWai.Elmish.Pages.Login

open System

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Components.MessageBox
open MoonWai.Elmish.Elements
open MoonWai.Elmish.Http
open MoonWai.Elmish.Router
open MoonWai.Shared.Models.Auth
open MoonWai.Shared.Models.User

open Thoth.Json

type Model =
    { LoginDto: LoginDto
      User: UserDto option
      InfoMsg: InfoMsg option
      Waiting: bool }

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess of UserDto
    | LoginFailed of string

let login (model: Model) =
    let ofSuccess json =
        Browser.Dom.console.log (string json)
        match Decode.Auto.fromString<UserDto>(json, caseStrategy=CamelCase) with
        | Ok user -> LoginSuccess user
        | Result.Error e -> LoginFailed e

    post "api/auth/login" model.LoginDto ofSuccess LoginFailed

let init user =
    { LoginDto = LoginDto(Username = "", Password = "", Trusted = false);
      User = user;
      InfoMsg = None;
      Waiting = false; }, Cmd.none

let update msg model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username ->
        let infoMsg = if String.IsNullOrEmpty(username) then Some (Info "Username can't be empty") else None
        model.LoginDto.Username <- username
        { model with InfoMsg = infoMsg }, Cmd.none

    | ChangePassword password ->
        let infoMsg = if String.IsNullOrEmpty(password) then Some (Info "Password can't be empty") else None
        model.LoginDto.Password <- password
        { model with InfoMsg = infoMsg }, Cmd.none

    | ChangeTrusted trusted ->
        model.LoginDto.Trusted <- trusted
        model, Cmd.none

    | Login ->
        { model with Waiting = true }, Cmd.OfPromise.result (login model)

    | LoginSuccess user ->
        setRoute (Board user.DefaultBoardPath)
        { model with User = Some user; Waiting = false; InfoMsg = None }, Cmd.none

    | LoginFailed s ->
        { model with Waiting = false; InfoMsg = Some (Error s) }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [ ClassName "form" ] [
        div [ ClassName "form__header" ] [ str "Welcome back!" ]
        div [ ClassName "login-box" ] [
            messageBox model.InfoMsg

            div [ ClassName "input-block" ] [
                input "username" "text" "Username" model.LoginDto.Username (ChangeUsername >> dispatch) true
            ]

            div [ ClassName "input-block" ] [
                input "password" "password" "Password" model.LoginDto.Password (ChangePassword >> dispatch) false
            ]

            div [ ClassName "input-block--sided" ] [ 
                div [] [
                    checkbox "trusted" (ChangeTrusted >> dispatch) model.LoginDto.Trusted
                    label [ HtmlFor "trusted" ] [ str "Trusted Computer" ]
                ]
                button (fun _ -> dispatch Login) "Log In" loginDisabled
            ]
        ]
    ]
