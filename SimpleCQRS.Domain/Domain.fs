module Domain

open System
open Messages
open Events

type InventoryItem() =
    inherit Repository.AggregateRoot()
    let mutable activated = false
    let mutable count = 0

    member this.Count = count
    member this.Activated = activated
    
    member this.Apply(x:obj Event) =
        match x.EventData with
        | :? InventoryItemEvent as e -> 
            match e with
            | Deactivated id -> activated <- false
            | Created(id,name) -> 
                this.Id <- id
                activated <- true
            | ItemsCheckedIn(_,c) -> count <- count + c
            | ItemsRemoved(_,c) -> count <- count - c
            | _ -> ()
        | _ -> ()

let create id name =
    let item = InventoryItem()
    InventoryItemEvent.Created(id,name) |> toEvent |> item.ApplyChange
    item

let changeName newName (item:InventoryItem) =
    if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
    InventoryItemEvent.Renamed(item.Id,newName) |> toEvent |> item.ApplyChange

let remove count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "can't remove negative count from inventory"
    if item.Count < count then raise <| new InvalidOperationException "can't remove item, since the inventory would go below zero"
    InventoryItemEvent.ItemsRemoved(item.Id,count) |> toEvent |> item.ApplyChange

let checkIn count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
    InventoryItemEvent.ItemsCheckedIn(item.Id,count) |> toEvent |> item.ApplyChange

let deactivate (item:InventoryItem) =
    if not item.Activated then raise <| new InvalidOperationException "already deactivated"
    InventoryItemEvent.Deactivated item.Id |> toEvent |> item.ApplyChange