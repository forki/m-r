module EventStore

open System
open System.Collections.Generic
open Messages

type IEventStore =
    abstract SaveEvents : Guid * obj Event seq * int -> unit
    abstract GetEventsForAggregate: Guid -> obj Event list

type EventDescriptor = {
    Id : Guid
    Data : obj
    Version : int }
    
type EventStore(publish) =
    let dict = new Dictionary<_,_>()

    let saveEvents (aggregateId:Guid) (events: obj Event seq) expectedVersion =
        let eventDescriptors =
            match dict.TryGetValue aggregateId with
            | true,(descriptor::rest as all) ->
                if descriptor.Version <> expectedVersion && expectedVersion <> -1 then
                    raise <| Exceptions.ConcurrencyException()
                else 
                    all
            | _ -> []

        events
          |> Seq.fold (fun (i,descriptors) event ->
              let newVersion = i + 1
              publish { event with Version = newVersion}
              newVersion,{Id = aggregateId; Data = event.Data; Version = newVersion}::descriptors)
              (expectedVersion,eventDescriptors)
          |> fun (_,descriptors) -> dict.[aggregateId] <- descriptors

    let getEventsForAggregate aggregateId =
        match dict.TryGetValue aggregateId with
        | true,descriptors ->
            descriptors
              |> List.map (fun x -> { Data = x.Data; Version = x.Version} : obj Event)
        | _ -> raise <| Exceptions.AggregateNotFoundException() 

    interface IEventStore with
      member this.SaveEvents(aggregateId,events,expectedVersion) = saveEvents aggregateId events expectedVersion
      member this.GetEventsForAggregate aggregateId = getEventsForAggregate aggregateId