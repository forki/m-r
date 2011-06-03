module Repository

open SimpleCQRS.Dynamic
open System
open System.Collections.Generic

type AggregateRoot = AggregateRoot<obj Messages.Event>

type 'a IRepository when 'a :> AggregateRoot =
    abstract GetById: Guid -> 'a
    abstract Save: int -> 'a -> unit
    abstract ProcessItem: Guid -> ('a -> unit) -> int -> unit

type 'a Repository when 'a :> AggregateRoot and 'a : (new: unit -> 'a) (storage:EventStore.IEventStore) =
    let getById id =
        let obj = new 'a()
        let e = storage.GetEventsForAggregate(id)
        obj.LoadsFromHistory(e)
        obj

    let save expectedVersion (item:'a) = storage.SaveEvents(item.Id, item.GetUncommittedChanges(), expectedVersion)

    let processItem id processF originalVersion =
          let item = getById id 
          processF item
          save originalVersion item

    interface 'a IRepository with
      member this.Save expectedVersion item = save expectedVersion item
      member this.GetById id = getById id
      member this.ProcessItem id processF originalVersion = processItem id processF originalVersion