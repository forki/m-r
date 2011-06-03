module Domain

open System
open Messages
open Events

type InventoryItem() as this =
    inherit Repository.AggregateRoot()
    let apply x = x |> toEvent |> this.ApplyChange

    let mutable activated = false
    
    member this.Apply(x:obj Event) =
      match x.Data with
      | :? InventoryItemDeactivated as e -> 
          activated <- false
      | :? InventoryItemCreated as e -> 
          this.Id <- e.Id
          activated <- true
      | _ -> ()

    member this.ChangeName newName =
        if String.IsNullOrEmpty newName then raise <| new ArgumentException "newName"
        apply ({ Id = this.Id; NewName = newName } : InventoryItemRenamed)

    member this.Remove count =
        if count <= 0 then raise <| new InvalidOperationException "cant remove negative count from inventory"
        apply ({ Id = this.Id; Count = count } : ItemsRemovedFromInventory)

    member this.CheckIn count =
        if count <= 0 then raise <| new InvalidOperationException "must have a count greater than 0 to add to inventory"
        apply ({ Id = this.Id; Count = count } : ItemsCheckedInToInventory)

    member this.Deactivate() =
        if not activated then raise <| new InvalidOperationException "already deactivated"
        apply ({ Id = this.Id } : InventoryItemDeactivated)

    static member Create(id,name) =
        let this = InventoryItem()
        ({ Id = id; Name = name } : InventoryItemCreated)
          |> toEvent
          |> this.ApplyChange

        this