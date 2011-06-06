module ReadModel

open System
open System.Collections.Generic
open Events
open Messages

type InventoryItemDetailsDto = {
    Id : Guid
    Name : string
    CurrentCount : int
    Version : int }

type InventoryItemListDto = {
    Id: Guid
    Name: string }

let Details = new Dictionary<Guid,InventoryItemDetailsDto>()
let Items = new Dictionary<Guid,InventoryItemListDto>()

type IReadModelFacade =
    abstract GetInventoryItems : unit -> InventoryItemListDto seq
    abstract GetInventoryItemDetails : Guid -> InventoryItemDetailsDto

type ReadModelFacade() =
    interface IReadModelFacade with
        member this.GetInventoryItems() = Items.Values |> Seq.cache
        member this.GetInventoryItemDetails id = Details.[id]

module InventoryListView =
    let handleInventoryItemEvent message = 
        match message.EventData with
        | Created(id,name)    -> Items.Add(id,{Id = id; Name = name})
        | Renamed(id,newName) -> Items.[id] <- { Items.[id] with Name = newName }
        | Deactivated id      -> Items.Remove(id) |> ignore
        | _ -> ()

module InventoryItemDetailView =
    let GetDetailsItem id =
        match Details.TryGetValue id with
        | true, d -> d
        | _ -> raise <| new InvalidOperationException "did not find the original inventory this shouldnt happen"

    let handleInventoryItemEvent message = 
        match message.EventData with
        | Created(id,name)         -> Details.Add(id, {Id = id; Name = name; CurrentCount = 0; Version = 0})
        | Renamed(id,newName)      -> Details.[id] <- { Details.[id] with Name = newName; Version = message.Version }
        | Deactivated id           -> Details.Remove id |> ignore
        | ItemsCheckedIn(id,count) -> Details.[id] <- { Details.[id] with CurrentCount = Details.[id].CurrentCount + count; Version = message.Version }
        | ItemsRemoved(id,count)   -> Details.[id] <- { Details.[id] with CurrentCount = Details.[id].CurrentCount - count; Version = message.Version }