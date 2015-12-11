using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleCQRS.Infrastructure.Persistence
{
    /// <summary>
    /// Implementation of an event store using Azure table storage
    /// </summary>
    public class EventStore : IEventStore
    {
        private readonly IMessageBus _messageBus;
        private readonly string _storageConnectionString;
        private readonly string _eventTable;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="messageBus"></param>
        public EventStore(IMessageBus messageBus) 
        {
            _messageBus = messageBus;
            _storageConnectionString = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Storage.ConnectionString");
            _eventTable = CloudConfigurationManager.GetSetting("Microsoft.WindowsAzure.Storage.EventStoreTable");
        }

        /// <summary>
        /// Save the events to the event store and publish the events
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <param name="currentVersion"></param>
        /// <param name="events"></param>
        public void SaveEvents(Guid aggregateId, int currentVersion, IEnumerable<IEvent> events)
        {
            if (events == null || !events.Any())
                return;

            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_eventTable);
            table.CreateIfNotExists();
            
            var batchOperation = new TableBatchOperation();

            foreach(var @event in events)
            {
                currentVersion++;
                batchOperation.Insert(new EventEntity(@event, currentVersion));
            }

            var results = table.ExecuteBatch(batchOperation);

            if (results.Any()) 
            {
                //no need to wait for publishing to the queue
                _messageBus.PublishToQueueAsync(events);
            }
        }

        /// <summary>
        /// Get all of the events for an aggregate
        /// </summary>
        /// <param name="aggregateId"></param>
        /// <returns></returns>
        public IEnumerable<IEvent> GetEvents(Guid aggregateId)
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(_eventTable);
            table.CreateIfNotExists();

            var query = (from @event in table.CreateQuery<EventEntity>()
                        where @event.PartitionKey == aggregateId.ToString()
                        select @event).ToList();

            var events = new List<IEvent>();

            if (query.Any()) 
            {
                foreach (var eventEntity in query.OrderBy(x => x.Version))
                {
                    Type type = Type.GetType(eventEntity.Type);
                    var @event = JsonConvert.DeserializeObject(eventEntity.Event, type);

                    events.Add((IEvent)@event);
                }
            }

            return events;
        }
    }
}
