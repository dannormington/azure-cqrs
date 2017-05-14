using Microsoft.Azure;
using Microsoft.Practices.Unity;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace SimpleCQRS.Infrastructure
{
    /// <summary>
    /// Simple message bus implementation
    /// </summary>
    public class MessageBus : IMessageBus
    {
        /// <summary>
        /// instance of unity container
        /// </summary>
        private readonly IUnityContainer _unityContainer;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="unityContainer"></param>
        public MessageBus(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }

        void IDisposable.Dispose()
        {
            _unityContainer.Dispose();
        }

        void IMessageBus.RegisterHandler<TEvent, TImplementation>()
        {
            var name = string.Format("{0}_{1}", typeof(TImplementation).Name, typeof(TEvent).Name);
            _unityContainer.RegisterType<IHandles<TEvent>, TImplementation>(name);
        }

        Task IMessageBus.PublishAsync<T>(T @event)
        {
            return PublishAsync(@event);
        }

        Task IMessageBus.PublishAsync(object @event)
        {
            return PublishAsync(@event);
        }

        Task IMessageBus.PublishToQueueAsync<T>(IEnumerable<T> events)
        {
            return PublishToQueueAsync(events);
        }

        Task IMessageBus.SendAsync<T>(T command)
        {
            return SendAsync(command);
        }

        /// <summary>
        /// Publish the message to all handlers asynchronously
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task PublishAsync(object @event)
        {
            if (@event == null)
                return;

            //Get a instance of the generic handler's type
            Type genericType = typeof(IHandles<>);

            //Get and instance of the domain events type
            Type myType = genericType.MakeGenericType(@event.GetType());

            //Get a list of all the handlers for this message type
            var handlers = _unityContainer.ResolveAll(myType);

            if (handlers != null && handlers.Any())
            {
                var tasks = new List<Task>();

                foreach (var handler in handlers)
                {
                    tasks.Add(Task.Run(async() =>
                    {
                        try
                        {
                            await (Task)myType.InvokeMember("HandleAsync", BindingFlags.InvokeMethod, null, handler, new[] { @event });
                        }
                        catch
                        {
                            //push the message to an error queue identifying which handler failed
                            Trace.WriteLine(string.Format("Exception handling {0}", @event.GetType().FullName));
                        }
                    }));
                }

                await Task.WhenAll(tasks);
            }
        }

        private async Task PublishToQueueAsync<T>(IEnumerable<T> events)
            where T : IEvent
        {
            var serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var serviceBusQueue = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.EventQueue");

            var factory = MessagingFactory.CreateFromConnectionString(serviceBusConnectionString);
            var messageSender = factory.CreateMessageSender(serviceBusQueue);

            var tasks = new List<Task>();

            foreach (var @event in events)
            {
                var message = new BrokeredMessage();
                var json = JsonConvert.SerializeObject(@event);

                message.Properties.Add("json", json);
                message.Properties.Add("type", @event.GetType().AssemblyQualifiedName);

                tasks.Add(messageSender.SendAsync(message));
            }

            await Task.WhenAll(tasks);

            messageSender.Close();
        }

        private async Task SendAsync<T>(T command)
            where T : class, ICommand
        {
            if (command == null)
                return;

            //Get a instance of the generic handler's type
            Type genericType = typeof(IHandles<>);

            //Get and instance of the messages type
            Type myType = genericType.MakeGenericType(command.GetType());

            //Get a list of all the handlers for this message type
            var handlers = _unityContainer.ResolveAll(myType);

            if (handlers != null && handlers.Any())
            {
                if (handlers.Count() > 1)
                {
                    Trace.WriteLine("A command should only have one handler.");
                    return;
                }

                var handler = handlers.First();

                try
                {
                    await (Task)myType.InvokeMember("HandleAsync", BindingFlags.InvokeMethod, null, handler, new[] { command });
                }
                catch (TargetInvocationException x)
                {
                    if (x.InnerException != null)
                    {
                        ExceptionDispatchInfo.Capture(x.InnerException).Throw();
                    }

                    throw;
                }
            }
            else
            {
                Trace.WriteLine(string.Format("No handler for the following command: {0}", command.GetType().FullName));
            }
        }
    }
}
