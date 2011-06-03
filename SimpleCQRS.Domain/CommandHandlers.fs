module CommandHandlers

open Domain
open Commands
open Messages
open Repository

let handleInventoryItemCommand (repository:IRepository<InventoryItem>) message =
    match message.CommandData with
    | Create(id,name) -> create id name |> repository.Save -1
    | Deactivate(id,originalVersion)         -> repository.ProcessItem id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> repository.ProcessItem id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> repository.ProcessItem id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> repository.ProcessItem id (changeName newName) originalVersion