﻿module CommandHandlers

open Domain
open Commands
open Events
open Messages
open Repository

let handleInventoryItemCommand (storage:EventStore.IEventStore) message =
    let getId (item:InventoryItem) = item.Root.Id
    let getUncommittedChanges (item:InventoryItem) = item.Root.UncommittedChanges
    let convert (event: obj Event) = {EventData = event.EventData :?> InventoryItemEvent; Version = event.Version }
    let apply (item:InventoryItem) =convert >> apply item false
    let f = processItem storage newItem apply getId getUncommittedChanges

    match message.CommandData with
    | Create(id,name) -> create id name |> save storage getId getUncommittedChanges -1
    | Deactivate(id,originalVersion)         -> f id deactivate originalVersion
    | RemoveItems(id,count,originalVersion)  -> f id (remove count) originalVersion
    | CheckInItems(id,count,originalVersion) -> f id (checkIn count) originalVersion
    | Rename(id,newName,originalVersion)     -> f id (changeName newName) originalVersion