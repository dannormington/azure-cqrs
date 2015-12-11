using Newtonsoft.Json;
using System;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Represents a base class for all events
    /// </summary>
    public abstract class Event : IEvent
    {
        /// <summary>
        /// The aggregate Id
        /// </summary>
        [JsonProperty] //Had to add this because base class readonly properties weren't getting deserialized
        public readonly Guid Id;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id"></param>
        public Event(Guid id) 
        {
            this.Id = id;
        }

        Guid IEvent.Id
        {
            get { return this.Id; }
        }
    }
}
