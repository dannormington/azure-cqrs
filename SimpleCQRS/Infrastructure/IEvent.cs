using System;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Interface for an event
    /// </summary>
    public interface IEvent : IMessage
    {
        /// <summary>
        /// Id of the aggregate
        /// </summary>
        Guid Id { get; }
    }
}
