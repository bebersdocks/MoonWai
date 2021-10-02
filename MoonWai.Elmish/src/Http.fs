module Http

open Fable.Core.JS
open Fable.Core.JsInterop
open Fetch.Types
open Thoth.Json

let inline post<'a, 'b> url (obj: 'a) (handleResponse: Promise<Response> -> 'b) = 
    promise { 
        let body = Encode.Auto.toString<'a>(0, obj)

        let props = [ 
            Method HttpMethod.POST
            Fetch.requestHeaders [ ContentType "application/json" ]
            Body !^body 
        ]

        return handleResponse(Fetch.fetch url props)   
    }
