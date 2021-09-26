module Main exposing (main)

import Browser exposing (Document)
import Browser.Navigation as Nav
import Json.Decode exposing (Value)
import Html exposing (..)
import Page
import Page.Home
import Page.Auth.Login
import Session exposing (Session(..), navKey)
import Route exposing (Route)
import Url exposing (Url)


type Model
    = Home Page.Home.Model
    | Login Page.Auth.Login.Model


init : Url -> Nav.Key -> ( Model, Cmd Msg )
init url navKey =
    let  
        (home, _) = 
            Page.Home.init (Anonymous (navKey))
    in
    changeRouteTo (Route.fromUrl url) (Home home)


view : Model -> Document Msg
view model =
    let
        viewPage page toMsg config =
            let
                { title, body } =
                    Page.view page config
            in
            { title = title
            , body = List.map (Html.map toMsg) body
            }
    in
    case model of
        Home _ -> 
            viewPage Page.Home HomeMsg Page.Home.view

        Login login ->
            viewPage Page.Login LoginMsg (Page.Auth.Login.view login)


type Msg
    = HomeMsg Page.Home.Msg
    | LoginMsg Page.Auth.Login.Msg
    | UrlChanged Url
    | UrlRequested Browser.UrlRequest


toSession : Model -> Session
toSession page =
    case page of
        Home home ->
            home.session

        Login login ->
            login.session


changeRouteTo : Maybe Route -> Model -> ( Model, Cmd Msg )
changeRouteTo maybeRoute model =
    let
        session =
            toSession model
    in
    case maybeRoute of
        Nothing -> 
            ( model, Cmd.none)

        Just Route.Home -> 
            ( model, Cmd.none)

        Just Route.Login ->
            Page.Auth.Login.init session
                |> updateWith Login LoginMsg model



update : Msg -> Model -> ( Model, Cmd Msg )
update msg model =
    case ( msg, model ) of
        ( HomeMsg _, Home _ ) -> 
            ( model, Cmd.none )

        ( LoginMsg subMsg, Login login ) ->
            Page.Auth.Login.update subMsg login
                |> updateWith Login LoginMsg model

        ( UrlRequested urlRequest, _ ) ->
            case urlRequest of
                Browser.Internal url ->
                    ( model
                    , Nav.pushUrl (Session.navKey (toSession model)) (Url.toString url)
                    )
                Browser.External href ->
                    ( model
                    , Nav.load href
                    )

        ( UrlChanged newUrl, _ ) ->
            changeRouteTo (Route.fromUrl newUrl) model

        ( _, _ ) ->
            ( model, Cmd.none )


updateWith : (subModel -> Model) -> (subMsg -> Msg) -> Model -> ( subModel, Cmd subMsg ) -> ( Model, Cmd Msg )
updateWith toModel toMsg model ( subModel, subCmd ) =
    ( toModel subModel
    , Cmd.map toMsg subCmd
    )


main : Program Value Model Msg
main =
    Debug.log "main"
    Browser.application
        { init = \_ url navKey -> init url navKey
        , onUrlChange =  UrlChanged
        , onUrlRequest = UrlRequested
        , view = view
        , update = update
        , subscriptions = \_ -> Sub.none
        }