module ReadModel

open System
open DB
open Events
open Messages

type IReadModelFacade =
    abstract GetInventoryItems : unit -> InventoryItemListDto seq
    abstract GetInventoryItemDetails : Guid -> InventoryItemDetailsDto

type  ReadModelFacade() =
    interface IReadModelFacade with
        member this.GetInventoryItems() = List |> Seq.cache
        member this.GetInventoryItemDetails id = Details.[id]

module InventoryListView =
    let handleInventoryItemCreated (message:InventoryItemCreated Event) = 
        List.Add({Id = message.Data.Id; Name = message.Data.Name})

    let handleInventoryItemRenamed (message:InventoryItemRenamed Event) = 
        List.Find(fun x -> x.Id = message.Data.Id).Name <- message.Data.NewName

    let handleInventoryItemDeactivated (message:InventoryItemDeactivated Event) = 
        List.RemoveAll(fun x -> x.Id = message.Data.Id) |> ignore

module InventoryItemDetailView =
    let handleInventoryItemCreated (message:InventoryItemCreated Event) = 
        Details.Add(message.Data.Id, {Id = message.Data.Id; Name = message.Data.Name; CurrentCount = 0; Version = 0})

    let GetDetailsItem id =
        match Details.TryGetValue id with
        | true, d -> d
        | _ -> raise <| new InvalidOperationException "did not find the original inventory this shouldnt happen"

    let handleInventoryItemRenamed (message:InventoryItemRenamed Event) = 
          let d = GetDetailsItem message.Data.Id
          d.Name <- message.Data.NewName
          d.Version <- message.Version

    let handleItemsRemovedFromInventory (message:ItemsRemovedFromInventory Event) = 
          let d = GetDetailsItem message.Data.Id
          d.CurrentCount <- d.CurrentCount - message.Data.Count
          d.Version <- message.Version

    let handleItemsCheckedInToInventory (message:ItemsCheckedInToInventory Event) = 
          let d = GetDetailsItem message.Data.Id
          d.CurrentCount <- d.CurrentCount + message.Data.Count
          d.Version <- message.Version

    let handleInventoryItemDeactivated (message:InventoryItemDeactivated Event) = 
          Details.Remove(message.Data.Id) |> ignore