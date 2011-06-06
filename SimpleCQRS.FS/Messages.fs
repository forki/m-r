module Messages

type 'a Command = { CommandData: 'a }

type 'a Event = {
    EventData: 'a
    Version: int }

let toCommand x = { CommandData = x } : 'a Command
let toEvent x = { EventData = x; Version = 0 }

let inline upcastEvent (event: obj Event) = {EventData = event.EventData :?> 'a; Version = event.Version }
let inline downcastEvent (event: 'a Event) = { EventData = event.EventData :> obj; Version = event.Version}