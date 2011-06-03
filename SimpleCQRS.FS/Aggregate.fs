module Aggregate

open System
open Messages 

type 'a Root() =
    let mutable id = Guid.Empty
    let changes = new System.Collections.Generic.List<'a Event>()

    member this.Id
        with get () = id
        and set value = id <- value
    
    member this.ApplyChange isNew event = if isNew then changes.Add event
    member this.GetUncommittedChanges() = changes |> Seq.map (fun x -> { EventData = x.EventData :> obj; Version = x.Version})