module Events

open System
open Messages

type InventoryItemEvent =
| Created of Guid * string
| Deactivated of Guid
| Renamed of Guid * string
| ItemsCheckedIn of Guid * int
| ItemsRemoved of Guid * int