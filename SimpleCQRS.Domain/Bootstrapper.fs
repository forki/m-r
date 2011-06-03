module Bootstrapper

open FakeBus
open EventStore
open Repository
open CommandHandlers


let createServiceBus() =
    let bus = new FakeBus()

    let storage = new EventStore(bus.Publish)
    let rep = new Repository<Domain.InventoryItem>(storage)

    bus.RegisterCommandHandler (CommandHandlers.handleInventoryItemCommand rep)
    
    bus.RegisterEventHandler ReadModel.InventoryListView.handleInventoryItemEvent
    bus.RegisterEventHandler ReadModel.InventoryItemDetailView.handleInventoryItemEvent

    bus