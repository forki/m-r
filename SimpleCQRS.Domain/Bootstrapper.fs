module Bootstrapper

open FakeBus
open EventStore
open Repository
open CommandHandlers


let createServiceBus() =
    let bus = new FakeBus()

    let storage = new EventStore(bus.Publish)
    let rep = new Repository<Domain.InventoryItem>(storage)

    bus.RegisterCommandHandler (InventoryCommandHandlers.handleCheckInItemsToInventory rep)
    bus.RegisterCommandHandler (InventoryCommandHandlers.handleCreateInventoryItem rep)
    bus.RegisterCommandHandler (InventoryCommandHandlers.handleDeactivateInventoryItem rep)
    bus.RegisterCommandHandler (InventoryCommandHandlers.handleRemoveItemsFromInventory rep)
    bus.RegisterCommandHandler (InventoryCommandHandlers.handleRenameInventoryItem rep)

    bus.RegisterEventHandler ReadModel.InventoryListView.handleInventoryItemEvent
    bus.RegisterEventHandler ReadModel.InventoryItemDetailView.handleInventoryItemEvent

    bus