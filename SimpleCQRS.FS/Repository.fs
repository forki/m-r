module Repository

open System
open System.Collections.Generic
open Messages

let inline save (storage:EventStore.IEventStore) expectedVersion item =
    let root = (^a : (member AggregateRoot :  Aggregate.Root<_>) item)    
    let uncommitedChanges = root.UncommittedChanges |> Seq.map (fun x -> { EventData = x.EventData :> obj; Version = x.Version})
    storage.SaveEvents(root.Id, uncommitedChanges, expectedVersion)

let inline getById (storage:EventStore.IEventStore) init apply id =
    storage.GetEventsForAggregate id
      |> Seq.fold apply (init())

let inline processItem storage init apply id processF originalVersion =
    getById storage init apply id
      |> processF
      |> save storage originalVersion