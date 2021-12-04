module MoonWai.Elements

open System

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

open MoonWai.Router
open MoonWai.Shared.Models

let button onClick s disabled = 
    button [
        ClassName "btn btn-primary"
        OnClick onClick
        Disabled disabled
    ] [ str s ]

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Router.newUrl href

let link route s =
    a [ 
        Style [ Padding "0 20px" ]
        Href (Router.toPath route)
        OnClick goToUrl
    ] [ str s ]

let navMenu (userSettings: UserSettingsDto option) (boards: BoardDto list) =
    let userBox =
        div [ ClassName "userBox" ] [
            if userSettings.IsNone then
                button (fun _ -> setRoute Login) "Login" false
                button (fun _ -> setRoute Register) "Register" false
            else 
                button ignore "Settings" false // TODO
        ]
    
    let boardLinks =
        List.map 
            (fun (x: BoardDto) -> link (Board x.Page) (sprintf "/%s/ - %s" x.Page x.Name)) 
            boards

    div [ ClassName "navMenu" ] (userBox :: boardLinks)

let input id inputType placeholder value onChangeEvent autoFocus = 
    Standard.input [
        Id id
        Type inputType
        Placeholder placeholder
        Value value
        OnChange (fun ev -> onChangeEvent ev.Value)
        AutoComplete "off"
        AutoFocus autoFocus
    ]

let checkbox id onChangeEvent isChecked = 
    Standard.input [
        Id id
        Type "checkbox"
        OnChange (fun ev -> onChangeEvent (unbox<bool> ev?target?``checked``))
        Checked isChecked
    ]

type InfoMsg =
    | Info of string
    | Warning of string
    | Error of string

let getInfoMsgStr = function
    | Some (Info s) | Some (Warning s) | Some (Error s) -> s
    | None -> String.Empty

let msgBox (infoMsg: InfoMsg option) =
    let visibility = 
        if Option.isSome infoMsg then "visible" 
        else "collapse"

    div [
        ClassName "msgBox"
        Style [ Visibility visibility ]
    ] [ str (getInfoMsgStr infoMsg) ]
