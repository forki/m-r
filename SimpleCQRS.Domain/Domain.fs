module Domain

open System
open Messages
open Events

type InventoryItem = {
    Root: Aggregate.Root<InventoryItemEvent>
    Id: Guid
    Activated: bool
    Count : int }    

let newItem() = {Root = new Aggregate.Root<InventoryItemEvent>(); Id = Guid.Empty; Activated = false; Count = 0}
let apply (item:InventoryItem) isNew event =
    item.Root.ApplyChange isNew event
    match event.EventData with
    | Deactivated id          -> {item with Activated = false }
    | Created(id,name)        -> {item with Id = id; Activated = true }
    | ItemsCheckedIn(_,count) -> {item with Count = item.Count + count }
    | ItemsRemoved(_,count)   -> {item with Count = item.Count - count }
    | _ -> item

let create id name =
    InventoryItemEvent.Created(id,name) |> toEvent |> apply (newItem()) true

let changeName newName (item:InventoryItem) =
    if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
    InventoryItemEvent.Renamed(item.Id,newName) |> toEvent |> apply item true

let remove count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "can't remove negative count from inventory"
    if item.Count < count then raise <| new InvalidOperationException "can't remove item, since the inventory would go below zero"
    InventoryItemEvent.ItemsRemoved(item.Id,count) |> toEvent |> apply item true

let checkIn count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
    InventoryItemEvent.ItemsCheckedIn(item.Id,count) |> toEvent |> apply item true

let deactivate (item:InventoryItem) =
    if not item.Activated then raise <| new InvalidOperationException "already deactivated"
    InventoryItemEvent.Deactivated item.Id |> toEvent |> apply item true