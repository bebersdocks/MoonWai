module Pages.Register

open System

open Elements

open Elmish

open Fable.React
open Fable.React.Props

open Router

open Thoth.Json

open MoonWai.Shared.Definitions
open MoonWai.Shared.Models

type Model = 
    { RegisterDto: RegisterDto
      UserSettings: UserSettingsDto option
      PasswordAgain: string option
      InfoMsg: InfoMsg
      Waiting: bool }

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangePasswordAgain of string
    | Register
    | RegisterSuccess of UserSettingsDto
    | RegisterFailed of string

let register (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<UserSettingsDto>(json, caseStrategy=CamelCase) with
        | Ok userSettings -> RegisterSuccess userSettings
        | Result.Error e -> RegisterFailed e

    Http.post "/auth/register" model.RegisterDto ofSuccess RegisterFailed

let init userSettings =
    { RegisterDto = { Username = ""; Password = ""; LanguageId = LanguageId.English };
      UserSettings = userSettings;
      PasswordAgain = None;
      InfoMsg = EmptyMsg;
      Waiting = false }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username when String.IsNullOrEmpty(username) ->
        { model with 
            RegisterDto = { model.RegisterDto with Username = username }; 
            InfoMsg = Info "Username can't be empty" }, Cmd.none

    | ChangeUsername username ->
        { model with RegisterDto = { model.RegisterDto with Username = username }; InfoMsg = EmptyMsg }, Cmd.none

    | ChangePassword password when 
        password.Length < Common.minPasswordLength && password.Length <> 0 ->
        { model with
            RegisterDto = { model.RegisterDto with Password = password };
            InfoMsg = Info (sprintf "Password length can't be less than %i" Common.minPasswordLength) }, Cmd.none

    | ChangePassword password when not (model.RegisterDto.Password.Equals(password)) ->
        { model with RegisterDto = { model.RegisterDto with Password = password }; InfoMsg = Info "Passwords don't match" }, Cmd.none

    | ChangePassword password ->
        { model with RegisterDto = { model.RegisterDto with Password = password }; InfoMsg = EmptyMsg }, Cmd.none

    | ChangePasswordAgain passwordAgain when not (model.RegisterDto.Password.Equals(passwordAgain)) ->
        { model with PasswordAgain = Some passwordAgain; InfoMsg = Info "Passwords don't match" }, Cmd.none

    | ChangePasswordAgain passwordAgain ->
        { model with PasswordAgain = Some passwordAgain; InfoMsg = EmptyMsg }, Cmd.none

    | Register ->
        { model with Waiting = true; InfoMsg = EmptyMsg }, Cmd.OfPromise.result (register model)

    | RegisterSuccess userSettings ->
        setRoute (Board userSettings.DefaultBoardPath)
        { model with UserSettings = Some userSettings; Waiting = false; InfoMsg = EmptyMsg }, Cmd.none

    | RegisterFailed s ->
        { model with Waiting = false; InfoMsg = Error s }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let registerDisabled =
        String.IsNullOrEmpty(model.RegisterDto.Username) || 
        String.IsNullOrEmpty(model.RegisterDto.Password) || 
        model.RegisterDto.Password.Length < Common.minPasswordLength ||
        not (model.RegisterDto.Password.Equals(model.PasswordAgain)) ||
        model.Waiting

    div [ ClassName "form" ] [
        h3 [] [ str "Registration" ]
        div [ ClassName "registerBox" ] [
               
            msgBox model.InfoMsg

            div [] [
                Elements.input "username" "text" "Username" model.RegisterDto.Username (ChangeUsername >> dispatch) true
            ]

            div [] [
                Elements.input "password" "password" "Password" model.RegisterDto.Password (ChangePassword >> dispatch) false
            ]

            div [] [
                Elements.input "passwordAgain" "password" "Repeat password" model.PasswordAgain (ChangePasswordAgain >> dispatch) false
            ]

            Elements.button (fun _ -> dispatch Register) "Register" registerDisabled
        ]
    ]