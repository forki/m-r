module Repository

open SimpleCQRS.Dynamic
open System
open System.Collections.Generic

type AggregateRoot = AggregateRoot<obj Messages.Event>

type 'a IRepository when 'a :> AggregateRoot =
    abstract GetById: Guid -> 'a
    abstract Save: 'a * int -> unit

type 'a Repository when 'a :> AggregateRoot and 'a : (new: unit -> 'a) (storage:EventStore.IEventStore) =
    interface 'a IRepository with
      member this.Save(aggregate:'a,expectedVersion) =
          storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion)

      member this.GetById id =
        let obj = new 'a()
        let e = storage.GetEventsForAggregate(id)
        obj.LoadsFromHistory(e)
        obj