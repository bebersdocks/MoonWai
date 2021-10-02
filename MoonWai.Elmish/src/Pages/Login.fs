module Pages.Login

open MoonWai.Shared.Auth

type Model = {
    LoginDto: LoginDto
    Waiting: bool
    ErrMsg: string
}
    
type Msg = 
    | ChangeUsername of string
    | ChangePassword of string
    | ChangeTrusted of bool
    | Login
    | LoginSuccess
    | LoginFailed of exn

let login (model: Model) = 
    Http.post "/auth/login" model.LoginDto (fun _ -> LoginSuccess)

open Elmish

let init () = 
    { LoginDto = { Username = ""; Password = ""; Trusted = false };
      Waiting = false;
      ErrMsg = "" }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> = 
    match msg with
    | ChangeUsername username ->
        { model with LoginDto = { model.LoginDto with Username = username } }, Cmd.none

    | ChangePassword password ->
        { model with LoginDto = { model.LoginDto with Password = password } }, Cmd.none

    | ChangeTrusted trusted -> 
        { model with LoginDto = { model.LoginDto with Trusted = trusted } }, Cmd.none

    | Login _ ->
        Browser.Dom.console.log "promise"
        { model with Waiting = true }, Cmd.OfPromise.either login model (fun _ -> LoginSuccess) LoginFailed

    | LoginSuccess ->
        { model with Waiting = false}, Cmd.none

    | LoginFailed exn -> 
        { model with Waiting = false; ErrMsg = exn.Message }, Cmd.none

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