module Elements

open Fable.React
open Fable.React.Props

let button onClick s disabled = 
    button [
        ClassName "btn btn-primary"
        OnClick onClick
        Disabled disabled
    ] [ str s ]

open Fable.Core.JsInterop

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Router.newUrl href

let viewLink route s =
    a [ 
        Style [ Padding "0 20px" ]
        Href (Router.toPath route)
        OnClick goToUrl
    ] [ str s ]

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
    | Empty
    | Info of string
    | Warning of string
    | Error of string

let getInfoMsgStr =
    function
    | Empty -> None
    | Info s -> Some s
    | Warning s -> Some s
    | Error s -> Some s

let msgBox (infoMsg: InfoMsg) =
    div [
        ClassName "msg-box"
        Style [ Visibility (if infoMsg <> Empty then "visible" else "collapse") ]
    ] [ str (Option.defaultValue "" (getInfoMsgStr infoMsg)) ]
