module Aggregate

type 'a Root() =
    let changes = new System.Collections.Generic.List<'a>()

    member this.ApplyChange isNew event = if isNew then changes.Add event
    member this.GetUncommittedChanges() = changes