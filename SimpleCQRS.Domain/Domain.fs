module Domain

open System
open Messages
open Events

type InventoryItem() as this =
    inherit Repository.AggregateRoot()
    let apply = toEvent >> this.ApplyChange

    let mutable activated = false
    
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

    member this.ChangeName newName =
        if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
        InventoryItemEvent.Renamed(this.Id,newName) |> apply

    member this.Remove count =
        if count <= 0 then raise <| new InvalidOperationException "cant remove negative count from inventory"
        InventoryItemEvent.ItemsRemoved(this.Id,count) |> apply

    member this.CheckIn count =
        if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
        InventoryItemEvent.ItemsCheckedIn(this.Id,count) |> apply

    member this.Deactivate() =
        if not activated then raise <| new InvalidOperationException "already deactivated"
        InventoryItemEvent.Deactivated this.Id |> apply

    static member Create(id,name) =
        let this = InventoryItem()
        InventoryItemEvent.Created(id,name)
          |> toEvent
          |> this.ApplyChange

        this