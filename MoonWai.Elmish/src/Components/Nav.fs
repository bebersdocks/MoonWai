module MoonWai.Elmish.Components.Nav

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props

open MoonWai.Elmish.Router
open MoonWai.Shared.Models

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    newUrl href

let navLink route s =
    a [
        ClassName "nav-link"
        Href (toPath route)
        OnClick goToUrl
    ] [ str s ]

let navMenu (userSettings: UserSettingsDto option) (boards: BoardDto list) =
    let accountLinks =
        match userSettings with 
        | _ -> 
            div [ ClassName "nav__auth" ] [
                navLink Login "Login"
                navLink Register "Register"
            ]
    
    let boardLinks =
        List.map 
            (fun (x: BoardDto) -> navLink (Board x.Path) (sprintf "/%s/ - %s" x.Path x.Name)) 
            boards

    div [ ClassName "nav" ] (accountLinks :: boardLinks)