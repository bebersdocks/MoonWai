module Router

let inline (</>) a b = a + "/" + string b

type Route =
    | Home
    | Login

let toPath =
    function
    | Home -> "/home"
    | Login -> "/login"

open Elmish.UrlParser

let route : Parser<Route -> Route, Route> =
    oneOf [ 
        map Home top
        map Home (s "home")
        map Login (s "login") 
    ]