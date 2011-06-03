module Commands

open System
open Messages

type DeactivateInventoryItem = { 
    InventoryItemId : Guid
    OriginalVersion : int }

type CreateInventoryItem = {
    InventoryItemId : Guid
    Name : string }

type RenameInventoryItem = {
    InventoryItemId : Guid
    NewName : string
    OriginalVersion : int }

type CheckInItemsToInventory = {
    InventoryItemId : Guid
    Count : int;
    OriginalVersion : int }

type RemoveItemsFromInventory = {
    InventoryItemId : Guid
    Count : int
    OriginalVersion : int }