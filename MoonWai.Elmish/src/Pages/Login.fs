module Pages.Login

open Elements
open MoonWai.Shared.Models

type Model = {
    LoginDto: LoginDto
    UserSettings: UserSettingsDto option
    InfoMsg: InfoMsg
    Waiting: bool
}

open Router

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess of UserSettingsDto
    | LoginFailed of string

open Thoth.Json

let login (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<UserSettingsDto>(json, caseStrategy=CamelCase) with
        | Ok userSettings -> LoginSuccess userSettings
        | Result.Error e -> LoginFailed e

    Http.post "/auth/login" model.LoginDto ofSuccess LoginFailed

open Elmish

let init userSettings =
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      UserSettings = userSettings
      InfoMsg = Empty;
      Waiting = false; }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username ->
        { model with LoginDto = { model.LoginDto with Username = username }; InfoMsg = Empty }, Cmd.none

    | ChangePassword password ->
        { model with LoginDto = { model.LoginDto with Password = password }; InfoMsg = Empty }, Cmd.none

    | ChangeTrusted trusted ->
        { model with LoginDto = { model.LoginDto with Trusted = trusted }; InfoMsg = Empty }, Cmd.none

    | Login _ ->
        { model with Waiting = true; InfoMsg = Empty }, Cmd.OfPromise.result (login model)

    | LoginSuccess userSettings ->
        setRoute (Board userSettings.DefaultBoardPath)
        { model with UserSettings = Some userSettings; Waiting = false; InfoMsg = Empty }, Cmd.none

    | LoginFailed s ->
        { model with Waiting = false; InfoMsg = Error s }, Cmd.none

open System
open Fable.React
open Fable.React.Props

let view (model: Model) (dispatch: Msg -> unit) =
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [] [
        h3 [] [ str "Welcome back!" ]
        
        msgBox model.InfoMsg

        div [] [
            label [ HtmlFor "username" ] [ str "Username" ]
            Elements.input "username" "text" "Username" model.LoginDto.Username (ChangeUsername >> dispatch) true
        ]

        div [] [
            label [ HtmlFor "password" ] [ str "Password" ]
            Elements.input "password" "password" "Password" model.LoginDto.Password (ChangePassword >> dispatch) false
        ]

        div [] [
            label [ HtmlFor "trusted" ] [ str "Trusted Computer" ]
            checkbox "trusted" (ChangeTrusted >> dispatch) model.LoginDto.Trusted
        ]

        div [] [
            Elements.button (fun _ -> dispatch Login) "Log In" loginDisabled
            Elements.button (fun _ -> setRoute Register) "Register" model.Waiting
        ]
    ]