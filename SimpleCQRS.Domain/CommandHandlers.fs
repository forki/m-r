module CommandHandlers

open Domain
open Commands
open Messages
open Repository

let handleInventoryItemCommand (repository:IRepository<InventoryItem>) message =
    match message.CommandData with
    | Create(id,name) -> 
        create id name
          |> repository.Save -1
    | Deactivate(id,originalVersion) -> 
        repository.GetById id
          |> deactivate
          |> repository.Save originalVersion
    | RemoveItems(id,count,originalVersion) -> 
        repository.GetById id
          |> remove count
          |> repository.Save originalVersion
    | CheckInItems(id,count,originalVersion) -> 
        repository.GetById id
          |> checkIn count
          |> repository.Save originalVersion
    | Rename(id,newName,originalVersion) -> 
        repository.GetById id
          |> changeName newName
          |> repository.Save originalVersion