module Bootstrapper

let createServiceBus() =
    let bus = new FakeBus.FakeBus()
    let storage = new EventStore.EventStore(bus.Publish)

    bus.RegisterCommandHandler (CommandHandlers.handleInventoryItemCommand storage)
    
    bus.RegisterEventHandler ReadModel.InventoryListView.handleInventoryItemEvent
    bus.RegisterEventHandler ReadModel.InventoryItemDetailView.handleInventoryItemEvent
    bus.RegisterEventHandler ReadModel.InventoryItemLedgersView.handleInventoryItemEvent

    bus