module MoonWai.Elmish.Pages.Register

open System

open Elmish

open Fable.React
open Fable.React.Props

open Thoth.Json

open MoonWai.Elmish.Components.MessageBox
open MoonWai.Elmish.Elements
open MoonWai.Elmish.Http
open MoonWai.Elmish.Router
open MoonWai.Shared.Definitions
open MoonWai.Shared.Models

type Model =
    { RegisterDto: RegisterDto
      User: UserDto option
      PasswordAgain: string option
      InfoMsg: InfoMsg option
      Waiting: bool }

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangePasswordAgain of string
    | Register
    | RegisterSuccess of UserDto
    | RegisterFailed of string

let register (model: Model) =
    let ofSuccess json =
        match Decode.Auto.fromString<UserDto>(json, caseStrategy=CamelCase) with
        | Ok user -> RegisterSuccess user
        | Result.Error e -> RegisterFailed e

    post "api/auth/register" model.RegisterDto ofSuccess RegisterFailed

let init user =
    { RegisterDto = { Username = ""; Password = ""; LanguageId = LanguageId.English };
      User = user;
      PasswordAgain = None;
      InfoMsg = None;
      Waiting = false }, Cmd.none

let update msg model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username ->
        let infoMsg = if String.IsNullOrEmpty(username) then Some (Info "Username can't be empty") else None
        { model with RegisterDto = { model.RegisterDto with Username = username }; InfoMsg = infoMsg }, Cmd.none

    | ChangePassword password ->
        let infoMsg = 
            if password.Length < Common.minPasswordLength && password.Length <> 0 then
                Some (Info (sprintf "Password length can't be less than %i" Common.minPasswordLength))
            else if not (model.RegisterDto.Password.Equals(password)) then 
                Some (Info "Passwords don't match")
            else 
                None

        { model with RegisterDto = { model.RegisterDto with Password = password }; InfoMsg = infoMsg }, Cmd.none

    | ChangePasswordAgain passwordAgain ->
        let infoMsg = 
            if not (model.RegisterDto.Password.Equals(passwordAgain)) then 
                Some (Info "Passwords don't match") else 
            None
        { model with PasswordAgain = Some passwordAgain; InfoMsg = infoMsg }, Cmd.none

    | Register ->
        { model with Waiting = true }, Cmd.OfPromise.result (register model)

    | RegisterSuccess user ->
        setRoute (Board user.DefaultBoardPath)
        { model with User = Some user; Waiting = false; InfoMsg = None }, Cmd.none

    | RegisterFailed s ->
        { model with Waiting = false; InfoMsg = Some (Error s) }, Cmd.none

let view (model: Model) (dispatch: Msg -> unit) =
    let registerDisabled =
        String.IsNullOrEmpty(model.RegisterDto.Username) || 
        String.IsNullOrEmpty(model.RegisterDto.Password) || 
        model.RegisterDto.Password.Length < Common.minPasswordLength ||
        not (model.RegisterDto.Password.Equals(model.PasswordAgain)) ||
        model.Waiting

    div [ ClassName "form" ] [
        div [ ClassName "form__header" ] [ str "Registration" ]
        div [ ClassName "register-box" ] [
            messageBox model.InfoMsg

            div [ ClassName "input-block" ] [
                input "username" "text" "Username" model.RegisterDto.Username (ChangeUsername >> dispatch) true
            ]

            div [ ClassName "input-block" ] [
                input "password" "password" "Password" model.RegisterDto.Password (ChangePassword >> dispatch) false
            ]

            div [ ClassName "input-block" ] [
                input "passwordAgain" "password" "Repeat password" model.PasswordAgain (ChangePasswordAgain >> dispatch) false
            ]

            button (fun _ -> dispatch Register) "Register" registerDisabled
        ]
    ]
