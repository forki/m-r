module Bootstrapper

let createServiceBus() =
    let bus = new FakeBus.FakeBus()
    let storage = new EventStore.EventStore(bus.Publish)
    let repo = new Repository.Repository<Domain.InventoryItem>(storage)

    bus.RegisterCommandHandler (CommandHandlers.handleInventoryItemCommand repo)
    
    bus.RegisterEventHandler ReadModel.InventoryListView.handleInventoryItemEvent
    bus.RegisterEventHandler ReadModel.InventoryItemDetailView.handleInventoryItemEvent

    bus