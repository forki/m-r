module CommandHandlers

open Domain
open Commands
open Messages
open Repository

let handleInventoryItemCommand (repository:IRepository<InventoryItem>) message =
    let processItem id f originalVersion = repository.GetById id |> f |> repository.Save originalVersion

    match message.CommandData with
    | Create(id,name) -> create id name |> repository.Save -1
    | Deactivate(id,originalVersion)         -> processItem id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> processItem id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> processItem id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> processItem id (changeName newName) originalVersion