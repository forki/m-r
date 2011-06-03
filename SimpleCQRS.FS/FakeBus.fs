module FakeBus

open System
open System.Collections.Generic
open Messages
open System.Threading

type FakeBus() =
    let commandRoutes = new Dictionary<_,_>()
    let eventRoutes = new Dictionary<_,_>()

    member this.RegisterCommandHandler (handler:'a Command -> unit)  =
        let key = typeof<'a>
        match commandRoutes.TryGetValue key with
        | true,handlers -> commandRoutes.[key] <- (handler :> obj)::handlers
        | _ -> commandRoutes.Add(key,[handler])

    member this.RegisterEventHandler (handler:'a Event-> unit) =
        let key = typeof<'a>
        let action (x:obj) = 
            let genericEvent = x :?> obj Event
            let event = { EventData = genericEvent.EventData :?> 'a; Version = genericEvent.Version } : 'a Event
            handler event

        match eventRoutes.TryGetValue key with
        | true,handlers -> eventRoutes.[key] <- action::handlers
        | _ -> eventRoutes.Add(key,[action])

    member this.Send (command:'a Command) =
        let key = typeof<'a>
        match commandRoutes.TryGetValue key with
        | true,handler::[] -> (handler :?> 'a Command -> unit) command 
        | true,handler::handlers -> failwith "cannot send to more than one handler"
        | _ -> failwith "no handler registered"

    member this.Publish (event:obj Event) =
        let rec publishAs key =                                                
            match eventRoutes.TryGetValue key with
            | true,handlers ->
                handlers 
                   |> List.iter (fun handler -> handler (event :> obj))  // Greg did this with Threadpool
            | _ -> if key.BaseType <> null then publishAs key.BaseType

        event.EventData.GetType() |> publishAs