using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Entity representation of an event
    /// </summary>
    internal class EventEntity : TableEntity
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EventEntity() { }

        /// <summary>
        /// Build an event entity
        /// </summary>
        /// <param name="event"></param>
        /// <param name="version"></param>
        public EventEntity(IEvent @event, int version) 
        {
            this.PartitionKey = @event.Id.ToString();
            this.RowKey = version.ToString();

            this.Event = JsonConvert.SerializeObject(@event);
            this.Type = @event.GetType().AssemblyQualifiedName;
            this.Version = version;
        }

        /// <summary>
        /// json representation of event
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Type to help with serialization
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Version to help with order
        /// </summary>
        public int Version { get; set; }
    }
}
