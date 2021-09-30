module Pages.Home

type Model = {
    nonStr: string
}
    
type Msg = 
    | None

open Elmish
open Fable.React

let init () = 
    { nonStr = "" }, Cmd.ofMsg None

let update (msg: Msg) model : Model*Cmd<Msg> = 
    (model, Cmd.none)

let view (model: Model) (dispatch: Msg -> unit) = 
    div [] [ h3 [] [ str "Hello!"] ]