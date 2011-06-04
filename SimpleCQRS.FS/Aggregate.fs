﻿module Aggregate

open System
open Messages 

type 'a Root = {
    Id : Guid
    UncommittedChanges: 'a Event list }
    with
        member this.ApplyChange isNew event = 
          if isNew then {this with UncommittedChanges = event :: this.UncommittedChanges} else this

let NewRoot() = {Id = Guid.Empty; UncommittedChanges = [] }