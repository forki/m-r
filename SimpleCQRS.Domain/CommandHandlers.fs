module CommandHandlers

open Domain
open Commands
open Events
open Messages
open Repository

let handleInventoryItemCommand (storage:EventStore.IEventStore) message =
    let processCommand = processCommand storage InventoryItem.New (apply false)

    match message.CommandData with
    | Create(id,name) -> create id name |> save storage -1
    | Deactivate(id,originalVersion)         -> processCommand id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> processCommand id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> processCommand id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> processCommand id (changeName newName) originalVersion