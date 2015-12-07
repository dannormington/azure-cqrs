using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using SimpleCQRS.Handlers;
using SimpleCQRS.Events;
using Newtonsoft.Json;
using SimpleCQRS.Infrastructure;

namespace SimpleCQRSWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        QueueClient _queueClient;
        ManualResetEvent _completedEvent = new ManualResetEvent(false);
        IEventBus _eventBus;
        
        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _queueClient.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        ProcessMessage(receivedMessage);
                    }
                    catch
                    {
                        // Handle any message processing specific exceptions here
                    }
                });

            _completedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            RegisterHandlers();

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            string queue = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.EventQueue");
                
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(queue))
            {
                namespaceManager.CreateQueue(queue);
            }

            // Initialize the connection to Service Bus Queue
            _queueClient = QueueClient.CreateFromConnectionString(connectionString, queue);
            return base.OnStart();
        }

        public override void OnStop()
        {
            _eventBus.Dispose();
            
            // Close the connection to Service Bus Queue
            _queueClient.Close();
            _completedEvent.Set();
            base.OnStop();
        }

        private void ProcessMessage(BrokeredMessage receivedMessage) 
        {
            if (receivedMessage == null)
                return;

            if (!receivedMessage.Properties.ContainsKey("json") || !receivedMessage.Properties.ContainsKey("type"))
                return;

            var json = receivedMessage.Properties["json"].ToString();
            var type = receivedMessage.Properties["type"].ToString();

            var messageType = Type.GetType(type);

            var message = JsonConvert.DeserializeObject(json, messageType);

            _eventBus.PublishAsync(message);

            receivedMessage.Complete();
        }

        private void RegisterHandlers() 
        {
            _eventBus = new EventBus();
            _eventBus.RegisterEventHandler<AttendeeRegistered, ConferenceEventHandler>();
            _eventBus.RegisterEventHandler<AttendeeEmailChanged, ConferenceEventHandler>();
            _eventBus.RegisterEventHandler<AttendeeChangeEmailConfirmed, ConferenceEventHandler>();
        }
    }
}
