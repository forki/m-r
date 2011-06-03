using System;
using System.Collections.Generic;

namespace SimpleCQRS.Dynamic
{
    public abstract class AggregateRoot<T>
    {
        private readonly List<T> _changes = new List<T>();
        public Guid Id { get; protected set;  }
        public int Version { get; internal set; }

        public IEnumerable<T> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<T> history)
        {
            foreach (var e in history) ApplyChange(e, false);
        }

        public void ApplyChange(T @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(T @event, bool isNew)
        {
            var d = this.AsDynamic();
            d.Apply(@event);
            if (isNew) _changes.Add(@event);
        }
    }
}
