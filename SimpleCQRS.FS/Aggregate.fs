module Aggregate

open Messages 

type 'a Root = {
    Id : System.Guid
    UncommittedChanges: 'a Event list }
    with
        member this.ApplyChange isNew event = 
          if isNew then {this with UncommittedChanges = event :: this.UncommittedChanges} else this