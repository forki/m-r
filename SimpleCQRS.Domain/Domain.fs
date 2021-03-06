﻿module Domain

open System
open Messages
open Events
open Aggregate

type InventoryItem = {
    Root: Root<InventoryItemEvent>
    Activated: bool
    Count : int }  
    with 
        member this.GetAggregateRoot() =  this.Root  // Todo: Remove this
        static member New() = { Root = Root<_>.New(); Activated = false; Count = 0}

let apply isNew (item:InventoryItem) event =
    let root = item.Root.ApplyChange isNew event
    match event.EventData with
    | Deactivated id          -> {item with Root = root; Activated = false }
    | Created(id,name)        -> {item with Root = {root with Id = id}; Activated = true }
    | ItemsCheckedIn(_,count) -> {item with Root = root; Count = item.Count + count }
    | ItemsRemoved(_,count)   -> {item with Root = root; Count = item.Count - count }
    | _ -> item

let create id name =
    InventoryItemEvent.Created(id,name) |> toEvent |> apply true (InventoryItem.New())

let changeName newName (item:InventoryItem) =
    if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
    InventoryItemEvent.Renamed(item.Root.Id,newName) |> toEvent |> apply true item

let remove count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "can't remove negative count from inventory"
    if item.Count < count then raise <| new InvalidOperationException "can't remove item, since the inventory would go below zero"
    InventoryItemEvent.ItemsRemoved(item.Root.Id,count) |> toEvent |> apply true item

let checkIn count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
    InventoryItemEvent.ItemsCheckedIn(item.Root.Id,count) |> toEvent |> apply true item

let deactivate (item:InventoryItem) =
    if not item.Activated then raise <| new InvalidOperationException "already deactivated"
    InventoryItemEvent.Deactivated item.Root.Id |> toEvent |> apply true item
