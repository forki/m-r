module Messages

type 'a Command = { CommandData: 'a }

type 'a Event = {
    EventData: 'a
    Version: int }

let toCommand x = { CommandData = x } : 'a Command
let toEvent x = { EventData = x; Version = 0 }