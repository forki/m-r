module Messages

type Message = interface end

type 'a Command = { Data: 'a }
    with interface Message

type 'a Event = {
    Data: 'a
    Version: int }
    with interface Message

let toCommand x = { Data = x } : 'a Command
let toEvent x = { Data = x :> obj ; Version = 0 } 