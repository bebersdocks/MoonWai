module Page.Auth.Login exposing (..)


import Json.Decode as D
import Html exposing (Html, button, div, input, label, text)
import Html.Attributes exposing (checked, class, type_, placeholder, required, value)
import Html.Events exposing (onClick, onCheck, onInput)
import Http
import Session exposing (Session)
import Page exposing (Page(..))


type alias Model =
    { session : Session
    , username : String 
    , password : String 
    , trusted : Bool
    }


init : Session -> ( Model, Cmd msg )
init session =
    ( { session = session
      , username = ""
      , password = ""
      , trusted = False
      }
    , Cmd.none
    )


view : Model -> Html Msg
view model = 
    div [ class "container" ]
        [ viewInput "text" "Username" True model.username Username
        , viewInput "password" "Password" True model.password Password
        , label [] 
            [ text "Trusted Computer"
            , input [ type_ "checkbox", required False, checked model.trusted, onCheck Trusted ] []
            ]
        , button [ onClick LoginRequest ] 
            [ text "Login" ] 
        ]


viewInput : String -> String -> Bool -> String -> (String -> msg) -> Html msg
viewInput t p r v toMsg =
    input [ type_ t, placeholder p, required r, value v, onInput toMsg] []


type Msg 
    = LoginRequest 
    | LoginResponse (Result Http.Error String)
    | Username String
    | Password String
    | Trusted Bool
 


login : Model -> Cmd Msg 
login model = 
    Http.post
        { url = "auth/login?username=" ++ model.username ++ "&password=" ++ model.password ++ "&trusted=" ++ (if model.trusted then "true" else "false")
        , expect = Http.expectJson LoginResponse (D.string)
        , body = Http.emptyBody
        }


update : Msg -> Model -> ( Model, Cmd Msg )
update msg model = 
    case msg of 
        LoginRequest -> 
            ( model, login model )

        LoginResponse _ -> 
            ( model, Cmd.none )

        Username username -> 
            ( { model | username = username }, Cmd.none )
        
        Password password -> 
            ( { model | password = password }, Cmd.none )

        Trusted trusted -> 
            ( { model | trusted = trusted }, Cmd.none )


toSession : Model -> Session
toSession model =
    model.session