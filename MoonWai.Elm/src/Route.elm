module Route exposing (Route(..), fromUrl, href, replaceUrl)


import Browser.Navigation as Nav
import Html exposing (Attribute)
import Html.Attributes as Attr
import Url exposing (Url)
import Url.Parser as Parser exposing ((</>), Parser, oneOf, s)


type Route
    = Home
    | Login


parser : Parser (Route -> a) a
parser =
    oneOf
        [ Parser.map Home Parser.top
        , Parser.map Login (s "login")
        ]


href : Route -> Attribute msg
href targetRoute = 
    Attr.href (routeToString targetRoute)


replaceUrl : Nav.Key -> Route -> Cmd msg
replaceUrl key route = 
    Nav.replaceUrl key (routeToString route)


fromUrl : Url -> Maybe Route
fromUrl url = 
    url 
        |> Parser.parse parser


routeToString : Route -> String
routeToString page = 
    "/" ++ String.join "/" (routeToPieces page)


routeToPieces : Route -> List String
routeToPieces page =
    case page of
        Home -> 
            []

        Login -> 
            [ "login" ]