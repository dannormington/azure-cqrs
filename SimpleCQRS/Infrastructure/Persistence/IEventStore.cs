using System;
using System.Collections.Generic;

namespace SimpleCQRS.Infrastructure.Persistence
{
    /// <summary>
    /// Interface to an event store
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Save the events
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="currentVersion"></param>
        /// <param name="events"></param>
        void SaveEvents(Guid aggregateId, int currentVersion, IEnumerable<IEvent> events);

        /// <summary>
        /// Get the events for an aggregate
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        IEnumerable<IEvent> GetEvents(Guid aggregateId);
    }
}
