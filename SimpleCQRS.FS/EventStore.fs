module EventStore

open System
open System.Collections.Generic
open Messages

type IEventStore =
    abstract SaveEvents : Guid * obj Event seq * int -> unit
    abstract GetEventsForAggregate: Guid -> obj Event list
    
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
          |> Seq.fold (fun (i,processedEvents) event ->
              let newVersion = i + 1
              let newEvent = { event with Version = newVersion}
              publish newEvent
              newVersion,newEvent::processedEvents)
              (expectedVersion,oldEvents)
          |> fun (_,descriptors) -> dict.[aggregateId] <- descriptors

    let getEventsForAggregate aggregateId =
        match dict.TryGetValue aggregateId with
        | true,descriptors ->
            descriptors
              |> List.map downcastEvent
        | _ -> raise <| Exceptions.AggregateNotFoundException() 

    interface IEventStore with
      member this.SaveEvents(aggregateId,events,expectedVersion) = saveEvents aggregateId events expectedVersion
      member this.GetEventsForAggregate aggregateId = getEventsForAggregate aggregateId