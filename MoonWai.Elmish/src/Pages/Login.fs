module Pages.Login

type Model = {
    Username: string
    Password: string
    Waiting: bool
}
    
type Msg = 
    | ChangeUsername of string
    | ChangePassword of string
    | Login
    | LoginSuccess
    | LoginFailed of exn

open Elmish
open Fable.Core.JsInterop
open Fetch.Types
open Thoth.Json

let login (model: Model) = 
    promise { 
        let body = Encode.Auto.toString(0, model)

        let props = [ 
            Method HttpMethod.POST
            Fetch.requestHeaders [ ContentType "application/x-www-form-urlencoded" ]
            Body !^body 
        ]

        let! _ = Fetch.fetch "/login" props   
         
        return LoginSuccess
    }

open Elmish

let init () = 
    { Username = ""; 
      Password = ""; 
      Waiting = false }, Cmd.none

let update (msg: Msg) model : Model * Cmd<Msg> = 
    match msg with
    | ChangeUsername username ->
        { model with Username = username }, Cmd.none

    | ChangePassword password ->
        { model with Password = password }, Cmd.none

    | Login _ ->
        { model with Waiting = true }, Cmd.OfPromise.either login model (fun _ -> LoginSuccess) LoginFailed

    | LoginSuccess ->
        { model with Waiting = false}, Cmd.none

    | LoginFailed _ -> 
        { model with Waiting = false}, Cmd.none

open Elements
open Fable.React
open System

let view (model: Model) (dispatch: Msg -> unit) = 
    let loginDisabled = String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password) || model.Waiting

    div [] [
        h3 [] [ str "Welcome back!" ]

        inputDiv "username" "text" "Username" "Username" model.Username (fun x -> dispatch (ChangeUsername x)) true
        inputDiv "password" "password" "Password" "Password" model.Password (fun x -> dispatch (ChangePassword x)) false

        buttonDiv (dispatch Login) "Log In" loginDisabled
    ]