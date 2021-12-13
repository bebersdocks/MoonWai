module MoonWai.Elmish.Components.MessageBox

open System

open Fable.React
open Fable.React.Props

type InfoMsg =
    | Info of string
    | Warning of string
    | Error of string

let getInfoMsgStr = function
    | Some (Info s) | Some (Warning s) | Some (Error s) -> s
    | None -> String.Empty

let messageBox (infoMsg: InfoMsg option) =
    if Option.isSome infoMsg then
        div [ ClassName "msg-box" ] [ str (getInfoMsgStr infoMsg) ]
    else
        div [] []
