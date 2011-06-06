module EventStore

open System
open System.Collections.Generic
open Messages

type IEventStore =
    abstract SaveEvents : Guid * obj Event seq * int -> unit
    abstract GetEventsForAggregate: Guid -> obj Event list

let performSideEffect f seq = Seq.map (fun x -> f x; x) seq
let concat list x = x :: list
    
type EventStore(publish) =
    let dict = new Dictionary<_,_>()

    let saveEvents (aggregateId:Guid) (events: obj Event seq) expectedVersion =
        let oldEvents =
            match dict.TryGetValue aggregateId with
            | true,(latestEvent::rest as all) ->
                if latestEvent.Version <> expectedVersion && expectedVersion <> -1 then
                    raise <| Exceptions.ConcurrencyException()
                else 
                    all
            | _ -> []

        events
          |> Seq.mapi (fun i event -> { event with Version = expectedVersion + i + 1})
          |> performSideEffect publish
          |> Seq.fold concat oldEvents
          |> fun events -> dict.[aggregateId] <- events

    let getEventsForAggregate aggregateId =
        match dict.TryGetValue aggregateId with
        | true,descriptors ->
            descriptors
              |> List.map downcastEvent
        | _ -> raise <| Exceptions.AggregateNotFoundException() 

    interface IEventStore with
      member this.SaveEvents(aggregateId,events,expectedVersion) = saveEvents aggregateId events expectedVersion
      member this.GetEventsForAggregate aggregateId = getEventsForAggregate aggregateId