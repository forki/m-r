module CommandHandlers

open Domain
open Commands
open Messages
open Repository

module InventoryCommandHandlers = 
    let handleCreateInventoryItem (repository:IRepository<InventoryItem>) (message:CreateInventoryItem Command) =    
        let item = InventoryItem.Create(message.Data.InventoryItemId, message.Data.Name)
        repository.Save(item, -1)
    
    let handleDeactivateInventoryItem (repository:IRepository<InventoryItem>) (message:DeactivateInventoryItem Command) =
        let item = repository.GetById(message.Data.InventoryItemId)
        item.Deactivate()
        repository.Save(item, message.Data.OriginalVersion)

    let handleRemoveItemsFromInventory (repository:IRepository<InventoryItem>) (message:RemoveItemsFromInventory Command) =
        let item = repository.GetById(message.Data.InventoryItemId)
        item.Remove(message.Data.Count)
        repository.Save(item, message.Data.OriginalVersion)

    let handleCheckInItemsToInventory (repository:IRepository<InventoryItem>) (message:CheckInItemsToInventory Command) =
        let item = repository.GetById(message.Data.InventoryItemId)
        item.CheckIn(message.Data.Count)
        repository.Save(item, message.Data.OriginalVersion)

    let handleRenameInventoryItem (repository:IRepository<InventoryItem>) (message:RenameInventoryItem Command) =
        let item = repository.GetById(message.Data.InventoryItemId)
        item.ChangeName(message.Data.NewName)
        repository.Save(item, message.Data.OriginalVersion)