module Page exposing (Page(..), view)


import Browser exposing (Document)
import Html exposing (Html, a, div, footer, li, nav, text, ul)
import Html.Attributes exposing (class, classList)
import Route exposing (Route)


type Page
    = Home
    | Login


view : Page ->  Html msg -> Document msg
view page content =
    { title = "MoonWai"
    , body = [ viewHeader page, content, viewFooter ]
    }


viewHeader : Page -> Html msg
viewHeader page =
    nav [ class "navbar navbar-light" ]
        [ div [ class "container" ]
            [ a [ class "navbar-brand", Route.href Route.Home ]
                [ text "conduit" ]
            , ul [ class "nav navbar-nav pull-xs-right" ] <|
                navbarLink page Route.Home [ text "Home" ]
                    :: viewMenu page
            ]
        ]


viewMenu : Page -> List (Html msg)
viewMenu page =
    let
        linkTo =
            navbarLink page
    in
    [ linkTo Route.Login [ text "Login" ]
    ]


viewFooter : Html msg
viewFooter =
    footer []
        [ div [ class "container" ]
            [ text "footer"
            ]
        ]


navbarLink : Page -> Route -> List (Html msg) -> Html msg
navbarLink page route linkContent =
    li [ classList [ ( "nav-item", True ), ( "active", isActive page route ) ] ]
        [ a [ class "nav-link", Route.href route ] linkContent ]


isActive : Page -> Route -> Bool
isActive page route =
    case ( page, route ) of
        ( Home, Route.Home) ->
            True

        ( Login, Route.Login ) ->
            True

        _ ->
            False