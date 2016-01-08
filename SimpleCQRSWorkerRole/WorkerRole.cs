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
using Microsoft.Azure;
using Microsoft.Practices.Unity;
using SimpleCQRS.Infrastructure.Query;

namespace SimpleCQRSWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        QueueClient _queueClient;
        ManualResetEvent _completedEvent = new ManualResetEvent(false);
        IUnityContainer _unityContainer = new UnityContainer();
        
        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            //Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            _queueClient.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        ProcessMessage(receivedMessage);
                    }
                    catch (Exception ex)
                    {
                        // Handle any message processing specific exceptions here
                        Trace.WriteLine(ex.Message);
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
            _unityContainer.Dispose();
            
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

            var messageBus = _unityContainer.Resolve<IMessageBus>();
            messageBus.PublishAsync(message).Wait();

            receivedMessage.Complete();
        }

        private void RegisterHandlers() 
        {
            IMessageBus messageBus = new MessageBus(_unityContainer);
            messageBus.RegisterHandler<AttendeeRegistered, ConferenceEventHandler>();
            messageBus.RegisterHandler<AttendeeEmailChanged, ConferenceEventHandler>();
            messageBus.RegisterHandler<AttendeeChangeEmailConfirmed, ConferenceEventHandler>();
            messageBus.RegisterHandler<AttendeeUnregistered, ConferenceEventHandler>();

            _unityContainer.RegisterInstance(messageBus);
            _unityContainer.RegisterType<IAttendeeDataAccess, AttendeeDataAccess>();
        }
    }
}
