module Router

let inline (</>) a b = a + "/" + string b

type Route =
    | Home
    | Register
    | Login

let toPath =
    function
    | Home -> "/home"
    | Register -> "/register"
    | Login -> "/login"

open Elmish.UrlParser

let route : Parser<Route -> Route, Route> =
    oneOf [ 
        map Home top
        map Home (s "home")
        map Register (s "register")
        map Login (s "login") 
    ]

open Elmish.Navigation

let newUrl newUrl =     
    Navigation.newUrl newUrl 
    |> List.map (fun f -> f ignore) 
    |> ignore

let setRoute (route: Route) = 
    newUrl (toPath route)