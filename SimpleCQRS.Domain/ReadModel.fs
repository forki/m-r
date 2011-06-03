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
    let handleInventoryItemEvent message = 
        match message.EventData with
        | Created(id,name)    -> List.Add {Id = id; Name = name}
        | Renamed(id,newName) -> List.Find(fun x -> x.Id = id).Name <- newName
        | Deactivated id      -> List.RemoveAll(fun x -> x.Id = id) |> ignore
        | _ -> ()

module InventoryItemDetailView =
    let GetDetailsItem id =
        match Details.TryGetValue id with
        | true, d -> d
        | _ -> raise <| new InvalidOperationException "did not find the original inventory this shouldnt happen"

    let handleInventoryItemEvent message = 
        match message.EventData with
        | Created(id,name)         -> Details.Add(id, {Id = id; Name = name; CurrentCount = 0; Version = 0})
        | Renamed(id,newName)      -> 
            let d = GetDetailsItem id
            d.Name <- newName
            d.Version <- message.Version
        | Deactivated id           -> Details.Remove id |> ignore
        | ItemsCheckedIn(id,count) ->
            let d = GetDetailsItem id
            d.CurrentCount <- d.CurrentCount + count
            d.Version <- message.Version 
        | ItemsRemoved(id,count) ->
            let d = GetDetailsItem id
            d.CurrentCount <- d.CurrentCount - count
            d.Version <- message.Version