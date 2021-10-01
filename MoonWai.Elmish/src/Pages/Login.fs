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

open Fable.Core.JsInterop
open Fetch.Types
open System
open Thoth.Json

let login (model: Model) = 
    promise { 
        let body = Encode.Auto.toString<LoginDto>(0, model.LoginDto)

        let props = [ 
            Method HttpMethod.POST
            Fetch.requestHeaders [ ContentType "application/json" ]
            Body !^body 
        ]

        let! _ = Fetch.fetch "/auth/login" props   
         
        return LoginSuccess
    }

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

let view (model: Model) (dispatch: Msg -> unit) = 
    let loginDisabled = String.IsNullOrEmpty(model.LoginDto.Username) || String.IsNullOrEmpty(model.LoginDto.Password) || model.Waiting

    div [] [
        h3 [] [ str "Welcome back!" ]

        div [] [
            label [ HtmlFor "username" ] [ str "Username" ]
            Elements.input "username" "text" "Username" model.LoginDto.Username (fun x -> dispatch (ChangeUsername x)) true
        ]

        div [] [
            label [ HtmlFor "password" ] [ str "Password" ]
            Elements.input "password" "password" "Password" model.LoginDto.Password (fun x -> dispatch (ChangePassword x)) false
        ]

        div [] [
            label [ HtmlFor "trusted" ] [ str "Trusted Computer" ]
            checkbox "trusted" (fun x -> dispatch (ChangeTrusted x)) model.LoginDto.Trusted
        ]

        Elements.button (fun _ -> dispatch Login) "Log In" loginDisabled
    ]