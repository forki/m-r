module CommandHandlers

open Domain
open Commands
open Events
open Messages
open Repository

let handleInventoryItemCommand (storage:EventStore.IEventStore) message =
    let processItem = processItem storage newItem (apply false)

    match message.CommandData with
    | Create(id,name) -> create id name |> save storage -1
    | Deactivate(id,originalVersion)         -> processItem id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> processItem id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> processItem id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> processItem id (changeName newName) originalVersion