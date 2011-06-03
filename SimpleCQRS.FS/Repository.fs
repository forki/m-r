module Repository

open System
open System.Collections.Generic
open Messages

let inline save (storage:EventStore.IEventStore) expectedVersion item =
    let id = (^a : (member Id :  Guid) item)
    let uncommitedChanges = (^a : (member GetUncommittedChanges : unit -> obj Event seq) item)
    storage.SaveEvents(id, uncommitedChanges, expectedVersion)

let inline getById (storage:EventStore.IEventStore) init apply id =
    storage.GetEventsForAggregate id
      |> Seq.fold apply (init())

let inline processItem storage init apply id processF originalVersion =
    getById storage init apply id
      |> processF
      |> save storage originalVersion