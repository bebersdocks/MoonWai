module Http

open Fable.Core.JsInterop
open Fetch
open Thoth.Json

let inline post<'a, 'b> url (obj: 'a) (ofSuccess: string -> 'b) (ofFailure: string -> 'b) = 
    promise { 
        let body = Encode.Auto.toString<'a>(0, obj)

        let props = [ 
            Method HttpMethod.POST
            requestHeaders [ ContentType "application/json" ]
            Body !^body 
        ]

        try
            let! resp = fetch url props
            let! txt = resp.text()

            if resp.Ok then 
                return ofSuccess txt 
            else if resp.Status = 400 || resp.Status = 404 then 
                return ofFailure txt
            else 
                return ofFailure (sprintf "Request to API failed with status code %i" resp.Status)
        with 
        | exn -> return ofFailure exn.Message
    }
