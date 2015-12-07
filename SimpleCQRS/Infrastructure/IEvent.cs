using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
