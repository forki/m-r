module DB

open System
open System.Collections.Generic

type InventoryItemDetailsDto = {
    Id : Guid
    mutable Name : string
    mutable CurrentCount : int
    mutable Version : int }

type InventoryItemListDto = {
    Id: Guid
    mutable Name: string }

let Details = new Dictionary<Guid,InventoryItemDetailsDto>()
let List = new List<InventoryItemListDto>()