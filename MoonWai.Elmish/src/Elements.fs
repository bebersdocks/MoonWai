module Elements

open Fable.React
open Fable.React.Props

let button onClick s disabled = 
    button [
        ClassName "btn btn-primary"
        OnClick onClick
        Disabled disabled
    ] [ str s ]

open Elmish.Navigation
open Fable.Core.JsInterop

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()

    let href = !!e.target?href

    Navigation.newUrl href 
    |> List.map (fun f -> f ignore) 
    |> ignore

let viewLink route s =
    a [ 
        Style [ Padding "0 20px" ]
        Href (Router.toPath route)
        OnClick goToUrl
    ] [ str s ]

let input id inputType placeholder defaultValue onChangeEvent autoFocus = 
    Standard.input [
        Id id
        HTMLAttr.Type inputType
        Placeholder placeholder
        DefaultValue defaultValue
        OnChange (fun ev -> onChangeEvent ev.Value)
        AutoComplete "off"
        AutoFocus autoFocus
    ]

let checkbox id onChangeEvent isChecked = 
    Standard.input [
        Id id
        HTMLAttr.Type "checkbox"
        OnChange (fun ev -> onChangeEvent (unbox<bool> ev?target?``checked``))
        Checked isChecked
    ]