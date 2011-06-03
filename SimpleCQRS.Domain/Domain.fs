﻿module Domain

open System
open Messages
open Events

type InventoryItem() =
    inherit Aggregate.Root<obj Messages.Event>()
    let mutable activated = false
    let mutable count = 0
    let mutable id = Guid.Empty

    member this.Count = count
    member this.Activated = activated
    member this.Id = id
    
    member this.Apply isNew (x:obj Event) =
        this.ApplyChange isNew x
        match x.EventData with
        | :? InventoryItemEvent as e -> 
            match e with
            | Deactivated id -> 
                activated <- false
                this
            | Created(newId,name) -> 
                id <- newId
                activated <- true
                this
            | ItemsCheckedIn(_,c) -> 
                count <- count + c
                this
            | ItemsRemoved(_,c) -> 
                count <- count - c
                this
            | _ -> this
        | _ -> this

let create id name =
    let item = InventoryItem()
    InventoryItemEvent.Created(id,name) |> toEvent |> item.Apply true

let changeName newName (item:InventoryItem) =
    if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
    InventoryItemEvent.Renamed(item.Id,newName) |> toEvent |> item.Apply true

let remove count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "can't remove negative count from inventory"
    if item.Count < count then raise <| new InvalidOperationException "can't remove item, since the inventory would go below zero"
    InventoryItemEvent.ItemsRemoved(item.Id,count) |> toEvent |> item.Apply true

let checkIn count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
    InventoryItemEvent.ItemsCheckedIn(item.Id,count) |> toEvent |> item.Apply true

let deactivate (item:InventoryItem) =
    if not item.Activated then raise <| new InvalidOperationException "already deactivated"
    InventoryItemEvent.Deactivated item.Id |> toEvent |> item.Apply true