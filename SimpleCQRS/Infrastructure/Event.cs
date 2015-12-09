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
        public readonly Guid Id;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id"></param>
        protected Event(Guid id) 
        {
            this.Id = id;
        }

        Guid IEvent.Id
        {
            get { return this.Id; }
        }
    }
}
