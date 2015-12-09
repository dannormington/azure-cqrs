using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
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
