module Router

let inline (</>) a b = a + "/" + string b

type Route =
    | Register
    | Login
    | Board of string

let toPath =
    function
    | Register -> "/register"
    | Login -> "/login"
    | Board boardPath -> "/" + boardPath

open Elmish.UrlParser

let route : Parser<Route -> Route, Route> =
    oneOf [ 
        map Register (s "register")
        map Login (s "login") 
        map Board str
    ]

open Elmish.Navigation

let newUrl newUrl =     
    Navigation.newUrl newUrl 
    |> List.map (fun f -> f ignore) 
    |> ignore

let setRoute (route: Route) = 
    newUrl (toPath route)