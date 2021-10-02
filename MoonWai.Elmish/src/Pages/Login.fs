module Pages.Login

open MoonWai.Shared.Auth

type Model = {
    LoginDto: LoginDto
    Waiting: bool
    ErrorMessage: string option
}
    
type Msg = 
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess
    | LoginFailed of string option

let login (model: Model) = 
    Http.post "/auth/login" model.LoginDto (fun _ -> LoginSuccess) (Some >> LoginFailed)

open Elmish

let init () = 
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      Waiting = false;
      ErrorMessage = None }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> = 
    match msg with
    | ChangeUsername username ->
        { model with LoginDto = { model.LoginDto with Username = username } }, Cmd.none

    | ChangePassword password ->
        { model with LoginDto = { model.LoginDto with Password = password } }, Cmd.none

    | ChangeTrusted trusted -> 
        { model with LoginDto = { model.LoginDto with Trusted = trusted } }, Cmd.none

    | Login _ ->
        { model with Waiting = true }, Cmd.OfPromise.result (login model)

    | LoginSuccess ->
        { model with Waiting = false}, Cmd.none

    | LoginFailed s -> 
        { model with Waiting = false; ErrorMessage = s }, Cmd.none

open Elements
open Fable.React
open Fable.React.Props
open System

let view (model: Model) (dispatch: Msg -> unit) = 
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [] [
        h3 [] [ str "Welcome back!" ]

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