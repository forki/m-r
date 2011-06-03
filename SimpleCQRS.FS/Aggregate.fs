module Aggregate

open Messages 

type 'a Root() =
    let changes = new System.Collections.Generic.List<'a Event>()

    member this.ApplyChange isNew event = if isNew then changes.Add event
    member this.GetUncommittedChanges() = changes |> Seq.map (fun x -> { EventData = x.EventData :> obj; Version = x.Version})