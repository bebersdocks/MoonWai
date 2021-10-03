module Pages.Register

open Elements
open MoonWai.Shared.Models

type Model = {
    RegisterDto: RegisterDto
    PasswordAgain: string option
    InfoMsg: InfoMsg
    Waiting: bool
}

open Router

type Msg =
    | ChangeUsername of string
    | ChangePassword of string
    | ChangePasswordAgain of string
    | Register
    | RegisterSuccess
    | RegisterFailed of string
    | GoTo of Route

let register (model: Model) =
    Http.post "/auth/register" model.RegisterDto (fun _ -> RegisterSuccess) RegisterFailed

open Elmish
open MoonWai.Shared.Definitions

let init () =
    { RegisterDto = { Username = ""; Password = ""; LanguageId = LanguageId.English };
      PasswordAgain = None;
      InfoMsg = Empty;
      Waiting = false }, Cmd.none

open System

let update (msg: Msg) model : Model * Cmd<Msg> =
    match msg with
    | ChangeUsername username when String.IsNullOrEmpty(username) ->
        { model with 
            RegisterDto = { model.RegisterDto with Username = username }; 
            InfoMsg = Info "Username can't be empty" }, Cmd.none

    | ChangeUsername username ->
        { model with RegisterDto = { model.RegisterDto with Username = username }; InfoMsg = Empty }, Cmd.none

    | ChangePassword password when password.Length < Constants.MIN_PASSWORD_LENGTH ->
        { model with
            RegisterDto = { model.RegisterDto with Password = password };
            InfoMsg = Info (sprintf "Password length can't be less than %i" Constants.MIN_PASSWORD_LENGTH) }, Cmd.none

    | ChangePassword password when not (model.RegisterDto.Password.Equals(password)) ->
        { model with RegisterDto = { model.RegisterDto with Password = password }; InfoMsg = Info "Passwords don't match" }, Cmd.none

    | ChangePassword password ->
        { model with RegisterDto = { model.RegisterDto with Password = password }; InfoMsg = Empty }, Cmd.none

    | ChangePasswordAgain passwordAgain when not (model.RegisterDto.Password.Equals(passwordAgain)) ->
        { model with PasswordAgain = Some passwordAgain; InfoMsg = Info "Passwords don't match" }, Cmd.none

    | ChangePasswordAgain passwordAgain ->
        { model with PasswordAgain = Some passwordAgain; InfoMsg = Empty }, Cmd.none

    | Register ->
        { model with Waiting = true; InfoMsg = Empty }, Cmd.OfPromise.result (register model)

    | RegisterSuccess ->
        { model with Waiting = false; InfoMsg = Empty }, Cmd.ofMsg (GoTo Home)

    | RegisterFailed s ->
        { model with Waiting = false; InfoMsg = Error s }, Cmd.none

    | GoTo route ->
        setRoute route
        model, Cmd.none

open Fable.React
open Fable.React.Props

let view (model: Model) (dispatch: Msg -> unit) =
    let registerDisabled =
        String.IsNullOrEmpty(model.RegisterDto.Username) || 
        String.IsNullOrEmpty(model.RegisterDto.Password) || 
        model.RegisterDto.Password.Length < Constants.MIN_PASSWORD_LENGTH ||
        not (model.RegisterDto.Password.Equals(model.PasswordAgain)) ||
        model.Waiting

    div [] [
        h3 [] [ str "Registration" ]
           
        msgBox model.InfoMsg

        div [] [
            label [ HtmlFor "username" ] [ str "Username" ]
            Elements.input "username" "text" "Username" model.RegisterDto.Username (ChangeUsername >> dispatch) true
        ]

        div [] [
            label [ HtmlFor "password" ] [ str "Password" ]
            Elements.input "password" "password" "Password" model.RegisterDto.Password (ChangePassword >> dispatch) false
        ]

        div [] [
            label [ HtmlFor "passwordAgain" ] [ str "Repeat Password" ]
            Elements.input "passwordAgain" "passwordAgain" "Repeat password" model.PasswordAgain (ChangePasswordAgain >> dispatch) false
        ]

        Elements.button (fun _ -> dispatch Register) "Register" registerDisabled
    ]