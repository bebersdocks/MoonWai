module MoonWai.Elmish.Pages.Login

open System

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Elements
open MoonWai.Elmish.Http
open MoonWai.Elmish.Router
open MoonWai.Shared.Models

open Thoth.Json

type Model =
    { LoginDto: LoginDto
      UserSettings: UserSettingsDto option
      InfoMsg: InfoMsg option
      Waiting: bool }

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess of UserSettingsDto
    | LoginFailed of string

let login (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<UserSettingsDto>(json, caseStrategy=CamelCase) with
        | Ok userSettings -> LoginSuccess userSettings
        | Result.Error e -> LoginFailed e

    post "api/auth/login" model.LoginDto ofSuccess LoginFailed

let init userSettings =
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      UserSettings = userSettings
      InfoMsg = None;
      Waiting = false; }, Cmd.none

let update msg model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username ->
        let infoMsg = if String.IsNullOrEmpty(username) then Some (Info "Username can't be empty") else None
        { model with LoginDto = { model.LoginDto with Username = username }; InfoMsg = infoMsg }, Cmd.none

    | ChangePassword password ->
        let infoMsg = if String.IsNullOrEmpty(password) then Some (Info "Password can't be empty") else None
        { model with LoginDto = { model.LoginDto with Password = password }; InfoMsg = infoMsg }, Cmd.none

    | ChangeTrusted trusted ->
        { model with LoginDto = { model.LoginDto with Trusted = trusted } }, Cmd.none

    | Login ->
        { model with Waiting = true }, Cmd.OfPromise.result (login model)

    | LoginSuccess userSettings ->
        setRoute (Board userSettings.DefaultBoardPath)
        { model with UserSettings = Some userSettings; Waiting = false; InfoMsg = None }, Cmd.none

    | LoginFailed s ->
        { model with Waiting = false; InfoMsg = Some (Error s) }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [ ClassName "form" ] [
        div [ ClassName "form__header" ] [ str "Welcome back!" ]
        div [ ClassName "login-box" ] [
            msgBox model.InfoMsg

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
