module Router

open Elmish.Navigation
open Elmish.UrlParser

let inline (</>) a b = a + "/" + string b

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
    | Register -> "/register"
    | Login -> "/login"

let route : Parser<Route -> Route, Route> =
    oneOf [
        map Catalog (s "catalog")
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