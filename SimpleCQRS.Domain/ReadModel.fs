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

type InventoryItemLedgersDto = {
    Id : Guid
    ChangedCount : int }

type InventoryItemListDto = {
    Id: Guid
    Name: string }

let Details = new Dictionary<_,_>()
let Ledgers = new Dictionary<_,_>()
let Items = new Dictionary<_,_>()

let GetInventoryItems() = Items.Values |> Seq.cache
let GetInventoryItemDetails id = Details.[id]
let GetInventoryItemLedgers id = Ledgers.[id]

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

module InventoryItemLedgersView =
    let GetLedgersItem id =
        match Ledgers.TryGetValue id with
        | true, d -> d
        | _ -> raise <| new InvalidOperationException "did not find the original inventory this shouldnt happen"

    let handleInventoryItemEvent message = 
        match message.EventData with
        | Created(id,name)         -> Ledgers.Add(id, [])
        | Renamed(id,newName)      -> ()
        | Deactivated id           -> Ledgers.Remove id |> ignore
        | ItemsCheckedIn(id,count) -> Ledgers.[id] <- {Id = id; ChangedCount = count} :: Ledgers.[id]
        | ItemsRemoved(id,count)   -> Ledgers.[id] <- {Id = id; ChangedCount = -count} :: Ledgers.[id]
