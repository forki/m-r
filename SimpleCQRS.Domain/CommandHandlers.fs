module CommandHandlers

open Domain
open Commands
open Messages
open Repository

let handleInventoryItemCommand (repository:IRepository<InventoryItem>) message =
    match message.CommandData with
    | Create(id,name) -> 
        let item = InventoryItem.Create(id, name)
        repository.Save(item, -1)
    | Deactivate(id,originalVersion) -> 
        let item = repository.GetById id
        item.Deactivate()
        repository.Save(item, originalVersion)
    | RemoveItems(id,count,originalVersion) -> 
        let item = repository.GetById id
        item.Remove count
        repository.Save(item, originalVersion)
    | CheckInItems(id,count,originalVersion) -> 
        let item = repository.GetById id
        item.CheckIn count
        repository.Save(item, originalVersion)
    | Rename(id,newName,originalVersion) -> 
        let item = repository.GetById id
        item.ChangeName newName
        repository.Save(item, originalVersion)