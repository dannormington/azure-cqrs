using SimpleCQRS.Domain;
using System;
using System.Linq;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Repository implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class, IAggregateRoot, new()
    {
        /// <summary>
        /// Event Store
        /// </summary>
        private readonly IEventStore _eventStore;

        /// <summary>
        /// Constructor
        /// </summary>
        public Repository()
        {
            _eventStore = new EventStore();
        }

        /// <summary>
        /// Save all changes
        /// </summary>
        /// <param name="aggregate"></param>
        /// <param name="handleEventsSynchronously"></param>
        public void Save(IAggregateRoot aggregate)
        {
            _eventStore.SaveEvents(aggregate.Id, aggregate.CurrentVersion, aggregate.GetUncommittedChanges());
            aggregate.MarkChangesAsCommitted();
        }


        /// <summary>
        /// Get an aggregate by it's Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById(Guid id)
        {
            T aggregate = null;
            var events = _eventStore.GetEvents(id);

            if (events != null && events.Any()) 
            {
                aggregate = new T();
                aggregate.LoadFromHistory(events);
            }
            
            return aggregate;
        }
    }
}
