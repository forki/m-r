module Domain

open System
open Messages
open Events

type InventoryItem() =
    inherit Repository.AggregateRoot()
    let mutable activated = false

    member this.Activated = activated
    
    member this.Apply(x:obj Event) =
        match x.EventData with
        | :? InventoryItemEvent as e -> 
            match e with
            | Deactivated id -> activated <- false
            | Created(id,name) -> 
                this.Id <- id
                activated <- true
            | _ -> ()
        | _ -> ()

let create id name =
    let item = InventoryItem()
    InventoryItemEvent.Created(id,name) |> toEvent |> item.ApplyChange

let changeName newName (item:InventoryItem) =
    if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
    InventoryItemEvent.Renamed(item.Id,newName) |> toEvent |> item.ApplyChange

let remove count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "cant remove negative count from inventory"
    InventoryItemEvent.ItemsRemoved(item.Id,count) |> toEvent |> item.ApplyChange

let checkIn count (item:InventoryItem) =
    if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
    InventoryItemEvent.ItemsCheckedIn(item.Id,count) |> toEvent |> item.ApplyChange

let deactivate (item:InventoryItem) =
    if not item.Activated then raise <| new InvalidOperationException "already deactivated"
    InventoryItemEvent.Deactivated item.Id |> toEvent |> item.ApplyChange