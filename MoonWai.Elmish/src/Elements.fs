module MoonWai.Elmish.Elements

open System

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Router
open MoonWai.Shared.Models

let button onClick s disabled = 
    button [
        ClassName "btn btn-primary"
        OnClick onClick
        Disabled disabled
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
