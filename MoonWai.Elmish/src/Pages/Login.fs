module Pages.Login

open MoonWai.Shared.Auth

open Elements

type Model = {
    LoginDto: LoginDto
    Waiting: bool
    InfoMsg: InfoMsg
}
    
type Msg = 
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess
    | LoginFailed of string

let login (model: Model) = 
    Http.post "/auth/login" model.LoginDto (fun _ -> LoginSuccess) LoginFailed

open Elmish

let init () = 
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      Waiting = false;
      InfoMsg = Empty }, Cmd.none

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

    | LoginSuccess ->
        { model with Waiting = false; InfoMsg = Empty }, Cmd.none

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

        Elements.button (fun _ -> dispatch Login) "Log In" loginDisabled
    ]