using SimpleCQRS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleCQRS.Domain
{
    /// <summary>
    /// Aggregate root implementation
    /// </summary>
    public abstract class AggregateRoot : IAggregateRoot
    {
        /// <summary>
        /// List of changes to the aggregate
        /// </summary>
        private readonly List<IEvent> _changes = new List<IEvent>();

        /// <summary>
        /// Current version of the aggregate
        /// </summary>
        private int _currentVersion;

        /// <summary>
        /// Id of the aggregate
        /// </summary>
        protected Guid _id;

        /// <summary>
        /// Apply the change
        /// </summary>
        /// <param name="event"></param>
        protected void ApplyChange(IEvent @event)
        {
            ApplyChange(@event, true);
        }

        /// <summary>
        /// Apply the change
        /// </summary>
        /// <param name="event"></param>
        /// <param name="isNew"></param>
        private void ApplyChange(IEvent @event, bool isNew)
        {
            //call the private Apply method
            try
            {
                this.GetType().InvokeMember("Apply", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, this, new[] { @event });
            }
            catch (MissingMethodException) 
            {
                //do nothing. This just means that an Apply method was not implemented 
                //because the state is not needed within the domain model
            }
            

            if (isNew) _changes.Add(@event);
        }

        IEnumerable<IEvent> IAggregateRoot.GetUncommittedChanges()
        {
            return _changes;
        }

        void IAggregateRoot.MarkChangesAsCommitted()
        {
            this._currentVersion += this._changes.Count;
            _changes.Clear();
        }

        void IAggregateRoot.LoadFromHistory(IEnumerable<IEvent> history)
        {
            foreach (var e in history) 
            {
                ApplyChange(e, false);
                _currentVersion++;
            } 
        }

        Guid IAggregateRoot.Id
        {
            get { return _id; }
        }

        int IAggregateRoot.CurrentVersion 
        {
            get { return _currentVersion; }
        }
    }
}
