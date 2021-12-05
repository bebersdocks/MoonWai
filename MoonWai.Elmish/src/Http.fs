module MoonWai.Elmish.Http

open Fable.Core.JsInterop

open Fetch

open MoonWai.Shared.Models

open Thoth.Json

let inline private request<'b> url props (ofSuccess: string -> 'b) (ofFailure: string -> 'b) =
    promise {
        try
            let! resp = fetchUnsafe url props
            let! txt = resp.text()

            if resp.Ok then
                return ofSuccess txt
            else
                let errorMsg =
                    match Decode.Auto.fromString<ErrorDto>(txt, caseStrategy=CamelCase) with
                    | Ok errorObj -> errorObj.Message
                    | Error _ -> (sprintf "Request to API failed with [%i]" resp.Status)
                return ofFailure errorMsg
        with
        | exn -> return ofFailure exn.Message
    }

let inline get<'b> url (ofSuccess: string -> 'b) (ofFailure: string -> 'b) =
    request url [ Method HttpMethod.GET ] ofSuccess ofFailure

let inline post<'a, 'b> url (obj: 'a) (ofSuccess: string -> 'b) (ofFailure: string -> 'b) =
    let body = Encode.Auto.toString<'a>(0, obj)

    let props = [ 
        Method HttpMethod.POST
        requestHeaders [ ContentType "application/json" ]
        Body !^body 
    ]

    request url props ofSuccess ofFailure
