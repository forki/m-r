module CommandHandlers

open Domain
open Commands
open Events
open Messages
open Repository

let handleInventoryItemCommand (storage:EventStore.IEventStore) message =
    let convert (event: obj Event) = {EventData = event.EventData :?> InventoryItemEvent; Version = event.Version }
    let f = processItem storage (fun () -> new InventoryItem()) (fun (item:InventoryItem) -> convert >> item.Apply false)
    match message.CommandData with
    | Create(id,name) -> create id name |> save storage -1
    | Deactivate(id,originalVersion)         -> f id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> f id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> f id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> f id (changeName newName) originalVersion