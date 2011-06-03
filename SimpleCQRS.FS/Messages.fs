module Messages

type Message = interface end

type 'a Command = { CommandData: 'a }
    with interface Message

type 'a Event = {
    EventData: 'a
    Version: int }
    with interface Message

let toCommand x = { CommandData = x } : 'a Command
let toEvent x = { EventData = x :> obj ; Version = 0 } 