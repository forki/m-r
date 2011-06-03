module Repository

open System
open System.Collections.Generic
open Messages

let inline save (storage:EventStore.IEventStore) getId getUncommittedChanges expectedVersion item =
    let id = getId item
    let uncommitedChanges = getUncommittedChanges item |> Seq.map (fun x -> { EventData = x.EventData :> obj; Version = x.Version})
    storage.SaveEvents(id, uncommitedChanges, expectedVersion)

let inline getById (storage:EventStore.IEventStore) init apply id =
    storage.GetEventsForAggregate id
      |> Seq.fold apply (init())

let inline processItem storage init apply getId getUncommittedChanges id processF originalVersion =
    getById storage init apply id
      |> processF
      |> save storage getId getUncommittedChanges originalVersion