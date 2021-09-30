module Elements

open Fable.React
open Fable.React.Props

let button onClick s disabled = 
    button [
        ClassName "btn btn-primary"
        OnClick (fun _ -> onClick) 
        Disabled disabled
    ] [ str s ]

let buttonDiv onClick s disabled = 
    div [] [ button onClick s disabled ]

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

let label parentId s = 
    label [
        HtmlFor parentId
    ] [ str s ]

let input id inputType placeholder defaultValue onChangeEvent autoFocus = 
    input [
        Id id
        HTMLAttr.Type inputType
        Placeholder placeholder
        DefaultValue defaultValue
        OnChange (fun ev -> onChangeEvent ev.Value)
        AutoFocus autoFocus
    ]

let inputDiv id inputType placeholder labelText defaultValue onChangeEvent autoFocus =
    div [ ClassName "input-div" ] [
        label id labelText
        input id inputType placeholder defaultValue onChangeEvent autoFocus
    ]