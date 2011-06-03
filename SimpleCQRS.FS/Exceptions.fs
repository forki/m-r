module Exceptions

open System

exception AggregateNotFoundException of unit
exception ConcurrencyException of unit