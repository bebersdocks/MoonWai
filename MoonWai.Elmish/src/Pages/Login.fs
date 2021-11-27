module Pages.Login

open System

open Elements

open Elmish

open Fable.React
open Fable.React.Props

open MoonWai.Shared.Models

open Router

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

    Http.post "/auth/login" model.LoginDto ofSuccess LoginFailed

let init userSettings =
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      UserSettings = userSettings
      InfoMsg = None;
      Waiting = false; }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username ->
        { model with LoginDto = { model.LoginDto with Username = username }; InfoMsg = None }, Cmd.none

    | ChangePassword password ->
        { model with LoginDto = { model.LoginDto with Password = password }; InfoMsg = None }, Cmd.none

    | ChangeTrusted trusted ->
        { model with LoginDto = { model.LoginDto with Trusted = trusted }; InfoMsg = None }, Cmd.none

    | Login _ ->
        { model with Waiting = true; InfoMsg = None }, Cmd.OfPromise.result (login model)

    | LoginSuccess userSettings ->
        setRoute (Board userSettings.DefaultBoardPath)
        { model with UserSettings = Some userSettings; Waiting = false; InfoMsg = None }, Cmd.none

    | LoginFailed s ->
        { model with Waiting = false; InfoMsg = Some (Error s) }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [ ClassName "form" ] [
        h3 [ ClassName "formHeader" ] [ str "Welcome back!" ]
        div [ ClassName "loginBox" ] [
            msgBox model.InfoMsg

            div [] [
                Elements.input "username" "text" "Username" model.LoginDto.Username (ChangeUsername >> dispatch) true
            ]

            div [] [
                Elements.input "password" "password" "Password" model.LoginDto.Password (ChangePassword >> dispatch) false
            ]

            div [] [
                label [ HtmlFor "trusted" ] [ 
                    checkbox "trusted" (ChangeTrusted >> dispatch) model.LoginDto.Trusted
                    str "Trusted Computer" 
                ]

                Elements.button (fun _ -> dispatch Login) "Log In" loginDisabled
            ]
        ]
    ]
