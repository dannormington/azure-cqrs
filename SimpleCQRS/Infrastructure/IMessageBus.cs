using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Simple interface to handle sending messages
    /// </summary>
    public interface IMessageBus : IDisposable
    {
        /// <summary>
        /// Register a handler for a specific event
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        void RegisterHandler<TMessage, TImplementation>()
            where TMessage : IMessage
            where TImplementation : class, IHandles<TMessage>;

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
        /// Publish the events to the default queue asynchronously
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="events"></param>
        /// <returns></returns>
        Task PublishToQueueAsync<T>(IEnumerable<T> events)
            where T : class, IEvent;

        /// <summary>
        /// Send a command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        Task SendAsync<T>(T command)
            where T : class, ICommand;
    }
}
