using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Simple interface to handle registering event handlers
    /// </summary>
    public interface IEventBus : IDisposable
    {
        /// <summary>
        /// Register a handler for a specific event
        /// </summary>
        /// <typeparam name="TMessageType"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void RegisterEventHandler<TEvent, TImplementation>()
            where TEvent : IEvent
            where TImplementation : class, IHandles<TEvent>;

        /// <summary>
        /// Publish the event to all handlers asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishAsync<T>(T @event)
            where T : class, IEvent;

        /// <summary>
        /// Publish the event to all handlers asynchronously
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task PublishAsync(object @event);

        /// <summary>
        /// Publish the event to all handlers synchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        void Publish<T>(T @event)
            where T : class, IEvent;

        /// <summary>
        /// Publish the event to all handlers synchronously
        /// </summary>
        /// <param name="event"></param>
        void Publish(object @event);

        /// <summary>
        /// Publish the events to all handlers synchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="events"></param>
        void Publish<T>(IEnumerable<T> events)
            where T : class, IEvent;

        /// <summary>
        /// Publish the events to the default queue asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="events"></param>
        /// <returns></returns>
        Task PublishToQueueAsync<T>(IEnumerable<T> events)
            where T : class, IEvent;
    }
}
