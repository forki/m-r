module Repository

open System
open System.Collections.Generic
open EventStore

let inline save (storage:IEventStore) expectedVersion item =
    let root = (^a : (member GetAggregateRoot : unit -> Aggregate.Root<_>) item)    
    let uncommitedChanges = root.UncommittedChanges |> Seq.map downcastEvent
    storage.SaveEvents root.Id uncommitedChanges expectedVersion

let inline getById (storage:IEventStore) init apply id =
    storage.GetEventsForAggregate id
      |> Seq.map upcastEvent
      |> Seq.fold apply (init())

let inline processItem storage init apply id processF originalVersion =
    getById storage init apply id
      |> processF
      |> save storage originalVersion
