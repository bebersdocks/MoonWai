module Page.Home exposing (..)


import Html exposing (Html, span, text)
import Html.Attributes exposing (class)
import Session exposing (Session)


type alias Model =
    { session : Session
    }


init : Session -> ( Model, Cmd msg )
init session =
    ( { session = session }
    , Cmd.none
    )


view : Html msg
view =
    span [ class "welcome-message" ] [ text "Hello, World!" ]

type Msg 
    = None