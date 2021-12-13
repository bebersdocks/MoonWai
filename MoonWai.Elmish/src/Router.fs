module MoonWai.Elmish.Router

open Elmish.Navigation
open Elmish.UrlParser

type Route =
    | Catalog
    | Board of string
    | Thread of int 
    | Register
    | Login

let toPath =
    function
    | Catalog -> "/catalog"
    | Board boardPath -> "/" + boardPath
    | Thread threadId -> "/threads/" + (string threadId)
    | Register -> "/register"
    | Login -> "/login"

let route: Parser<Route -> Route, Route> =
    oneOf [
        map Catalog top
        map Catalog (s "catalog")
        map Thread (s "threads" </> i32)
        map Register (s "register")
        map Login (s "login")
        map Board str
    ]

let newUrl newUrl =
    Navigation.newUrl newUrl 
    |> List.map (fun f -> f ignore) 
    |> ignore

let setRoute (route: Route) =
    newUrl (toPath route)
