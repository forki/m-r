module Commands

open System
open Messages

type InventoryItemCommand =
| Create of Guid * string
| Deactivate of Guid * int
| Rename of Guid * string * int
| CheckInItems of Guid * int * int
| RemoveItems of Guid * int * int