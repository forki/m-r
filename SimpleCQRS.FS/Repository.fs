module Repository

open System
open System.Collections.Generic
open Messages

let inline save (storage:EventStore.IEventStore) expectedVersion item =
    let id = (^a : (member Id :  Guid) item)
    let uncommitedChanges = (^a : (member GetUncommittedChanges : unit -> obj Event seq) item)
    storage.SaveEvents(id, uncommitedChanges, expectedVersion)

let inline getById (storage:EventStore.IEventStore) apply id =
    let obj = new 'a()
    storage.GetEventsForAggregate id
      |> Seq.fold apply obj

let inline processItem storage apply id processF originalVersion =
    getById storage apply id
      |> processF
      |> save storage originalVersion